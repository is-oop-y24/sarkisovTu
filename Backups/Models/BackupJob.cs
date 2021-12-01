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
        private IBackupAlgorithm _backupAlgorithm;
        private List<JobObject> _jobObjects;
        private List<RestorePoint> _restorePoints;

        public BackupJob(string name, T repositorySystem, IBackupAlgorithm backupAlgorithm, string pathToSave)
        {
            _name = name;
            _repositorySystem = repositorySystem;
            _pathToSave = pathToSave;
            _backupAlgorithm = backupAlgorithm;
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
            RestorePoint newRestorePoint = _backupAlgorithm.CreateRestorePoint(_jobObjects, _repositorySystem, BackupJobNamePattern, BackupDatePattern, RestorePointNamePattern, _pathToSave, _name);
            _restorePoints.Add(newRestorePoint);
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