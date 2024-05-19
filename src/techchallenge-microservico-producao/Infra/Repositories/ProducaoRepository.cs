using Microsoft.EntityFrameworkCore;
using Infra.DatabaseConfig;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;

namespace techchallenge_microservico_producao.Repositories
{
    public class ProducaoRepository : IProducaoRepository
    {
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

        public async Task<Pedido> GetPedidoByIdOrigem(string id)
        {
            return _efDbContext.Pedidos
           .Include(pedido => pedido.Produtos)
           .Include(pedido => pedido.Usuario)
           .Include(pedido => pedido.Pagamento).FirstOrDefault(x => x.IdPedidoOrigem == id);
        }

        public async Task UpdatePedido(string id, Pedido pedidoInput)
        {
            _efDbContext.SaveChanges();
        }

        public async Task<Pedido> CreatePedido(Pedido pedido)
        {
            _efDbContext.Pedidos.Add(pedido);
            _efDbContext.SaveChanges();
            return pedido;
        }
    }
}
