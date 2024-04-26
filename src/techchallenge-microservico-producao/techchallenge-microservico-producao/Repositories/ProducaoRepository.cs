using MongoDB.Driver;
using techchallenge_microservico_producao.DatabaseConfig;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;

namespace techchallenge_microservico_producao.Repositories
{
    public class ProducaoRepository : IProducaoRepository
    {
        private readonly IMongoCollection<Pedido> _collection;

        public ProducaoRepository(IDatabaseConfig databaseConfig)
        {
            var connectionString = databaseConfig.ConnectionString.Replace("user", databaseConfig.User).Replace("password", databaseConfig.Password);
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseConfig.DatabaseName);
            _collection = database.GetCollection<Pedido>("Pedido");
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            return await _collection.Find(_ => true).ToListAsync();
        }
    }
}
