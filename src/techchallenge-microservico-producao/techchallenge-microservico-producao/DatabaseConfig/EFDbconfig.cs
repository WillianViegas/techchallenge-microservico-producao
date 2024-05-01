using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Reflection;
using System;
using techchallenge_microservico_producao.Models;

namespace techchallenge_microservico_producao.DatabaseConfig
{
    public class EFDbconfig : DbContext
    {
        public DbSet<Pedido> Pedidos { get; set; }
        public DbSet<Pagamento> Pagamentos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        public DbSet<Produto> Produtos { get; set; }

        public EFDbconfig(DbContextOptions<EFDbconfig> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
