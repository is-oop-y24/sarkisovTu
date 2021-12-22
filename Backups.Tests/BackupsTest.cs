using Backups.Models;
using Backups.Repository;
using Backups.Tools;
using NUnit.Framework;

namespace Backups.Tests
{
    public class Tests
    {
        private InMemoryRepository _inMemoryRepository;

        [SetUp]
        public void Setup()
        {
            _inMemoryRepository = new InMemoryRepository();
            
            _inMemoryRepository.CreateDirectory(@"root\files");
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "1.txt", "Hello World");
            _inMemoryRepository.CreateFileInDirectory(@"root\files", "2.txt", "Foo bar");
        }

        [Test]
        public void CreateNewRestorePointAfterChangingFile_StorageContainsUpdatedFile()
        {
            var backupJob = new BackupJob("test job", _inMemoryRepository, new SingleStoragesAlgorithm(), @"root\output");
            backupJob.AddJobObject(@"root\files\1.txt");
            RestorePoint restorePoint1 = backupJob.CreateRestorePoint();
            string restorePointPath1 = backupJob.GetPathOfRestorePoint(restorePoint1);
            
            //"Backup-collection" - default name of zip archive in Single storage mode
            string restoreArchivePath1 = _inMemoryRepository.JoinPath(restorePointPath1, @"\Backup-collection.zip");
            string createdFileName1 = _inMemoryRepository.GetFilesInDirectory(restoreArchivePath1)[0];
            string createdFilePath1 = _inMemoryRepository.JoinPath(restoreArchivePath1, createdFileName1);
            Assert.AreEqual(_inMemoryRepository.ReadFile(createdFilePath1), _inMemoryRepository.ReadFile(@"root\files\1.txt"));
            
            _inMemoryRepository.UpdateFile(@"root\files\1.txt", "World Hello");
            
            RestorePoint restorePoint2 = backupJob.CreateRestorePoint();
            string restorePointPath2 = backupJob.GetPathOfRestorePoint(restorePoint2);
            string restoreArchivePath2 = _inMemoryRepository.JoinPath(restorePointPath2, @"\Backup-collection.zip");
            string createdFileName2 = _inMemoryRepository.GetFilesInDirectory(restoreArchivePath2)[0];
            string createdFilePath2 = _inMemoryRepository.JoinPath(restoreArchivePath2, createdFileName2);
            
            
            Assert.AreEqual(_inMemoryRepository.ReadFile(createdFilePath2), _inMemoryRepository.ReadFile(@"root\files\1.txt"));
        }
        
        [Test]
        public void RunJobWithObjectReferToDeletedFile_ThrowException()
        {
            var backupJob = new BackupJob("test job", _inMemoryRepository, new SingleStoragesAlgorithm(), @"root\output");
            backupJob.AddJobObject(@"root\files\1.txt");
            backupJob.AddJobObject(@"root\files\2.txt");
            _inMemoryRepository.DeleteFile(@"root\files\1.txt");
            Assert.Catch<BackupsException>(() =>
            {
                backupJob.CreateRestorePoint();
            });
        }

        [Test]
        public void RunSplitStoragesJobAndDeleteOneFile_TwoRestorePointsAndThreeStoragesWereCreated()
        {
            var backupJobSplit = new BackupJob("Split job", _inMemoryRepository, new SplitStoragesAlgorithm(), @"root\output");
            backupJobSplit.AddJobObject(@"root\files\1.txt");
            backupJobSplit.AddJobObject(@"root\files\2.txt");
            backupJobSplit.CreateRestorePoint();
            backupJobSplit.RemoveJobObject(@"root\files\1.txt");
            backupJobSplit.CreateRestorePoint();
            int storagesCount = 0;
            backupJobSplit.RestorePoints.ForEach(point => storagesCount += point.Storages.Count);
            
            Assert.AreEqual(2, backupJobSplit.RestorePoints.Count);
            Assert.AreEqual(3, storagesCount);

        }

        [Test]
        public void RunSingleJobWithTwoFiles_DirectoryWithArchiveWasCreated()
        {
            var backupJobSingle = new BackupJob("Single job", _inMemoryRepository, new SingleStoragesAlgorithm(), @"root\output");
            backupJobSingle.AddJobObject(@"root\files\1.txt");
            backupJobSingle.AddJobObject(@"root\files\2.txt");
            RestorePoint restorePoint1 = backupJobSingle.CreateRestorePoint();
            string restorePointPath = backupJobSingle.GetPathOfRestorePoint(restorePoint1);
            
            //"Backup-collection" - default name of zip archive in Single storage mode
            string restoreArchivePath = _inMemoryRepository.JoinPath(restorePointPath, @"\Backup-collection.zip");
            if(!_inMemoryRepository.IsDirectoryExist(restoreArchivePath)) Assert.Fail();
            string[] createdFilesNames = _inMemoryRepository.GetFilesInDirectory(restoreArchivePath);
            foreach (var curName in createdFilesNames)
            {
                string createdFilePath = _inMemoryRepository.JoinPath(restoreArchivePath, curName);
                string originalFilePath = $@"root\files\{curName}";   
                Assert.AreEqual(_inMemoryRepository.ReadFile(createdFilePath), _inMemoryRepository.ReadFile(originalFilePath));
            }
        }
    }
}