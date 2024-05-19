using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.Services.Interfaces
{
    public interface IProducaoService
    {
        public Task<IList<Pedido>> GetAllPedidos();
        public Task<Pedido> GetPedidoByIdOrigem(string id);
        public Task UpdateStatusPedido(string id, int status, Pedido pedido);
        public Task RegistrarPedidos();
    }
}
