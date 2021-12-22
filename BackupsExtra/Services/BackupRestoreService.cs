using Backups.Models;
using Backups.Repository;

namespace BackupsExtra.Services
{
    public class BackupRestoreService
    {
        public void UpBackToOriginalLocation(RestorePoint restorePoint)
        {
            IRepository repositorySystem = restorePoint.Storages[0].RepositoryRef;
            restorePoint.Storages.ForEach(storage =>
            {
                string storageOriginalPath = storage.Path;
                if (repositorySystem.IsFileExist(storageOriginalPath))
                {
                    repositorySystem.UpdateFile(storageOriginalPath, storage.Content);
                }
                else
                {
                    string pathToReplace = storageOriginalPath.Replace($@"\{storage.Name}", string.Empty);
                    repositorySystem.CreateFileInDirectory(pathToReplace, storage.Name, storage.Content);
                }
            });
        }

        public void UpBackToDifferentLocation(RestorePoint restorePoint, string path)
        {
            IRepository repositorySystem = restorePoint.Storages[0].RepositoryRef;
            restorePoint.Storages.ForEach(storage =>
            {
                repositorySystem.CreateFileInDirectory(path, storage.Name, storage.Content);
            });
        }
    }
}