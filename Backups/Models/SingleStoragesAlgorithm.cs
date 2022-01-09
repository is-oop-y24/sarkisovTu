using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;

namespace Backups.Models
{
    public class SingleStoragesAlgorithm : IBackupAlgorithm
    {
        public RestorePoint CreateRestorePoint(List<JobObject> jobObjects, IRepository repositorySystem, string backupJobNamePattern, string backupDatePattern, string restorePointNamePattern, string pathToSave, string jobName, DateTime date)
        {
            List<BackupStorage> backupStorages = jobObjects.Select(curJob => new BackupStorage(repositorySystem, curJob.Path, curJob.Content)).ToList();
            RestorePoint newRestorePoint = new RestorePoint(backupStorages, date);
            string restorePointCreationDate = newRestorePoint.DateOfCreation.ToString(backupDatePattern);

            string backupDirectory = repositorySystem.JoinPath(pathToSave, $"\\{backupJobNamePattern} {jobName}");
            string restorePointDirectory = repositorySystem.JoinPath(backupDirectory, $"\\{restorePointNamePattern} {restorePointCreationDate}");
            repositorySystem.CreateDirectory(backupDirectory);
            repositorySystem.CreateDirectory(restorePointDirectory);

            const string backupArchiveName = "Backup-collection";
            string curStorageArchivePath = repositorySystem.JoinPath(restorePointDirectory, $"\\{backupArchiveName}.zip");
            repositorySystem.CreateArchive(curStorageArchivePath, backupStorages);

            return newRestorePoint;
        }
    }
}