using System.Collections.Generic;
using System.Linq;
using Backups.Tools;

namespace Backups.Models
{
    public class EmulatedFolder
    {
        public EmulatedFolder(string folderName)
        {
            FolderName = folderName;
            ChildFolders = new List<EmulatedFolder>();
            Files = new Dictionary<string, string>();
        }

        public string FolderName { get; private set; }
        public List<EmulatedFolder> ChildFolders { get; private set; }
        public Dictionary<string, string> Files { get; private set; }

        public bool HasFolder(string name)
        {
            if (ChildFolders.Find(folder => folder.FolderName == name) != null) return true;
            return false;
        }

        public EmulatedFolder GetFolder(string name)
        {
            return ChildFolders.Find(folder => folder.FolderName == name);
        }

        public void CreateFolder(string name)
        {
            if (ChildFolders.Find(folder => folder.FolderName == name) == null)
            {
                ChildFolders.Add(new EmulatedFolder(name));
            }
        }

        public void CreateFile(string name, string content)
        {
            if (!Files.ContainsKey(name))
            {
                Files.Add(name, content);
            }
        }

        public string[] GetFiles()
        {
            return Files.Keys.ToArray();
        }

        public string ReadFile(string name)
        {
            if (!Files.ContainsKey(name)) throw new BackupsException("Unable to read specified file");
            return Files[name];
        }

        public void UpdateFile(string name, string content)
        {
            if (!Files.ContainsKey(name)) throw new BackupsException("Unable to read specified file");
            Files[name] = content;
        }

        public void DeleteFile(string name)
        {
            if (!Files.ContainsKey(name)) throw new BackupsException("Unable to find specified file");
            Files.Remove(name);
        }

        public void DeleteFolder(string name)
        {
            EmulatedFolder queryFolder = ChildFolders.Find(folder => folder.FolderName == name);
            ChildFolders.Remove(queryFolder);
        }
    }
}