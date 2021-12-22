using System;
using System.Collections.Generic;
using Backups.Models;
using Backups.Tools;

namespace Backups.Repository
{
    public class InMemoryRepository : IRepository
    {
        public InMemoryRepository()
        {
            RootFolder = new EmulatedFolder("root");
        }

        public EmulatedFolder RootFolder { get; private set; }

        public void CreateArchive(string path, List<BackupStorage> storages)
        {
            CreateDirectory(path);
            foreach (BackupStorage curStorage in storages)
            {
                CreateFileInDirectory(path, curStorage.Name, curStorage.Content);
            }
        }

        public void CreateDirectory(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i]))
                    {
                        curFolder.CreateFolder(separatedPath[i]);
                    }

                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }
        }

        public void CreateFileInDirectory(string path, string name, string content)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length; i++)
                {
                    if (curFolder.HasFolder(separatedPath[i]))
                    {
                        curFolder = curFolder.GetFolder(separatedPath[i]);
                    }
                    else
                    {
                        curFolder.CreateFolder(separatedPath[i]);
                        curFolder = curFolder.GetFolder(separatedPath[i]);
                    }
                }

                curFolder.CreateFile(name, content);
            }
        }

        public bool IsDirectoryExist(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) return false;
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            return true;
        }

        public bool IsFileExist(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length - 1; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) return false;
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            return Array.Find(curFolder.GetFiles(), fileName => fileName == separatedPath[^1]) != null;
        }

        public string[] GetFilesInDirectory(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) return Array.Empty<string>();
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            return curFolder.GetFiles();
        }

        public string ReadFile(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length - 1; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) return string.Empty;
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            return curFolder.ReadFile(separatedPath[^1]);
        }

        public void UpdateFile(string path, string newContent)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length - 1; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) throw new BackupsException("Unable to find specified file");
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            curFolder.UpdateFile(separatedPath[^1], newContent);
        }

        public void DeleteFile(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length - 1; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) throw new BackupsException("Unable to find specified file");
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            curFolder.DeleteFile(separatedPath[^1]);
        }

        public void DeleteFolder(string path)
        {
            string[] separatedPath = path.Split("\\");
            EmulatedFolder curFolder = RootFolder;
            if (separatedPath[0] == "root")
            {
                for (int i = 1; i < separatedPath.Length - 1; i++)
                {
                    if (!curFolder.HasFolder(separatedPath[i])) throw new BackupsException("Unable to find specified file");
                    curFolder = curFolder.GetFolder(separatedPath[i]);
                }
            }

            curFolder.DeleteFolder(separatedPath[^1]);
        }

        public string JoinPath(string path1, string path2)
        {
            if (path1[^1] != @"\"[0] && path2[0] != @"\"[0]) path1 += @"\";
            return path1 + path2;
        }
    }
}