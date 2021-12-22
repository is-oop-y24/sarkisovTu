using System;
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

        public List<RestorePoint> OptimizeRestorePoints(BackupJob backupJob)
        {
            List<RestorePoint> restorePoints = backupJob.RestorePoints;
            List<RestorePoint> optimizedRestorePoints = new List<RestorePoint>();
            List<RestorePoint> capacityOptimized = new List<RestorePoint>();
            List<RestorePoint> dateOptimized = new List<RestorePoint>();

            if (Configuration.Capacity != default(int))
            {
                capacityOptimized = OptimizeByCapacity(restorePoints);
            }

            if (Configuration.Date != default(DateTime))
            {
                dateOptimized = OptimizeByTime(restorePoints);
            }

            if (Configuration.IsHybrid)
            {
                if (Configuration.OptimizationType == RestorePointOptimizationType.Soft) optimizedRestorePoints = capacityOptimized.Union(dateOptimized).ToList();

                if (Configuration.OptimizationType == RestorePointOptimizationType.Strict) optimizedRestorePoints = capacityOptimized.Intersect(dateOptimized).ToList();
            }
            else
            {
                if (capacityOptimized.Any()) optimizedRestorePoints = capacityOptimized;
                if (dateOptimized.Any()) optimizedRestorePoints = dateOptimized;
            }

            Console.WriteLine(dateOptimized.Count);
            if (optimizedRestorePoints.Count == 0) throw new BackupsExtraException("Optimization method deleted all restore points");
            int intersectionIndex = restorePoints.IndexOf(optimizedRestorePoints[0]);
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
            return optimizedRestorePoints;
        }

        private List<RestorePoint> OptimizeByCapacity(List<RestorePoint> restorePoints)
        {
            return Enumerable.Reverse(restorePoints).Take(Configuration.Capacity).Reverse().ToList();
        }

        private List<RestorePoint> OptimizeByTime(List<RestorePoint> restorePoints)
        {
            return restorePoints.FindAll(restorePoint => DateTime.Compare(restorePoint.DateOfCreation, Configuration.Date) >= 0);
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