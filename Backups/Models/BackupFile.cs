using System.Linq;
using System.Text.RegularExpressions;
using Backups.Repository;

namespace Backups.Models
{
    public class BackupFile
    {
        protected BackupFile(IRepository repositoryRef, string path)
        {
            RepositoryRef = repositoryRef;
            Path = path;
        }

        public IRepository RepositoryRef { get; private set; }
        public string Path { get; private set; }

        public string Name
        {
            get { return GetFileNameFromPath(Path); }
        }

        public string GetFileNameFromPath(string path)
        {
            const string fileRegexPattern = @"\\[A-Za-z0-9]+\.[a-z]+";
            var fileRegex = new Regex(fileRegexPattern);
            return fileRegex.Matches(path).Last().ToString().Substring(1);
        }
    }
}