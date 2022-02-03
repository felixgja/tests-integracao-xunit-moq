using Alura.CoisasAFazer.Core.Models;
using Microsoft.EntityFrameworkCore;

namespace Alura.CoisasAFazer.Infrastructure
{
    public class DbTarefasContext : DbContext
    {
        public DbTarefasContext(DbContextOptions options) : base(options)
        {
        }

        public DbTarefasContext()
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (optionsBuilder.IsConfigured) return;
            
            var conexao = "server=localhost;userid=challenge;password=challenge;database=DbTarefas";
            var versao = MySqlServerVersion.AutoDetect(conexao);
            optionsBuilder.UseMySql(conexao, versao);
        }

        public DbSet<Tarefa> Tarefas { get; set; }
        public DbSet<Categoria> Categorias { get; set; }
    }
}
