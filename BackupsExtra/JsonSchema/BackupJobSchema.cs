using System.Collections.Generic;
using Backups.Models;

namespace BackupsExtra.JsonSchema
{
    public class BackupJobSchema
    {
        public BackupJobSchema()
        {
        }

        public BackupJobSchema(string name, string repositorySystem, string pathToSave, string backupAlgorithm, List<JobObjectSchema> jobObjects, List<RestorePointSchema> restorePoints)
        {
            Name = name;
            RepositorySystem = repositorySystem;
            PathToSave = pathToSave;
            BackupAlgorithm = backupAlgorithm;

            JobObjects = jobObjects;
            RestorePoints = restorePoints;
        }

        public string Name { get; set; }
        public string RepositorySystem { get; set; }
        public string PathToSave { get; set; }
        public string BackupAlgorithm { get; set; }
        public List<JobObjectSchema> JobObjects { get; set; }
        public List<RestorePointSchema> RestorePoints { get; set; }
    }
}