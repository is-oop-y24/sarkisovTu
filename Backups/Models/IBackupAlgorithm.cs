using System;
using System.Collections.Generic;
using Backups.Repository;

namespace Backups.Models
{
    public interface IBackupAlgorithm
    {
        RestorePoint CreateRestorePoint(List<JobObject> jobObjects, IRepository repositorySystem, string backupJobNamePattern, string backupDatePattern, string restorePointNamePattern, string pathToSave, string jobName, DateTime date);
    }
}