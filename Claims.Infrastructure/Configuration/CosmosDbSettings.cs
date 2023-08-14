namespace Claims.Infrastructure.Configuration
{
    public class CosmosDbSettings
    {
        public string Account { get; set; }
        public string Key { get; set; }
        public string DatabaseName { get; set; }
        public Dictionary<string, string> Containers { get; set; }
    }
}
