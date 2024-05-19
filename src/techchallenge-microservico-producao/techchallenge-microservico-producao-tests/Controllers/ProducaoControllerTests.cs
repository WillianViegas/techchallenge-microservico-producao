using Domain.Enum;
using Microsoft.Extensions.Logging;
using Moq;
using techchallenge_microservico_producao.Controllers;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao_tests.Controllers
{
    public class ProducaoRepositoryTests
    {
        [Fact]
        public void GetAllPedidos()
        {
            //arrange
            var pedido1 = GetPedidoObj();
            var pedido2 = GetPedidoObj();
            var listPedidos = new List<Pedido>();
            listPedidos.Add(pedido1);
            listPedidos.Add(pedido2);

            var pedidoService = new Mock<IProducaoService>().Object;

            Mock.Get(pedidoService)
                .Setup(service => service.GetAllPedidos())
                .ReturnsAsync(listPedidos);

            var mock = new Mock<ILogger<ProducaoController>>();
            ILogger<ProducaoController> logger = mock.Object;

            var controller = new ProducaoController(logger, pedidoService);

            //act
            var result = controller.GetAllPedidos();

            //assert
            Assert.NotNull(result);
        }


        [Fact]
        public void UpdateStatusPedido()
        {
            //arrange
            var pedido1 = GetPedidoObj();

            var producaoService = new Mock<IProducaoService>().Object;


            Mock.Get(producaoService)
                .Setup(service => service.GetPedidoByIdOrigem(pedido1.IdPedidoOrigem))
                .ReturnsAsync(pedido1);

            Mock.Get(producaoService)
                .Setup(service => service.UpdateStatusPedido(pedido1.IdPedidoOrigem, 3, pedido1));

            var mock = new Mock<ILogger<ProducaoController>>();
            ILogger<ProducaoController> logger = mock.Object;

            var controller = new ProducaoController(logger, producaoService);


            //act
            var result = controller.UpdateStatusPedido(pedido1.Id, 3);

            //assert
            Assert.NotNull(result);
        }

        private Pedido GetPedidoObj()
        {
            var produtos = new List<Produto>();
            var produto = new Produto()
            {
                Id = "",
                Nome = "",
                Descricao = "",
                CategoriaId = "",
                Preco = 10.00m
            };

            produtos.Add(produto);


            var pedido = new Pedido();
            pedido.Id = "91E4C318-1025-4273-A40B-7D8940F34575";
            pedido.IdCarrinho = pedido.IdCarrinho;
            pedido.Numero = 1;
            pedido.DataCriacao = DateTime.Now;
            pedido.Produtos = produtos;
            pedido.Status = EPedidoStatus.Novo;
            pedido.IdPedidoOrigem = Guid.NewGuid().ToString();

            return pedido;
        }
    }
}