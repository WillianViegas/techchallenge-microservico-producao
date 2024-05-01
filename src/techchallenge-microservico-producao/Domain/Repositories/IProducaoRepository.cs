using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.Repositories.Interfaces
{
    public interface IProducaoRepository
    {
        public Task<IList<Pedido>> GetAllPedidos();
        public Task<Pedido> GetPedidoById(string id);
        public Task UpdatePedido(string id, Pedido pedidoInput);

    }
}
