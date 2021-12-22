using System.Collections.Generic;

namespace BackupsExtra.JsonSchema
{
    public class StorageSchema
    {
        public StorageSchema()
        {
        }

        public StorageSchema(string path, string content)
        {
            Path = path;
            Content = content;
        }

        public string Path { get; set; }
        public string Content { get; set; }
    }
}