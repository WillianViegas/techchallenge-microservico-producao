using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.DatabaseConfig.Map
{
    public class PagamentoMap : IEntityTypeConfiguration<Pagamento>
    {
        public void Configure(EntityTypeBuilder<Pagamento> builder)
        {
            builder.HasKey(u => u.Id);
        }
    }
}
