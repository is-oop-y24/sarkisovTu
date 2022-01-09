using Backups.Models;
using Backups.Repository;

namespace BackupsExtra.Models
{
    public class DifferentLocationAlgorithm : IRestoreAlgorithm
    {
        public void Restore(RestorePoint restorePoint, string path)
        {
            IRepository repositorySystem = restorePoint.Storages[0].RepositoryRef;
            restorePoint.Storages.ForEach(storage =>
            {
                repositorySystem.CreateFileInDirectory(path, storage.Name, storage.Content);
            });
        }
    }
}