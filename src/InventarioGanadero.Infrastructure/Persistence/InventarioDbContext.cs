using InventarioGanadero.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Infrastructure.Persistence;

public class InventarioDbContext(DbContextOptions<InventarioDbContext> options) : DbContext(options)
{
    public DbSet<Ciclo12026> Ciclo12026 => Set<Ciclo12026>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Ciclo12026>(entity =>
        {
            entity.ToTable("ciclo_1_2026", "dbo");
            entity.HasKey(e => e.Id);
        });
    }
}
