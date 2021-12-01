using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using Backups.Models;

namespace Backups.Repository
{
    public class FsRepository : IRepository
    {
        public void CreateArchive(string path, List<BackupStorage> storages)
        {
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    foreach (BackupStorage curStorage in storages)
                    {
                        ZipArchiveEntry newFile = archive.CreateEntry(curStorage.Name);
                        using (Stream entryStream = newFile.Open())
                        using (var streamWriter = new StreamWriter(entryStream))
                        {
                            streamWriter.Write(curStorage.Content);
                        }
                    }
                }

                using (var fileStream = new FileStream(path, FileMode.Create))
                {
                    memoryStream.Seek(0, SeekOrigin.Begin);
                    memoryStream.CopyTo(fileStream);
                }
            }
        }

        public void CreateDirectory(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }

        public void CreateFileInDirectory(string path, string name, string content)
        {
            File.WriteAllText(Path.Combine(path, name), content);
        }

        public string[] GetFilesInDirectory(string path)
        {
            return Directory.GetFiles(@path);
        }

        public bool IsDirectoryExist(string path)
        {
            return Directory.Exists(path);
        }

        public string ReadFile(string path)
        {
            return File.ReadAllText(path);
        }

        public void UpdateFile(string path, string newContent)
        {
            File.WriteAllText(path, newContent);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public string JoinPath(string path1, string path2)
        {
            return Path.Join(path1, path2);
        }
    }
}