namespace ImportReplacement.Api.Repositories.Configurations
{
    public class NpgsqlConnectionConfiguration
    {
        public NpgsqlConnectionConfiguration(string value) => Value = value;
        public string Value { get; }
    }
}
