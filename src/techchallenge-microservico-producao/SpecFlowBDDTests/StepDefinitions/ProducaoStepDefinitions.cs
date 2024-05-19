using Domain.Enum;
using Moq;
using NUnit.Framework;
using System;
using techchallenge_microservico_producao.Models;
using techchallenge_microservico_producao.Services.Interfaces;
using TechTalk.SpecFlow;

namespace SpecFlowBDDTests.StepDefinitions
{
    [Binding]
    public class ProducaoStepDefinitions
    {
        private List<Pedido> _pedidos;
        private string _pedidoJson;
        private Pedido _pedidoObj;
        private int _status;

        #region GetAllPedidos
        [Given(@"Que devo exibir todos os pedidos")]
        public void GivenQueDevoExibirTodosOsPedidos()
        {
            _pedidos = new List<Pedido>();
        }

        [When(@"For solicitado buscar todos os pedidos")]
        public void WhenForSolicitado()
        {
            var pedido1 = GetPedidoObj();
            var pedido2 = GetPedidoObj();
            _pedidos.Add(pedido1);
            _pedidos.Add(pedido2);
        }

        [Then(@"Retorno uma lista com os pedidos")]
        public void ThenRetornoUmaListaComOsPedidos()
        {
            var pedidoService = new Mock<IProducaoService>().Object;

            Mock.Get(pedidoService)
                .Setup(service => service.GetAllPedidos())
                .ReturnsAsync(_pedidos);

            var result = pedidoService.GetAllPedidos();

            Assert.NotNull(result);
        }
        #endregion

        #region UpdateStatusPedido
        [Given(@"Que preciso atualizar o status de um pedido")]
        public void GivenQuePrecisoAtualizarOStatusDeUmPedido()
        {
            _pedidoJson = "{\r\n    \"Produtos\": [\r\n        {\r\n            \"id\": \"65a315a4db1f522d916d935a\",\r\n            \"nome\": \"Hamburguer especial da casa\",\r\n            \"descricao\": \"Hamburguer artesanal da casa com maionese caseira e molho secreto\",\r\n            \"preco\": 35.99,\r\n            \"categoriaId\": \"65a315a4db1f522d916d9357\"\r\n        }\r\n    ],\r\n    \"Usuario\": {\r\n        \"id\": \"65a315a4db1f522d916d9355\",\r\n        \"nome\": \"Marcos\",\r\n        \"email\": \"marcao@gmail.com\",\r\n        \"cpf\": \"65139370000\"\r\n    },\r\n    \"Total\": 35.99,\r\n    \"Pagamento\":{}\r\n}";
        }

        [When(@"Receber um novo status e o pedido para ser atualizado")]
        public void WhenReceberUmNovoStatusEOPedidoParaSerAtualizado()
        {
            var pedido = Newtonsoft.Json.JsonConvert.DeserializeObject<Pedido>(_pedidoJson);
            _pedidoObj = pedido;
            _status = 2;
        }

        [Then(@"Atualizo o status do pedido")]
        public void ThenAtualizoOStatusDoPedido()
        {
            var pedidoService = new Mock<IProducaoService>().Object;

            Mock.Get(pedidoService)
                .Setup(service => service.UpdateStatusPedido(_pedidoObj.Id, _status, _pedidoObj));

            //act
            var result = pedidoService.UpdateStatusPedido(_pedidoObj.Id, _status, _pedidoObj);

            //assert
            Assert.NotNull(result);
        }
        #endregion

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
