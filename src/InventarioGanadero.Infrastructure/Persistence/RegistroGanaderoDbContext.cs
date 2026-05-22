using InventarioGanadero.Infrastructure.Entities;
using Microsoft.EntityFrameworkCore;

namespace InventarioGanadero.Infrastructure.Persistence;

public class RegistroGanaderoDbContext(DbContextOptions<RegistroGanaderoDbContext> options) : DbContext(options)
{
    public DbSet<Propietario> Propietarios => Set<Propietario>();
    public DbSet<Rol> Roles => Set<Rol>();
    public DbSet<Usuario> Usuarios => Set<Usuario>();
    public DbSet<Lote> Lotes => Set<Lote>();
    public DbSet<Raza> Razas => Set<Raza>();
    public DbSet<Color> Colores => Set<Color>();
    public DbSet<TipoAnimal> TiposAnimales => Set<TipoAnimal>();
    public DbSet<EstadoAnimal> EstadosAnimal => Set<EstadoAnimal>();
    public DbSet<Animal> Animales => Set<Animal>();
    public DbSet<MovimientoAnimal> MovimientosAnimal => Set<MovimientoAnimal>();
    public DbSet<Pesaje> Pesajes => Set<Pesaje>();
    public DbSet<Vacunacion> Vacunaciones => Set<Vacunacion>();
    public DbSet<TratamientoVeterinario> TratamientosVeterinarios => Set<TratamientoVeterinario>();
    public DbSet<Compra> Compras => Set<Compra>();
    public DbSet<Venta> Ventas => Set<Venta>();
    public DbSet<Muerte> Muertes => Set<Muerte>();
    public DbSet<Parto> Partos => Set<Parto>();
    public DbSet<RelacionMadreCria> RelacionesMadreCria => Set<RelacionMadreCria>();
    public DbSet<Palpacion> Palpaciones => Set<Palpacion>();
    public DbSet<TareaGanadera> TareasGanaderas => Set<TareaGanadera>();
    public DbSet<DetalleTareaGanadera> DetalleTareasGanaderas => Set<DetalleTareaGanadera>();
    public DbSet<AuditoriaCambio> AuditoriaCambios => Set<AuditoriaCambio>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Propietario>(e =>
        {
            e.ToTable("Propietarios");
            e.HasKey(x => x.IdPropietario);
            e.Property(x => x.IdPropietario).HasColumnName("IdPropietario");
        });

        modelBuilder.Entity<Rol>(e =>
        {
            e.ToTable("Roles");
            e.HasKey(x => x.IdRol);
        });

        modelBuilder.Entity<Usuario>(e =>
        {
            e.ToTable("Usuarios");
            e.HasKey(x => x.IdUsuario);
            e.Property(x => x.UsuarioLogin).HasColumnName("Usuario");
            e.Property(x => x.PasswordHash).HasMaxLength(255);
            e.Property(x => x.LegacyClave).HasColumnName("Clave").HasMaxLength(200);
            e.HasOne(x => x.Rol).WithMany(r => r.Usuarios).HasForeignKey(x => x.IdRol);
        });

        modelBuilder.Entity<Lote>(e => { e.ToTable("Lotes"); e.HasKey(x => x.IdLote); });
        modelBuilder.Entity<Raza>(e => { e.ToTable("Razas"); e.HasKey(x => x.IdRaza); });
        modelBuilder.Entity<Color>(e => { e.ToTable("Colores"); e.HasKey(x => x.IdColor); });
        modelBuilder.Entity<TipoAnimal>(e => { e.ToTable("TiposAnimales"); e.HasKey(x => x.IdTipoAnimal); });
        modelBuilder.Entity<EstadoAnimal>(e => { e.ToTable("EstadosAnimal"); e.HasKey(x => x.IdEstadoAnimal); });

        modelBuilder.Entity<Animal>(e =>
        {
            e.ToTable("Animales");
            e.HasKey(x => new { x.Numero, x.Anio });
            e.HasOne(x => x.Propietario).WithMany(p => p.Animales).HasForeignKey(x => x.IdPropietario);
            e.HasOne(x => x.TipoAnimal).WithMany().HasForeignKey(x => x.IdTipoAnimal);
            e.HasOne(x => x.EstadoAnimal).WithMany().HasForeignKey(x => x.IdEstadoAnimal);
            e.HasOne(x => x.Lote).WithMany().HasForeignKey(x => x.IdLote);
            e.HasOne(x => x.Raza).WithMany().HasForeignKey(x => x.IdRaza);
            e.HasOne(x => x.Color).WithMany().HasForeignKey(x => x.IdColor);
        });

        modelBuilder.Entity<MovimientoAnimal>(e =>
        {
            e.ToTable("MovimientosAnimal");
            e.HasKey(x => x.IdMovimiento);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
            e.HasOne(x => x.LoteOrigen).WithMany().HasForeignKey(x => x.IdLoteOrigen);
            e.HasOne(x => x.LoteDestino).WithMany().HasForeignKey(x => x.IdLoteDestino);
        });

        modelBuilder.Entity<Pesaje>(e =>
        {
            e.ToTable("Pesajes");
            e.HasKey(x => x.IdPesaje);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<Vacunacion>(e =>
        {
            e.ToTable("Vacunaciones");
            e.HasKey(x => x.IdVacunacion);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
            e.HasIndex(x => new { x.Numero, x.Anio, x.FechaVacunacion, x.NombreVacuna }).IsUnique();
        });

        modelBuilder.Entity<TratamientoVeterinario>(e =>
        {
            e.ToTable("TratamientosVeterinarios");
            e.HasKey(x => x.IdTratamiento);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<Compra>(e =>
        {
            e.ToTable("Compras");
            e.HasKey(x => x.IdCompra);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<Venta>(e =>
        {
            e.ToTable("Ventas");
            e.HasKey(x => x.IdVenta);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<Muerte>(e =>
        {
            e.ToTable("Muertes");
            e.HasKey(x => x.IdMuerte);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<Parto>(e =>
        {
            e.ToTable("Partos");
            e.HasKey(x => x.IdParto);
            e.Property(x => x.TipoParto).HasMaxLength(50);
            e.Property(x => x.EstadoParto).HasMaxLength(50);
            e.HasOne(x => x.Madre).WithMany().HasForeignKey(x => new { x.NumeroMadre, x.AnioMadre });
            e.HasMany(x => x.RelacionesCrias).WithOne(r => r.Parto).HasForeignKey(r => r.IdParto);
        });

        modelBuilder.Entity<RelacionMadreCria>(e =>
        {
            e.ToTable("RelacionMadreCria");
            e.HasKey(x => x.IdRelacion);
            e.HasOne(x => x.Cria).WithMany().HasForeignKey(x => new { x.NumeroCria, x.AnioCria });
        });

        modelBuilder.Entity<Palpacion>(e =>
        {
            e.ToTable("Palpaciones");
            e.HasKey(x => x.IdPalpacion);
            e.Property(x => x.Resultado).HasMaxLength(30);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<TareaGanadera>(e =>
        {
            e.ToTable("TareasGanaderas");
            e.HasKey(x => x.IdTarea);
            e.Property(x => x.TipoTarea).HasMaxLength(30);
            e.HasOne(x => x.Lote).WithMany().HasForeignKey(x => x.IdLote);
            e.HasOne(x => x.Propietario).WithMany().HasForeignKey(x => x.IdPropietario);
            e.HasOne(x => x.LoteDestino).WithMany().HasForeignKey(x => x.IdLoteDestino);
            e.HasMany(x => x.Detalles).WithOne(d => d.Tarea).HasForeignKey(d => d.IdTarea);
        });

        modelBuilder.Entity<DetalleTareaGanadera>(e =>
        {
            e.ToTable("DetalleTareaGanadera");
            e.HasKey(x => x.IdDetalleTarea);
            e.Property(x => x.EstadoRegistro).HasMaxLength(30);
            e.HasOne(x => x.Animal).WithMany().HasForeignKey(x => new { x.Numero, x.Anio });
        });

        modelBuilder.Entity<AuditoriaCambio>(e =>
        {
            e.ToTable("AuditoriaCambios");
            e.HasKey(x => x.IdAuditoria);
        });
    }
}
