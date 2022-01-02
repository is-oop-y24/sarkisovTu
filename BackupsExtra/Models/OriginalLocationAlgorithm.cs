using Backups.Models;
using Backups.Repository;

namespace BackupsExtra.Models
{
    public class OriginalLocationAlgorithm : IRestoreAlgorithm
    {
        public void Restore(RestorePoint restorePoint, string path = "")
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
    }
}