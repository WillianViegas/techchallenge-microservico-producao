using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.Services.Interfaces
{
    public interface IProducaoService
    {
        public Task<IList<Pedido>> GetAllPedidos();
    }
}
