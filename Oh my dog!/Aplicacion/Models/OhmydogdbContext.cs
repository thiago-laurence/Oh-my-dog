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

    public virtual DbSet<Cuidadores> Cuidadores { get; set; }

    public virtual DbSet<Descuentos> Descuentos { get; set; }

    public virtual DbSet<EstadoTurno> EstadoTurnos { get; set; }

    public virtual DbSet<HorarioTurno> HorarioTurnos { get; set; }

    public virtual DbSet<ModalidadCuidador> ModalidadCuidadors { get; set; }

    public virtual DbSet<Paseadores> Paseadores { get; set; }

    public virtual DbSet<Perros> Perros { get; set; }

    public virtual DbSet<PerroTurnos> PerroTurnos { get; set; }

    public virtual DbSet<Publicacion> Publicacions { get; set; }

    public virtual DbSet<Rol> Rols { get; set; }

    public virtual DbSet<TipoModalidad> TipoModalidads { get; set; }

    public virtual DbSet<TipoPublicacion> TipoPublicacions { get; set; }

    public virtual DbSet<Tratamientos> Tratamientos { get; set; }

    public virtual DbSet<TratamientoPerro> TratamientoPerros { get; set; }

    public virtual DbSet<Turnos> Turnos { get; set; }

    public virtual DbSet<Usuarios> Usuarios { get; set; }

    public virtual DbSet<UsuarioAdopcionPublicacion> UsuarioAdopcionPublicacions { get; set; }

    public virtual DbSet<UsuarioColectaPublicacion> UsuarioColectaPublicacions { get; set; }

    public virtual DbSet<UsuarioPerdidaPublicacion> UsuarioPerdidaPublicacions { get; set; }

    public virtual DbSet<Vacuna> Vacunas { get; set; }

    public virtual DbSet<VacunaPerro> VacunaPerros { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("server=localhost; database=ohmydogdb; integrated security=true; trustservercertificate=true;");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Cuidadores>(entity =>
        {
            entity.HasIndex(e => new { e.Ubicacion, e.Email }, "IX_Cuidadores");

            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(300)
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
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Descuentos>(entity =>
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

        modelBuilder.Entity<HorarioTurno>(entity =>
        {
            entity.ToTable("HorarioTurno");

            entity.Property(e => e.Turno)
                .HasMaxLength(30)
                .IsUnicode(false);
        });

        modelBuilder.Entity<ModalidadCuidador>(entity =>
        {
            entity.ToTable("ModalidadCuidador");

            entity.HasOne(d => d.IdCuidadorNavigation).WithMany(p => p.ModalidadCuidadors)
                .HasForeignKey(d => d.IdCuidador)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModalidadCuidador_Cuidadores");

            entity.HasOne(d => d.IdModalidadNavigation).WithMany(p => p.ModalidadCuidadors)
                .HasForeignKey(d => d.IdModalidad)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_ModalidadCuidador_TipoModalidad");
        });

        modelBuilder.Entity<Paseadores>(entity =>
        {
            entity.HasIndex(e => new { e.Email, e.Ubicacion }, "IX_Paseadores");

            entity.Property(e => e.Apellido)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(300)
                .IsUnicode(false);
            entity.Property(e => e.Foto)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.HorarioIn)
                .HasPrecision(0)
                .HasColumnName("HorarioIN");
            entity.Property(e => e.HorarioOut).HasPrecision(0);
            entity.Property(e => e.Latitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Longitud)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(100)
                .IsUnicode(false);
            entity.Property(e => e.Ubicacion)
                .HasMaxLength(200)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Perros>(entity =>
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

            entity.HasOne(d => d.IdDueñoNavigation).WithMany(p => p.Perros)
                .HasForeignKey(d => d.IdDueño)
                .HasConstraintName("FK_Perros_Usuarios");
        });

        modelBuilder.Entity<PerroTurnos>(entity =>
        {
            entity.ToTable("PerroTurno");

            entity.HasIndex(e => e.Id, "IX_PerroTurno");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPerroNavigation).WithMany(p => p.PerroTurnos)
                .HasForeignKey(d => d.IdPerro)
                .HasConstraintName("FK_PerroTurno_Perros");

            entity.HasOne(d => d.IdTurnoNavigation).WithMany(p => p.PerroTurnos)
                .HasForeignKey(d => d.IdTurno)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PerroTurno_Turnos");
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
            entity.HasKey(e => e.IdRol).HasName("PK__Rol__2A49584C2F1A143A");

            entity.ToTable("Rol");

            entity.Property(e => e.IdRol).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoModalidad>(entity =>
        {
            entity.ToTable("TipoModalidad");

            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TipoPublicacion>(entity =>
        {
            entity.ToTable("TipoPublicacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Tratamientos>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Nombre)
                .HasMaxLength(50)
                .IsUnicode(false);
        });

        modelBuilder.Entity<TratamientoPerro>(entity =>
        {
            entity.ToTable("TratamientoPerro");

            entity.Property(e => e.Id).ValueGeneratedNever();
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

        modelBuilder.Entity<Turnos>(entity =>
        {
            entity.Property(e => e.Comentario)
                .HasMaxLength(200)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("date");
            entity.Property(e => e.HorarioFinal)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.EstadoNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Estado)
                .HasConstraintName("FK_Turnos_EstadoTurno");

            entity.HasOne(d => d.HorarioNavigation).WithMany(p => p.Turnos)
                .HasForeignKey(d => d.Horario)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Turnos_HorarioTurno");
        });

        modelBuilder.Entity<Usuarios>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Usuarios__3214EC076CD730FD");

            entity.Property(e => e.Apellido)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Direccion)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Nombre)
                .HasMaxLength(20)
                .IsUnicode(false);
            entity.Property(e => e.Pass)
                .HasMaxLength(250)
                .IsUnicode(false);
            entity.Property(e => e.Telefono)
                .HasMaxLength(10)
                .IsUnicode(false);

            entity.HasOne(d => d.IdRolNavigation).WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdRol)
                .HasConstraintName("FK__Usuarios__IdRol__5BE2A6F2");
        });

        modelBuilder.Entity<UsuarioAdopcionPublicacion>(entity =>
        {
            entity.ToTable("UsuarioAdopcionPublicacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioAdopcionPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioAdopcionPublicacion_Publicacion");
        });

        modelBuilder.Entity<UsuarioColectaPublicacion>(entity =>
        {
            entity.ToTable("UsuarioColectaPublicacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Motivo)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Titulo)
                .HasMaxLength(50)
                .IsUnicode(false);

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioColectaPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioColectaPublicacion_Publicacion");
        });

        modelBuilder.Entity<UsuarioPerdidaPublicacion>(entity =>
        {
            entity.ToTable("UsuarioPerdidaPublicacion");

            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Descripcion)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.Fecha).HasColumnType("date");

            entity.HasOne(d => d.IdPublicacionNavigation).WithMany(p => p.UsuarioPerdidaPublicacions)
                .HasForeignKey(d => d.IdPublicacion)
                .HasConstraintName("FK_UsuarioPerdidaPublicacion_Publicacion");
        });

        modelBuilder.Entity<Vacuna>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Vacuna1)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("Vacuna");
        });

        modelBuilder.Entity<VacunaPerro>(entity =>
        {
            entity.ToTable("VacunaPerro");

            entity.Property(e => e.Id).ValueGeneratedNever();
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
