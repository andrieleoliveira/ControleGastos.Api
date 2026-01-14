using ControleGastos.Api.Domain.Entities;
using ControleGastos.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace ControleGastos.Api.Infrastructure.Data;

/// <summary>
/// DbContext da aplicação.
/// Responsável por mapear as entidades do domínio para o banco (SQLite)
/// e configurar regras de relacionamento (ex.: cascade delete).
/// </summary>
public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Categoria> Categorias => Set<Categoria>();
    public DbSet<Transacao> Transacoes => Set<Transacao>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Pessoa -> Transações (1:N)
        // Regra de negocio: ao deletar uma pessoa, apagar todas as transações dela.
        modelBuilder.Entity<Pessoa>()
            .HasMany(p => p.Transacoes)
            .WithOne(t => t.Pessoa!)
            .HasForeignKey(t => t.PessoaId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ajuste de precisão do decimal para o SQLite (evita valores “quebrados”)
        modelBuilder.Entity<Transacao>()
            .Property(t => t.Valor)
            .HasPrecision(18, 2);

        // Regras básicas de consistência
        modelBuilder.Entity<Pessoa>()
            .Property(p => p.Nome)
            .HasMaxLength(120)
            .IsRequired();

        modelBuilder.Entity<Categoria>()
            .Property(c => c.Descricao)
            .HasMaxLength(120)
            .IsRequired();
    }
}
