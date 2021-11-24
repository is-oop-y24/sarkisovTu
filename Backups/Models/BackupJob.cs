using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;

namespace Backups.Models
{
    public class BackupJob<T>
        where T : IRepository
    {
        private const string BackupJobNamePattern = "Backup-job";
        private const string BackupDatePattern = "yyyy-MM-dd - H mm ss ffff";
        private const string RestorePointNamePattern = "Restore-point";

        private string _name;
        private T _repositorySystem;
        private string _pathToSave;
        private int _backupType;
        private List<JobObject> _jobObjects;
        private List<RestorePoint> _restorePoints;

        public BackupJob(string name, T repositorySystem, BackupAlgorithmType type, string pathToSave)
        {
            _name = name;
            _repositorySystem = repositorySystem;
            _pathToSave = pathToSave;
            _backupType = (int)type;
            _jobObjects = new List<JobObject>();
            _restorePoints = new List<RestorePoint>();
        }

        public List<RestorePoint> RestorePoints
        {
            get { return _restorePoints; }
        }

        public JobObject AddJobObject(string pathToFile)
        {
            if (FindJobByPath(pathToFile) != null)
            {
                throw new Exception("Job with this file have been already added");
            }

            JobObject newJobObject = new JobObject(_repositorySystem, pathToFile);
            _jobObjects.Add(newJobObject);
            return newJobObject;
        }

        public void RemoveJobObject(string pathToFile)
        {
            JobObject queryJob = FindJobByPath(pathToFile);
            if (queryJob != null)
            {
                _jobObjects.Remove(queryJob);
            }
        }

        public void RemoveJobObject(JobObject jobObject)
        {
            _jobObjects.Remove(jobObject);
        }

        public JobObject FindJobByPath(string path)
        {
            return _jobObjects.Find(job => job.Path == path);
        }

        public RestorePoint CreateRestorePoint()
        {
            List<BackupStorage> backupStorages = _jobObjects.Select(curJob => new BackupStorage(_repositorySystem, curJob.Path, curJob.Content)).ToList();
            RestorePoint newRestorePoint = new RestorePoint(backupStorages);
            _restorePoints.Add(newRestorePoint);
            string restorePointCreationDate = newRestorePoint.DateOfCreation.ToString(BackupDatePattern);

            string backupDirectory = _repositorySystem.JoinPath(_pathToSave, $"\\{BackupJobNamePattern} {_name}");
            string restorePointDirectory = _repositorySystem.JoinPath(backupDirectory, $"\\{RestorePointNamePattern} {restorePointCreationDate}");
            _repositorySystem.CreateDirectory(backupDirectory);
            _repositorySystem.CreateDirectory(restorePointDirectory);

            if (_backupType == 0)
            {
                foreach (var storage in backupStorages)
                {
                    string curStorageArchivePath = _repositorySystem.JoinPath(restorePointDirectory, $"\\{storage.Name.Split(".")[0]}.zip");
                    _repositorySystem.CreateArchive(curStorageArchivePath, new List<BackupStorage>() { storage });
                }
            }

            if (_backupType == 1)
            {
                const string backupArchiveName = "Backup-collection";
                string curStorageArchivePath = _repositorySystem.JoinPath(restorePointDirectory, $"\\{backupArchiveName}.zip");
                _repositorySystem.CreateArchive(curStorageArchivePath, backupStorages);
            }

            return newRestorePoint;
        }

        public string GetPathOfRestorePoint(RestorePoint restorePoint)
        {
            string backupDirectory = _repositorySystem.JoinPath(_pathToSave, $"\\{BackupJobNamePattern} {_name}");
            string restorePointCreationDate = restorePoint.DateOfCreation.ToString(BackupDatePattern);
            string restorePointDirectory = _repositorySystem.JoinPath(backupDirectory, $"\\{RestorePointNamePattern} {restorePointCreationDate}");
            return restorePointDirectory;
        }
    }
}