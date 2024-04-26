namespace techchallenge_microservico_producao.DatabaseConfig
{
    public class DatabaseConfig : IDatabaseConfig
    {
        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public string CollectionName { get; set; }
        public string User { get; set; }
        public string Password { get; set; }
    }
}
