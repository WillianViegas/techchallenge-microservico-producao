using Microsoft.AspNetCore.Mvc;
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


        //Transformar em GET dos pedidos em producao
        [HttpGet("/getAllPedidos")]
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
    }


}
