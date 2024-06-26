﻿using Domain.Enum;

namespace techchallenge_microservico_producao.Models
{
    public class Pedido
    {
        public string Id { get; set; }
        public int Numero { get; set; }
        public List<Produto> Produtos { get; set; }
        public Usuario Usuario { get; set; }
        public decimal Total { get; set; }
        public EPedidoStatus Status { get; set; }
        public DateTime DataCriacao { get; set; }
        public string? IdCarrinho { get; set; }
        public Pagamento? Pagamento { get; set; }
        public string IdPedidoOrigem { get; set; }
    }
}
