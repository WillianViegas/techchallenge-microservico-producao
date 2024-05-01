using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ProducaoController : ControllerBase
    {
        private readonly ILogger<ProducaoController> _logger;
        private IProducaoService _producaoService;

        public ProducaoController(ILogger<ProducaoController> logger, IProducaoService producaoService)
        {
            _logger = logger;
            _producaoService = producaoService;
        }


        [HttpGet("/teste")]
        public IResult Teste()
        {
            try
            {
                return TypedResults.Ok("Teste");
            }
            catch (Exception ex)
            {
                return TypedResults.Problem(ex.Message);
            }
        }


        [HttpGet("/getAllPedidos")]
        [ProducesResponseType(typeof(IEnumerable<Pedido>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
        Summary = "Obter todos os pedidos",
        Description = "Retorna uma lista de todos os pedidos")]
        public async Task<IResult> GetAllPedidos()
        {
            try
            {
                var pedidos = await _producaoService.GetAllPedidos();
                return TypedResults.Ok(pedidos);
            }
            catch (Exception ex)
            {
                var erro = $"Erro ao obter os pedidos ativos.";
                _logger.LogError(erro, ex);
                return TypedResults.Problem(erro);
            }
        }


        [HttpPost("/updateStatusPedido")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        [SwaggerOperation(
        Summary = "Atualizar status pedido",
        Description = "Atualiza o status do pedido"
    )]
        public async Task<IResult> UpdateStatusPedido(string id, int status)
        {
            try
            {
                if (string.IsNullOrEmpty(id))
                    return TypedResults.BadRequest("Erro: Id inválido.");

                if (status < 3 || status > 5)
                    return TypedResults.BadRequest("Erro: Status inválido para atualização do pedido.");

                var pedido = await _producaoService.GetPedidoById(id);
                if (pedido is null || string.IsNullOrEmpty(pedido.Id)) return TypedResults.NotFound("Pedido não encontrado.");

                if ((int)pedido.Status >= status)
                    return TypedResults.BadRequest("Erro: O Status atual do pedido é igual ou superior ao solicitado para atualização.");

                await _producaoService.UpdateStatusPedido(id, status, pedido);
                return TypedResults.NoContent();
            }
            catch (Exception ex)
            {
                var erro = $"Erro ao atualizar o status do pedido. Id: {id}";
                _logger.LogError(erro, ex);
                return TypedResults.Problem(erro);
            }
        }
    }
}


