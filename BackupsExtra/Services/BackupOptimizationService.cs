using System.Collections.Generic;
using System.Linq;
using Backups.Models;
using BackupsExtra.Models;
using BackupsExtra.Tools;
using BackupsExtra.Types;

namespace BackupsExtra.Services
{
    public class BackupOptimizationService
    {
        public BackupOptimizationService(OptimizationConfiguration configuration)
        {
            Configuration = configuration;
        }

        public OptimizationConfiguration Configuration { get; private set; }

        public void FileSystemOptimization(BackupJob backupJob, List<RestorePoint> optimizedRestorePoints)
        {
            int intersectionIndex = backupJob.RestorePoints.IndexOf(optimizedRestorePoints[0]);
            if (Configuration.MergeEnabled) MergePoints(backupJob, intersectionIndex, optimizedRestorePoints[0]);

            backupJob.RepositorySystem.DeleteFolder(backupJob.RepositorySystem.JoinPath(backupJob.PathToSave, $"\\{backupJob.BackupJobNamePatternValue} {backupJob.Name}"));
            optimizedRestorePoints.ForEach(restorePoint =>
            {
                List<JobObject> jobObjects = restorePoint.Storages.Select(storage => new JobObject(backupJob.RepositorySystem, storage.Path)).ToList();
                backupJob.BackupAlgorithm.CreateRestorePoint(
                    jobObjects,
                    backupJob.RepositorySystem,
                    backupJob.BackupJobNamePatternValue,
                    backupJob.BackupDatePatternValue,
                    backupJob.RestorePointNamePatternValue,
                    backupJob.PathToSave,
                    backupJob.Name,
                    restorePoint.DateOfCreation);
            });
        }

        public List<RestorePoint> OptimizeRestorePoints(BackupJob backupJob)
        {
            if (Configuration.Algorithms.Count != 1) throw new BackupsExtraException("Optimization type wasn't provided");
            List<RestorePoint> optimizedRestorePoints = Configuration.Algorithms[0].Run(backupJob.RestorePoints);
            if (optimizedRestorePoints.Count == 0) throw new BackupsExtraException("Optimization method deleted all restore points");
            FileSystemOptimization(backupJob, optimizedRestorePoints);
            return optimizedRestorePoints;
        }

        public List<RestorePoint> OptimizeRestorePoints(BackupJob backupJob, RestorePointOptimizationType optimizationType)
        {
            List<RestorePoint> optimizedRestorePoints = Configuration.Algorithms[0].Run(backupJob.RestorePoints);
            for (int i = 1; i < Configuration.Algorithms.Count; i++)
            {
                if (optimizationType == RestorePointOptimizationType.Soft)
                {
                    optimizedRestorePoints = Configuration.Algorithms[i].Run(backupJob.RestorePoints).Union(optimizedRestorePoints).ToList();
                }
                else if (optimizationType == RestorePointOptimizationType.Strict)
                {
                    optimizedRestorePoints = Configuration.Algorithms[i].Run(backupJob.RestorePoints).Intersect(optimizedRestorePoints).ToList();
                }
            }

            if (optimizedRestorePoints.Count == 0) throw new BackupsExtraException("Optimization method deleted all restore points");
            FileSystemOptimization(backupJob, optimizedRestorePoints);
            return optimizedRestorePoints;
        }

        private void MergePoints(BackupJob backupJob, int intersectionIndex, RestorePoint pointToMerge)
        {
            if (backupJob.BackupAlgorithm.GetType().Name == "SingleStoragesAlgorithm") return;
            for (int i = 0; i < intersectionIndex; i++)
            {
                List<BackupStorage> storagesToMerge = backupJob.RestorePoints[i].Storages
                    .Where(x => !pointToMerge.Storages.Any(y => y.Name == x.Name))
                    .ToList();
                foreach (BackupStorage storage in storagesToMerge) pointToMerge.AddNewStorage(storage);
            }
        }
    }
}