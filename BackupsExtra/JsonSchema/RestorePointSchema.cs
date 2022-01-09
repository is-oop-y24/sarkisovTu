using System.Collections.Generic;
using Backups.Models;

namespace BackupsExtra.JsonSchema
{
    public class RestorePointSchema
    {
        public RestorePointSchema()
        {
        }

        public RestorePointSchema(string date, List<StorageSchema> storagesSchema)
        {
            Date = date;
            StoragesSchema = storagesSchema;
        }

        public string Date { get; set; }
        public List<StorageSchema> StoragesSchema { get; set; }
    }
}