using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Softplan.Justica.GerenciadorProcessos.Domain.Models;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Extensions;
using Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Mappings;

namespace Softplan.Justica.GerenciadorProcessos.Infra.DataAccess.Context
{
    public class GerenciadorProcessosDbContext : DbContext, IGerenciadorProcessosDbContext
    {
        public GerenciadorProcessosDbContext(DbContextOptions<GerenciadorProcessosDbContext> options) : base(options)
        {
        }

        public DbSet<Processo> Processo { get; set; }

        public DbSet<Responsavel> Responsavel { get; set; }

        public DbSet<SituacaoProcesso> SituacaoProcesso { get; set; }

        public void EnsureDatabase()
        {
            this.Database.EnsureCreated();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly(), t => typeof(IEntityMapping).IsAssignableFrom(t));
            modelBuilder.SeedData();
        }
    }
}