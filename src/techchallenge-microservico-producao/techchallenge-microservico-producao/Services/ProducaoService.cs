using Domain.Enum;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao.Services
{
    public class ProducaoService : IProducaoService
    {
        private readonly IProducaoRepository _pedidoRepository;
        private readonly ILogger _log;

        public ProducaoService(IProducaoRepository producaoRepository,  ILogger<Pedido> log)
        {
            _pedidoRepository = producaoRepository;
            _log = log;
        }

        public async Task<IList<Pedido>> GetAllPedidos()
        {
            try
            {
                var listaPedidos = await _pedidoRepository.GetAllPedidos();
                var listaPedidosFiltrados = listaPedidos.Where(x => x.Status == EPedidoStatus.EmPreparo || x.Status == EPedidoStatus.Pronto || x.Status == EPedidoStatus.Recebido)
                    .OrderBy(n => n.Status == EPedidoStatus.Recebido)
                    .ThenBy(n => n.Status == EPedidoStatus.EmPreparo)
                    .ThenBy(n => n.Status == EPedidoStatus.Pronto).ToList();

                return listaPedidosFiltrados;
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task<Pedido> GetPedidoById(string id)
        {
            try
            {
                return await _pedidoRepository.GetPedidoById(id);
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }

        public async Task UpdateStatusPedido(string id, int status, Pedido pedido)
        {
            try
            {
                pedido.Status = (EPedidoStatus)status;
                await _pedidoRepository.UpdatePedido(id, pedido);
                _log.LogInformation($"Pedido atualizado id: {id}, status: {pedido.Status.ToString()}");
            }
            catch (Exception ex)
            {
                _log.LogError(ex.Message);
                throw new Exception(ex.Message);
            }
        }
    }
}
