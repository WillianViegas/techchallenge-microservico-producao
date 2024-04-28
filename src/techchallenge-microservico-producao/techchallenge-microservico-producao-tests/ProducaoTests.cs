using Domain.Enum;
using Moq;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao_tests
{
    public class ProducaoTests
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

            //act
            var result = pedidoService.GetAllPedidos();

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
            pedido.Id = "";
            pedido.IdCarrinho = pedido.IdCarrinho;
            pedido.Numero = 1;
            pedido.DataCriacao = DateTime.Now;
            pedido.Produtos = produtos;
            pedido.Status = EPedidoStatus.Novo;

            return pedido;
        }
    }
}