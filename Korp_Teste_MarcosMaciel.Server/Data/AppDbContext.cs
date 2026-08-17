using Korp_Teste_MarcosMaciel.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace Korp_Teste_MarcosMaciel.Server.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Product> Products => Set<Product>();
    public DbSet<NotaFiscal> NotasFiscais => Set<NotaFiscal>();
    public DbSet<NotaFiscalItem> NotaFiscalItems => Set<NotaFiscalItem>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Product>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Codigo).IsRequired().HasMaxLength(50);
            entity.Property(x => x.Descricao).IsRequired().HasMaxLength(200);
            entity.Property(x => x.Saldo).IsRequired();
            entity.HasIndex(x => x.Codigo).IsUnique();
        });

        modelBuilder.Entity<NotaFiscal>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.Numero).IsRequired();
            entity.Property(x => x.Status).IsRequired().HasMaxLength(20);
            entity.HasIndex(x => x.Numero).IsUnique();
            entity.HasMany(x => x.Itens)
                .WithOne()
                .HasForeignKey(x => x.NotaFiscalId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<NotaFiscalItem>(entity =>
        {
            entity.HasKey(x => x.Id);
            entity.Property(x => x.ProdutoId).IsRequired();
            entity.Property(x => x.Quantidade).IsRequired();
            entity.HasOne(x => x.Produto)
                .WithMany()
                .HasForeignKey(x => x.ProdutoId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
