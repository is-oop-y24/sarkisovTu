using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;

namespace Backups.Models
{
    public class RestorePoint
    {
        public RestorePoint(List<BackupStorage> backupStorages)
        {
            DateOfCreation = DateTime.Now;
            Storages = backupStorages;
        }

        public DateTime DateOfCreation { get; private set; }
        public List<BackupStorage> Storages { get; private set; }
    }
}