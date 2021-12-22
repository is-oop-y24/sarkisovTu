namespace BackupsExtra.JsonSchema
{
    public class JobObjectSchema
    {
        public JobObjectSchema()
        {
        }

        public JobObjectSchema(string path)
        {
            Path = path;
        }

        public string Path { get; set; }
    }
}