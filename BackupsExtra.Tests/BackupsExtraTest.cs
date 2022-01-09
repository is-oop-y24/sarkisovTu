using System;
using System.Collections.Generic;
using Backups.Models;
using Backups.Repository;
using BackupsExtra.Models;
using BackupsExtra.Tools;
using BackupsExtra.Types;
using NUnit.Framework;

namespace BackupsExtra.Tests
{
    public class BackupsExtraTest
    {
        private IRepository _inMemoryRepository;
        
        [SetUp]
        public void Setup()
        {
            _inMemoryRepository = new InMemoryRepository();
            _inMemoryRepository.CreateDirectory(@"root\files");
            _inMemoryRepository.CreateDirectory(@"root\output");
            _inMemoryRepository.CreateDirectory(@"root\upback");
            _inMemoryRepository.CreateFileInDirectory(@"root", "logs.txt", "");
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "1.txt", "Hello World");
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "2.txt", "Foo bar");
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "3.txt", "XYZ");
            string jsonInput = "{\"BackupJobs\":[{\"Name\":\"test job\",\"RepositorySystem\":\"InMemoryRepository\",\"PathToSave\":\"root\\\\output\",\"BackupAlgorithm\":\"SplitStoragesAlgorithm\",\"JobObjects\":[{\"Path\":\"root\\\\files\\\\2.txt\"}],\"RestorePoints\":[{\"Date\":\"2021-12-21 - 17 28 21 4619\",\"StoragesSchema\":[{\"Path\":\"root\\\\files\\\\3.txt\",\"Content\":\"321sio\"}]},{\"Date\":\"2021-12-21 - 17 28 21 4922\",\"StoragesSchema\":[{\"Path\":\"root\\\\files\\\\1.txt\",\"Content\":\"321sio\"}]},{\"Date\":\"2021-12-21 - 17 28 21 4994\",\"StoragesSchema\":[{\"Path\":\"root\\\\files\\\\2.txt\",\"Content\":\"123666\"},{\"Path\":\"root\\\\files\\\\1.txt\",\"Content\":\"321sio\"}]}]}]}";
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "config.json", jsonInput);
            
        }

        [Test]
        public void LoadStateFromJsonFile_StateWasLoaded()
        {
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new CapacityOptimization(3) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            Assert.AreEqual(loadedBackupJob.Name, "test job");
            Assert.AreEqual(loadedBackupJob.PathToSave, @"root\output");
            Assert.AreEqual(loadedBackupJob.RestorePoints.Count, 3);
        }

        [Test]
        public void LimitRestorePointsWithCapacity_ExtraPointWereDeleted()
        {
            int capacityLimit = 2;
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new CapacityOptimization(capacityLimit) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            int restorePointsCountBeforeOptimization = loadedBackupJob.RestorePoints.Count;
            backupsJobService.CreateRestorePoint(loadedBackupJob);
            int restorePointsCountAfterOptimization = loadedBackupJob.RestorePoints.Count;
            Assert.AreNotEqual(restorePointsCountBeforeOptimization, restorePointsCountAfterOptimization);
            Assert.AreEqual(restorePointsCountAfterOptimization, capacityLimit);
        }

        [Test]
        public void LimitRestorePointsWithFutureDateTime_AllPointsWereDeletedThrowException()
        {
            DateTime timeLimit = DateTime.Now.AddDays(10000);
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new TimeOptimization(timeLimit) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            Assert.Catch<BackupsExtraException>(() =>
            {
                backupsJobService.CreateRestorePoint(loadedBackupJob);
            });
        }

        [Test]
        public void UpBackRestorePointToOriginalLocation_FilesWereRestored()
        {
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new CapacityOptimization(3) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            RestorePoint restorePoint = loadedBackupJob.RestorePoints[0];
            backupsJobService.UpBackRestorePoint(restorePoint, new OriginalLocationAlgorithm());
            restorePoint.Storages.ForEach(strorage =>
            {
                Assert.AreEqual(strorage.Content, _inMemoryRepository.ReadFile(strorage.Path));
            });
        }

        [Test]
        public void UpBackRestorePointToDifferentLocationFilesWereRestored()
        {
            string directoryToRestore = @"root\upback";
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new CapacityOptimization(3) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            RestorePoint restorePoint = loadedBackupJob.RestorePoints[0];
            backupsJobService.UpBackRestorePoint(restorePoint, new DifferentLocationAlgorithm(), directoryToRestore);
            restorePoint.Storages.ForEach(strorage =>
            {
                Assert.AreEqual(strorage.Content, _inMemoryRepository.ReadFile(_inMemoryRepository.JoinPath(directoryToRestore, strorage.Name)));
            });
        }

        [Test]
        public void SendFileNotificationOnCreatingRestorePoint_NotificationWasSent()
        {
            string notificationFilePath = @"root\logs.txt";
            var backupsJobService = new BackupsJobService(_inMemoryRepository, @"root\files\config.json", new List<IOptimizationAlgorithm>() { new CapacityOptimization(3) }, true, RestorePointOptimizationType.Soft);
            BackupJob loadedBackupJob = backupsJobService.GetBackupJobByName("test job");
            INotificationClient fsNotificationClient = new FsNotificationClient(notificationFilePath);
            backupsJobService.AttachNotificationClient(fsNotificationClient);
            Assert.AreEqual(_inMemoryRepository.ReadFile(notificationFilePath), "");
            backupsJobService.CreateRestorePoint(loadedBackupJob);
            Assert.AreNotEqual(_inMemoryRepository.ReadFile(notificationFilePath), "");
        }
    }
}