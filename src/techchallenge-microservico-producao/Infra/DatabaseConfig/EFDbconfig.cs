using Microsoft.EntityFrameworkCore;
using System.Reflection;
using techchallenge_microservico_producao.Models;
using Domain.Entities;

namespace Infra.DatabaseConfig
{
    public class EFDbconfig : DbContext
    {
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }
        public DbSet<Categoria> Categorias { get; set; }

        public EFDbconfig(DbContextOptions<EFDbconfig> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Pedido>()
            .Property(p => p.Total)
            .HasColumnType("decimal(18,2)");

            modelBuilder.Entity<Produto>()
            .Property(p => p.Preco)
            .HasColumnType("decimal(18,2)");

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
