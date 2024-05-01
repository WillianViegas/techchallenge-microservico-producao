using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using techchallenge_microservico_producao.DatabaseConfig;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;

namespace techchallenge_microservico_producao.Repositories
{
    public class ProducaoRepository : IProducaoRepository
    {
        private readonly IMongoCollection<Pedido> _collection;
        private readonly EFDbconfig _efDbContext;


        public ProducaoRepository(EFDbconfig efDbContext)
        {
            _efDbContext = efDbContext;
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            return  _efDbContext.Pedidos
             .Include(pedido => pedido.Produtos) 
             .Include(pedido => pedido.Usuario)  
             .Include(pedido => pedido.Pagamento)  
             .ToList();
        }
    }
}
