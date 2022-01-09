using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Backups.Models;
using Backups.Repository;
using BackupsExtra.JsonSchema;
using BackupsExtra.Services;
using BackupsExtra.Tools;
using BackupsExtra.Types;

namespace BackupsExtra.Models
{
    public class BackupsJobService
    {
        private const string BackupDatePattern = "yyyy-MM-dd - H mm ss ffff";
        private List<INotificationClient> _notificationClients;
        private string _configFilePath;
        private List<BackupJob> _backupJobs;
        private IRepository _repositorySystem;
        private BackupOptimizationService _optimizationService;
        private RestorePointOptimizationType _optimizationType;

        public BackupsJobService(IRepository repositorySystem, string configFilePath, List<IOptimizationAlgorithm> algorithms, bool mergeEnabled, RestorePointOptimizationType optimizationType)
        {
            _notificationClients = new List<INotificationClient>();
            _configFilePath = configFilePath;
            _repositorySystem = repositorySystem;
            _optimizationService = new BackupOptimizationService(new OptimizationConfiguration(algorithms, mergeEnabled));
            _optimizationType = optimizationType;
            _backupJobs = new List<BackupJob>();
            _backupJobs.AddRange(ConvertSchemaToModels(LoadState()));
        }

        public IRepository RepositorySystem { get { return _repositorySystem;  } }

        public BackupJob CreateNewBackupJob(string name, IRepository repositorySystem, IBackupAlgorithm backupAlgorithm, string pathToSave)
        {
            if (_backupJobs.Find(backupJob => backupJob.Name == name) != null) throw new BackupsExtraException("Job with this name was already created");
            BackupJob newBackupJob = new BackupJob(name, repositorySystem, backupAlgorithm, pathToSave);
            _backupJobs.Add(newBackupJob);
            NotifyClients(new NotificationMessage(this, DateTime.Now, $"Backup job with name {newBackupJob.Name} was created"));
            return newBackupJob;
        }

        public BackupJob GetBackupJobByName(string name)
        {
            return _backupJobs.Find(backupJob => backupJob.Name == name);
        }

        public void CreateRestorePoint(BackupJob backupJob, DateTime date = default(DateTime))
        {
            backupJob.CreateRestorePoint(date);
            backupJob.ReloadRestorePoints(_optimizationService.OptimizeRestorePoints(backupJob, _optimizationType));
            NotifyClients(new NotificationMessage(this, DateTime.Now, $"{backupJob.Name} created new restore point"));
        }

        public void SaveState()
        {
            List<BackupJobSchema> backupJobsSchema = new List<BackupJobSchema>();
            _backupJobs.ForEach(backupJob =>
            {
                List<JobObjectSchema> jobObjectsSchema = backupJob.JobObjects.Select(jobObject => new JobObjectSchema(jobObject.Path)).ToList();

                List<RestorePointSchema> restorePointsSchema = backupJob.RestorePoints.Select(restorePoint =>
                {
                    List<StorageSchema> storagesSchema = restorePoint.Storages.Select(restorePoint => new StorageSchema(restorePoint.Path, restorePoint.Content)).ToList();
                    return new RestorePointSchema(restorePoint.DateOfCreation.ToString(backupJob.BackupDatePatternValue), storagesSchema);
                }).ToList();

                backupJobsSchema.Add(new BackupJobSchema(
                    backupJob.Name,
                    backupJob.RepositorySystem.GetType().Name,
                    backupJob.PathToSave,
                    backupJob.BackupAlgorithm.GetType().Name,
                    jobObjectsSchema,
                    restorePointsSchema));
            });
            JsonValuesSchema backupState = new JsonValuesSchema(backupJobsSchema);
            string jsonOutput = JsonSerializer.Serialize(backupState);
            _repositorySystem.UpdateFile(_configFilePath, jsonOutput);

            NotifyClients(new NotificationMessage(this, DateTime.Now, $"Current state was saved into configuration file"));
        }

        public JsonValuesSchema LoadState()
        {
            string jsonInput = _repositorySystem.ReadFile(_configFilePath);
            NotifyClients(new NotificationMessage(this, DateTime.Now, $"Try to parse JSON configuration file"));
            return JsonSerializer.Deserialize<JsonValuesSchema>(jsonInput, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        }

        public List<BackupJob> ConvertSchemaToModels(JsonValuesSchema schema)
        {
            return schema.BackupJobs.Select(schemaJob =>
            {
                IBackupAlgorithm backupAlgorithm = (IBackupAlgorithm)CreateType(schemaJob.BackupAlgorithm);
                BackupJob newBackupJob = new BackupJob(schemaJob.Name, _repositorySystem, backupAlgorithm, schemaJob.PathToSave);
                List<string> jobObjectsPath = schemaJob.JobObjects.Select(jobObject => jobObject.Path).ToList();
                newBackupJob.AddJobObjects(jobObjectsPath);
                List<RestorePoint> restorePoints = schemaJob.RestorePoints.Select(restorePoint =>
                {
                        List<BackupStorage> backupStorages = restorePoint.StoragesSchema.Select(storage => new BackupStorage(_repositorySystem, storage.Path, storage.Content)).ToList();
                        return new RestorePoint(backupStorages, DateTime.ParseExact(restorePoint.Date, BackupDatePattern, System.Globalization.CultureInfo.InvariantCulture));
                }).ToList();
                newBackupJob.ReloadRestorePoints(restorePoints);
                return newBackupJob;
            }).ToList();
        }

        public void UpBackRestorePoint(RestorePoint restorePoint, IRestoreAlgorithm algorithm, string path = "")
        {
            algorithm.Restore(restorePoint, path);
            NotifyClients(new NotificationMessage(this, DateTime.Now, $"Restore point storages was restored"));
        }

        public void AttachNotificationClient(INotificationClient notificationClient)
        {
            _notificationClients.Add(notificationClient);
        }

        public void DetachNotificationClient(INotificationClient notificationClient)
        {
            _notificationClients.Remove(notificationClient);
        }

        public void NotifyClients(NotificationMessage message)
        {
            if (_notificationClients.Count != 0)
            {
                foreach (INotificationClient notificationClient in _notificationClients)
                {
                    notificationClient.Update(message);
                }
            }
        }

        private object CreateType(string typeName)
        {
            IEnumerable<Type> allTypes = AppDomain.CurrentDomain.GetAssemblies().SelectMany(a => a.GetTypes());
            Type queryType = allTypes.Single(t => t.Name == typeName);
            return Activator.CreateInstance(queryType);
        }
    }
}