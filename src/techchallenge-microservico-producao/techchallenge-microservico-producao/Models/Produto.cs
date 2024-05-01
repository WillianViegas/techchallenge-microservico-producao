using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Bson;
using System.ComponentModel.DataAnnotations.Schema;

namespace techchallenge_microservico_producao.Models
{
    public class Produto
    {
        [ForeignKey("Pedido")]
        public string PedidoId { get; set; } = null;
        public string Id { get; set; }
        public string? Nome { get; set; } = null;
        public string? Descricao { get; set; } = null;
        public decimal Preco { get; set; }
        public string? CategoriaId { get; set; } = null;
    }
}
