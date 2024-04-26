namespace techchallenge_microservico_producao.DatabaseConfig
{
    public interface IDatabaseConfig
    {
        string DatabaseName { get; set; }
        string ConnectionString { get; set; }
        string CollectionName { get; set; }
        string User { get; set; }
        string Password { get; set; }
    }
}
