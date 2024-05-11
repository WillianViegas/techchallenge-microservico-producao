using Domain.Enum;
using Moq;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Repositories.Interfaces;
using techchallenge_microservico_producao.Services.Interfaces;

namespace techchallenge_microservico_producao_tests.Repositories
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

            var producaoRepository = new Mock<IProducaoRepository>().Object;

            Mock.Get(producaoRepository)
                .Setup(repo => repo.GetAllPedidos())
                .ReturnsAsync(listPedidos);

            //act
            var result = producaoRepository.GetAllPedidos();

            //assert
            Assert.NotNull(result);
        }


        [Fact]
        public void GetPedidoById()
        {
            //arrange
            var pedido1 = GetPedidoObj();

            var producaoRepository = new Mock<IProducaoRepository>().Object;

            Mock.Get(producaoRepository)
                .Setup(repo => repo.GetPedidoById(pedido1.Id))
                .ReturnsAsync(pedido1);

            //act
            var result = producaoRepository.GetPedidoById(pedido1.Id);

            //assert
            Assert.NotNull(result);
        }

        [Fact]
        public void UpdateStatusPedido()
        {
            //arrange
            var pedido1 = GetPedidoObj();

            var producaoRepository = new Mock<IProducaoRepository>().Object;

            Mock.Get(producaoRepository)
                .Setup(repo => repo.UpdatePedido(pedido1.Id, pedido1));

            //act
            var result = producaoRepository.UpdatePedido(pedido1.Id,  pedido1);

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

            return pedido;
        }
    }
}