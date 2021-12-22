using System.Collections.Generic;
using Backups.Models;

namespace Backups.Repository
{
    public interface IRepository
    {
        void CreateArchive(string path, List<BackupStorage> storages);
        void CreateDirectory(string path);
        void CreateFileInDirectory(string path, string name, string content);

        bool IsDirectoryExist(string path);

        bool IsFileExist(string path);
        string[] GetFilesInDirectory(string path);

        string ReadFile(string path);

        void UpdateFile(string path, string newContent);

        void DeleteFile(string path);

        void DeleteFolder(string path);

        string JoinPath(string path1, string path2);
    }
}