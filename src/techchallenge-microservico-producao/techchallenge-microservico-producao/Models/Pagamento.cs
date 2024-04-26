using Domain.Enum;

namespace techchallenge_microservico_producao.Models
{
    public class Pagamento
    {
        public ETipoPagamento? Tipo { get; set; }
        public string? QRCodeUrl { get; set; }
        public string? Bandeira { get; set; }
        public string? OrdemDePagamento { get; set; }
    }
}
