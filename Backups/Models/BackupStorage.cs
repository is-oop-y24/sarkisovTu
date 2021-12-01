using Backups.Repository;

namespace Backups.Models
{
    public class BackupStorage : BackupFile
    {
        public BackupStorage(IRepository repositoryRef, string pathToFile, string content)
            : base(repositoryRef, pathToFile)
        {
            Content = content;
        }

        public string Content { get; private set; }
    }
}