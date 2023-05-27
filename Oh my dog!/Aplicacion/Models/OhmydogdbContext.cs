using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace Aplicacion.Models;

public partial class OhmydogdbContext : DbContext
{
    public OhmydogdbContext()
    {
    }

    public OhmydogdbContext(DbContextOptions<OhmydogdbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Cuidadore> Cuidadores { get; set; }

    public virtual DbSet<Descuento> Descuentos { get; set; }

    public virtual DbSet<EstadoTurno> EstadoTurnos { get; set; }

    public virtual DbSet<Paseadore> Paseadores { get; set; }

    public virtual DbSet<Perro> Perros { get; set; }

    public virtual DbSet<Publicacion> Publicacions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoPublicacion> TipoPublicacions { get; set; }

    public virtual DbSet<Tratamiento> Tratamientos { get; set; }

    public virtual DbSet<TratamientoPerro> TratamientoPerros { get; set; }

    public virtual DbSet<Turno> Turnos { get; set; }

    public virtual DbSet<Usuario> Usuarios { get; set; }

    public virtual DbSet<UsuarioAdopcionPublicacion> UsuarioAdopcionPublicacions { get; set; }

    public virtual DbSet<UsuarioColectaPublicacion> UsuarioColectaPublicacions { get; set; }

    public virtual DbSet<UsuarioPerdidaPublicacion> UsuarioPerdidaPublicacions { get; set; }

    public virtual DbSet<Vacuna> Vacunas { get; set; }

    public virtual DbSet<VacunaPerro> VacunaPerros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=localhost; database=ohmydogdb;trustservercertificate=true; integrated security=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Cuidadore>(entity =>
        {
            entity.HasIndex(e => new { e.Ubicacion, e.Email }, "IX_Cuidadores");

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Foto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HorarioIn)
                .HasPrecision(0)
                .HasColumnName("HorarioIN");
            entity.Property(e => e.HorarioOut)
                .HasPrecision(0)
                .HasColumnName("HorarioOUT");
            entity.Property(e => e.Latitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Longitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Descuento>(entity =>
        {
            entity.ToTable("Descuento");

            entity.HasIndex(e => e.Email, "IX_Descuento").IsUnique();

            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<EstadoTurno>(entity =>
        {
            entity.ToTable("EstadoTurno");

            entity.Property(e => e.Estado)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Paseadore>(entity =>
        {
            entity.HasIndex(e => new { e.Email, e.Ubicacion }, "IX_Paseadores").IsUnique();

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Foto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HorarioIn)
                .HasPrecision(0)
                .HasColumnName("HorarioIN");
            entity.Property(e => e.HorarioOut)
                .HasPrecision(0)
                .HasColumnName("HorarioOUT");
            entity.Property(e => e.Latitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Longitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Perro>(entity =>
        {
            entity.Property(e => e.Color)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FechaDeNacimiento).HasColumnType("date");
            entity.Property(e => e.Foto)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Raza)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Sexo)
                .HasMaxLength(10)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Publicacion>(entity =>
        {
            entity.ToTable("Publicacion");

            entity.HasOne(d => d.IdPerroNavigation).WithMany(p => p.Publicacions)
                .HasForeignKey(d => d.IdPerro)
                .HasConstraintName("FK_Publicacion_Perros");

            entity.HasOne(d => d.TipoNavigation).WithMany(p => p.Publicacions)
                .HasForeignKey(d => d.Tipo)
                .HasConstraintName("FK_Publicacion_TipoPublicacion");
        });

        modelBuilder.Entity<Rol>(entity =>
        {
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584CD2E41696");

            entity.ToTable("Rol");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoPublicacion>(entity =>
        {
            entity.ToTable("TipoPublicacion");

            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tratamiento>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TratamientoPerro>(entity =>
        {
            entity.ToTable("TratamientoPerro");

            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.Observaciones)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPerroNavigation).WithMany(p => p.TratamientoPerros)
                .HasForeignKey(d => d.IdPerro)
                .HasConstraintName("FK_TratamientoPerro_Perros");

            entity.HasOne(d => d.IdTratamientoNavigation).WithMany(p => p.TratamientoPerros)
                .HasForeignKey(d => d.IdTratamiento)
                .HasConstraintName("FK_TratamientoPerro_Tratamientos");
        });

        modelBuilder.Entity<Turno>(entity =>
        {
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.DuenoNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Dueno)
                .HasConstraintName("FK_Turnos_Usuarios");

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Estado)
                .HasConstraintName("FK_Turnos_EstadoTurno");

            entity.HasOne(d => d.PerroNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Perro)
                .HasConstraintName("FK_Turnos_Perros");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC078968375E");

            entity.HasIndex(e => e.Email, "IX_Usuarios").IsUnique();

            entity.Property(e => e.Apellido)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Pass)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(14)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Usuarios_Rol");
        });

        modelBuilder.Entity<UsuarioAdopcionPublicacion>(entity =>
        {
            entity.ToTable("UsuarioAdopcionPublicacion");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioAdopcionPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioAdopcionPublicacion_Publicacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioAdopcionPublicacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_UsuarioAdopcionPublicacion_Usuarios");
        });

        modelBuilder.Entity<UsuarioColectaPublicacion>(entity =>
        {
            entity.ToTable("UsuarioColectaPublicacion");

            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioColectaPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioColectaPublicacion_Publicacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioColectaPublicacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_UsuarioColectaPublicacion_Usuarios");
        });

        modelBuilder.Entity<UsuarioPerdidaPublicacion>(entity =>
        {
            entity.ToTable("UsuarioPerdidaPublicacion");

            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("date");

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioPerdidaPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioPerdidaPublicacion_Publicacion");

            entity.HasOne(d => d.IdUsuarioNavigation).WithMany(p => p.UsuarioPerdidaPublicacions)
                .HasForeignKey(d => d.IdUsuario)
                .HasConstraintName("FK_UsuarioPerdidaPublicacion_UsuarioPerdidaPublicacion");
        });

        modelBuilder.Entity<Vacuna>(entity =>
        {
            entity.Property(e => e.Vacuna1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Vacuna");
        });

        modelBuilder.Entity<VacunaPerro>(entity =>
        {
            entity.ToTable("VacunaPerro");

            entity.Property(e => e.Dosis)
                .HasMaxLength(10)
                .IsUnicode(false);
            entity.Property(e => e.FechaAplicacion).HasColumnType("date");
            entity.Property(e => e.NroLote)
                .HasMaxLength(20)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPerroNavigation).WithMany(p => p.VacunaPerros)
                .HasForeignKey(d => d.IdPerro)
                .HasConstraintName("FK_VacunaPerro_Perros");

            entity.HasOne(d => d.IdVacunaNavigation).WithMany(p => p.VacunaPerros)
                .HasForeignKey(d => d.IdVacuna)
                .HasConstraintName("FK_VacunaPerro_Vacunas");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
