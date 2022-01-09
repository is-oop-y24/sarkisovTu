using System;
using System.Collections.Generic;
using System.Linq;
using Backups.Repository;

namespace Backups.Models
{
    public class RestorePoint
    {
        public RestorePoint(List<BackupStorage> backupStorages, DateTime date)
        {
            Storages = backupStorages;
            if (date == default(DateTime))
            {
                DateOfCreation = DateTime.Now;
            }
            else
            {
                DateOfCreation = date;
            }
        }

        public List<BackupStorage> Storages { get; private set; }
        public DateTime DateOfCreation { get; private set; }

        public void AddNewStorage(BackupStorage storage)
        {
            Storages.Add(storage);
        }

        public void RemoveStorage(BackupStorage storage)
        {
            Storages.Remove(storage);
        }
    }
}