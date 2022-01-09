using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;

namespace Backups.Models
{
    public class BackupJob
    {
        private const string BackupJobNamePattern = "Backup-job";
        private const string BackupDatePattern = "yyyy-MM-dd - H mm ss ffff";
        private const string RestorePointNamePattern = "Restore-point";
        public BackupJob(string name, IRepository repositorySystem, IBackupAlgorithm backupAlgorithm, string pathToSave)
        {
            Name = name;
            RepositorySystem = repositorySystem;
            PathToSave = pathToSave;
            BackupAlgorithm = backupAlgorithm;
            JobObjects = new List<JobObject>();
            RestorePoints = new List<RestorePoint>();
        }

        public string Name { get; private set; }
        public IRepository RepositorySystem { get; private set; }
        public string PathToSave { get; private set; }
        public IBackupAlgorithm BackupAlgorithm { get; private set; }
        public List<JobObject> JobObjects { get; private set; }
        public List<RestorePoint> RestorePoints { get; private set; }
        public string BackupJobNamePatternValue { get { return BackupJobNamePattern; } }
        public string BackupDatePatternValue { get { return BackupDatePattern; } }
        public string RestorePointNamePatternValue { get { return RestorePointNamePattern; } }

        public void ValidateJobObjectPath(string pathToFile)
        {
            if (FindJobByPath(pathToFile) != null)
            {
                throw new Exception("Job with this file have been already added");
            }
        }

        public JobObject AddJobObject(string pathToFile)
        {
            ValidateJobObjectPath(pathToFile);
            JobObject newJobObject = new JobObject(RepositorySystem, pathToFile);
            JobObjects.Add(newJobObject);
            return newJobObject;
        }

        public void AddJobObjects(List<string> jobObjectsPath)
        {
            List<JobObject> jobObjects = jobObjectsPath.Select(jobObjectPath =>
            {
                ValidateJobObjectPath(jobObjectPath);
                return new JobObject(RepositorySystem, jobObjectPath);
            }).ToList();
            JobObjects.AddRange(jobObjects);
        }

        public void RemoveJobObject(string pathToFile)
        {
            JobObject queryJob = FindJobByPath(pathToFile);
            if (queryJob != null)
            {
                JobObjects.Remove(queryJob);
            }
        }

        public void RemoveJobObject(JobObject jobObject)
        {
            JobObjects.Remove(jobObject);
        }

        public JobObject FindJobByPath(string path)
        {
            return JobObjects.Find(job => job.Path == path);
        }

        public RestorePoint CreateRestorePoint(DateTime date = default(DateTime))
        {
            RestorePoint newRestorePoint = BackupAlgorithm.CreateRestorePoint(JobObjects, RepositorySystem, BackupJobNamePattern, BackupDatePattern, RestorePointNamePattern, PathToSave, Name, date);
            RestorePoints.Add(newRestorePoint);
            return newRestorePoint;
        }

        public string GetPathOfRestorePoint(RestorePoint restorePoint)
        {
            string backupDirectory = RepositorySystem.JoinPath(PathToSave, $"\\{BackupJobNamePattern} {Name}");
            string restorePointCreationDate = restorePoint.DateOfCreation.ToString(BackupDatePattern);
            string restorePointDirectory = RepositorySystem.JoinPath(backupDirectory, $"\\{RestorePointNamePattern} {restorePointCreationDate}");
            return restorePointDirectory;
        }

        public void ReloadRestorePoints(List<RestorePoint> restorePoints)
        {
            RestorePoints.Clear();
            RestorePoints.AddRange(restorePoints);
        }
    }
}