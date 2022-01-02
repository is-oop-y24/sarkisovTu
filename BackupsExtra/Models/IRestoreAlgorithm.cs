using Backups.Models;

namespace BackupsExtra.Models
{
    public interface IRestoreAlgorithm
    {
        void Restore(RestorePoint restorePoint, string path);
    }
}