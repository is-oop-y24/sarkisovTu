using System.Collections.Generic;

namespace BackupsExtra.JsonSchema
{
    public class JsonValuesSchema
    {
        public JsonValuesSchema()
        {
        }

        public JsonValuesSchema(List<BackupJobSchema> backupJobs)
        {
            BackupJobs = backupJobs;
        }

        public List<BackupJobSchema> BackupJobs { get; set; }
    }
}