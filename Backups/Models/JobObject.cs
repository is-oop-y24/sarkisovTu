using System.Text.RegularExpressions;
using Backups.Repository;

namespace Backups.Models
{
    public class JobObject : BackupFile
    {
        public JobObject(IRepository repositoryRef, string pathToFile)
            : base(repositoryRef, pathToFile)
        { }

        public string Content
        {
            get { return RepositoryRef.ReadFile(Path); }
        }
    }
}