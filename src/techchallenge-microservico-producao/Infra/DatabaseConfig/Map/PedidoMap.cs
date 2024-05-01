using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using techchallenge_microservico_producao.Models;
using System.Reflection.Emit;

namespace techchallenge_microservico_producao.DatabaseConfig.Map
{
    public class PedidoMap : IEntityTypeConfiguration<Pedido>
    {
        public void Configure(EntityTypeBuilder<Pedido> builder)
        {
            builder.HasKey(u => u.Id);

            builder.HasMany(p => p.Produtos)
            .WithOne()
            .HasForeignKey(p => p.PedidoId);
        }
    }
}
