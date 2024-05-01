using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.DatabaseConfig.Map
{
    public class UsuarioMap : IEntityTypeConfiguration<Produto>
    {
        public void Configure(EntityTypeBuilder<Produto> builder)
        {
            builder.HasKey(u => u.Id);
        }

    }
}
