using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProyectoMSD.Modelos;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    // --- NUESTRA ARQUITECTURA LIMPIA ---
    public virtual DbSet<Configuracione> Configuraciones { get; set; }
    public virtual DbSet<Dispositivo> Dispositivos { get; set; }
    public virtual DbSet<Espacio> Espacios { get; set; }
    public virtual DbSet<Propiedade> Propiedades { get; set; }
    public virtual DbSet<Soporte> Soportes { get; set; }
    public virtual DbSet<Usuario> Usuarios { get; set; }
    public virtual DbSet<RegistroAcceso> RegistroAccesos { get; set; }
    public virtual DbSet<Notificacion> Notificaciones { get; set; }
    public virtual DbSet<UsuariosPropiedade> UsuariosPropiedades { get; set; }
    
    // LA NUEVA TABLA DE GOOGLE MAPS
    public virtual DbSet<Ubicacione> Ubicaciones { get; set; } 

    // NOTA PROFESIONAL: El método OnConfiguring se eliminó por completo 
    // para evitar que fuerce la conexión local a 127.0.0.1.

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_general_ci")
            .HasCharSet("utf8mb4");

        // ELIMINAMOS EL BLOQUE DE 'Almacenan' POR COMPLETO

        modelBuilder.Entity<Configuracione>(entity =>
        {
            entity.HasKey(e => e.Codigo).HasName("PRIMARY");
            entity.ToTable("configuraciones");
            entity.HasIndex(e => e.IdDispositivos, "ID_Dispositivos");
            entity.HasIndex(e => e.IdUsuarios, "ID_Usuarios");
            entity.Property(e => e.Codigo).HasColumnType("int(11)");
            entity.Property(e => e.Adaptador).HasMaxLength(250);
            entity.Property(e => e.Alertas).HasMaxLength(250);
            entity.Property(e => e.IdDispositivos).HasColumnType("int(11)").HasColumnName("ID_Dispositivos");
            entity.Property(e => e.IdUsuarios).HasColumnType("int(11)").HasColumnName("ID_Usuarios");
            entity.Property(e => e.Idioma).HasMaxLength(250);

            entity.HasOne(d => d.IdDispositivosNavigation).WithMany(p => p.Configuraciones)
                .HasForeignKey(d => d.IdDispositivos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("configuraciones_ibfk_1");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.Configuraciones)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("configuraciones_ibfk_2");
        });

        modelBuilder.Entity<Dispositivo>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("dispositivos");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            
            // Actualizado para reflejar la nueva estructura y longitudes
            entity.Property(e => e.IdEspacio).HasColumnType("int(11)").HasColumnName("ID_Espacio");
            entity.Property(e => e.Estado).HasMaxLength(30);
            entity.Property(e => e.Marca).HasMaxLength(50);
            entity.Property(e => e.Nombre).HasMaxLength(100);
            entity.Property(e => e.Tipo).HasMaxLength(50);
            entity.Property(e => e.Usos).HasMaxLength(255);
            entity.Property(e => e.MAC_Address).HasMaxLength(50);
            entity.Property(e => e.Protocolo).HasMaxLength(50);
        });

        modelBuilder.Entity<Espacio>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("espacios");
            entity.HasIndex(e => e.IdPropiedades, "ID_Propiedad");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            entity.Property(e => e.IdPropiedades).HasColumnType("int(11)").HasColumnName("ID_Propiedad");
            entity.Property(e => e.Nombre).HasMaxLength(250);
            entity.Property(e => e.Permisos).HasMaxLength(250);
            entity.Property(e => e.Señal).HasMaxLength(11).HasColumnName("Senal");

            entity.HasOne(d => d.IdPropiedadesNavigation).WithMany(p => p.Espacios)
                .HasForeignKey(d => d.IdPropiedades)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("espacios_ibfk_1");
        });

        modelBuilder.Entity<Propiedade>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("propiedades");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            entity.Property(e => e.Direccion).HasMaxLength(250);
            entity.Property(e => e.Pisos).HasColumnType("int(11)");
            entity.Property(e => e.Tipo).HasMaxLength(250);
        });

        modelBuilder.Entity<Soporte>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("soportes");
            entity.HasIndex(e => e.IdUsuarios, "ID_USUARIOS");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            entity.Property(e => e.Descripcion).HasMaxLength(250);
            entity.Property(e => e.IdUsuarios).HasColumnType("int(11)").HasColumnName("ID_USUARIOS");
            entity.Property(e => e.Respuesta).HasMaxLength(250);
            entity.Property(e => e.Tipo).HasMaxLength(250);

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.Soportes)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("soportes_ibfk_1");
        });

        modelBuilder.Entity<Usuario>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("usuarios");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            entity.Property(e => e.Acesso).HasMaxLength(250);
            entity.Property(e => e.Clave).HasMaxLength(250);
            entity.Property(e => e.Correo).HasMaxLength(250);
            entity.Property(e => e.Documento).HasMaxLength(20);
            entity.Property(e => e.Nombre).HasMaxLength(250);
            entity.Property(e => e.Permisos).HasMaxLength(250);
            entity.Property(e => e.PrefijoTelefono).HasMaxLength(10);
            entity.Property(e => e.Rol).HasMaxLength(250);
            entity.Property(e => e.Rut).HasMaxLength(250);
            entity.Property(e => e.Telefono).HasMaxLength(20);
            entity.Property(e => e.IdUbicacion).HasColumnType("int(11)").HasColumnName("ID_Ubicacion");

            entity.HasOne(d => d.UbicacionNavigation)
                .WithMany(p => p.Usuarios)
                .HasForeignKey(d => d.IdUbicacion)
                .OnDelete(DeleteBehavior.SetNull) // Si se borra la ubicación, el usuario queda con null
                .HasConstraintName("FK_usuarios_ubicaciones");
        });
        modelBuilder.Entity<Notificacion>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("notificaciones");
            
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("Id");
            entity.Property(e => e.IdUsuarios).HasColumnType("int(11)").HasColumnName("IdUsuarios");
            entity.Property(e => e.Titulo).HasMaxLength(255).IsRequired();
            entity.Property(e => e.Mensaje).HasColumnType("text").IsRequired();
            entity.Property(e => e.Tipo).HasMaxLength(50).IsRequired();
            entity.Property(e => e.Leida).HasColumnType("tinyint(1)").HasDefaultValue(false);
            entity.Property(e => e.FechaCreacion).HasColumnType("datetime").IsRequired();
            entity.Property(e => e.RutaRedireccion).HasMaxLength(255).IsRequired(false);

            entity.HasOne(d => d.Usuario).WithMany(p => p.Notificaciones)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_notificaciones_usuarios");
        });

        modelBuilder.Entity<RegistroAcceso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.ToTable("registro_accesos");
            entity.HasIndex(e => e.IdUsuarios, "ID_Usuarios");
            entity.HasIndex(e => e.FechaAcceso, "IX_FechaAcceso");
            entity.Property(e => e.Id).HasColumnType("int(11)").HasColumnName("ID");
            entity.Property(e => e.IdUsuarios).HasColumnType("int(11)").HasColumnName("ID_Usuarios");
            entity.Property(e => e.FechaAcceso).HasColumnType("datetime");
            entity.Property(e => e.TipoAccion).HasMaxLength(50);
            entity.Property(e => e.DireccionIp).HasMaxLength(45).HasColumnName("Direccion_Ip");
            entity.Property(e => e.Navegador).HasMaxLength(500);
            entity.Property(e => e.PaginaVisitada).HasMaxLength(250).HasColumnName("Pagina_Visitada");
            entity.Property(e => e.DuracionSesion).HasColumnType("int(11)").HasColumnName("Duracion_Sesion");

            entity.HasOne(d => d.IdUsuariosNavigation).WithMany(p => p.RegistroAccesos)
                .HasForeignKey(d => d.IdUsuarios)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("registro_accesos_ibfk_1");
        });

            modelBuilder.Entity<UsuariosPropiedade>(entity =>
        {
            // 1. Forzamos el nombre de la tabla (asegúrate que sea el de tu BD)
            entity.ToTable("usuarios_propiedades"); 

            entity.HasKey(e => e.Id).HasName("PRIMARY");

            // 2. FORZAMOS el nombre de la columna en SINGULAR
            // Esto mata el error "e.ID_Propiedades"
            entity.Property(e => e.IdPropiedad)
                .HasColumnName("ID_Propiedad"); 

            entity.Property(e => e.IdUsuario)
                .HasColumnName("ID_Usuario");

            // EL PROBLEMA: Tu BD tiene "Rol_En_Propiedad" (snake_case modificado)
            // Agregamos el mapeo explícito para evitar el error de columna desconocida
            entity.Property(e => e.RolEnPropiedad)
                .HasColumnName("Rol_En_Propiedad");

            // 3. Definimos las relaciones explícitamente
            entity.HasOne(d => d.IdPropiedadNavigation)
                .WithMany(p => p.UsuariosPropiedades)
                .HasForeignKey(d => d.IdPropiedad)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_usuarios_propiedades_propiedad");

            entity.HasOne(d => d.IdUsuarioNavigation)
                .WithMany(p => p.UsuariosPropiedades)
                .HasForeignKey(d => d.IdUsuario)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("FK_usuarios_propiedades_usuario");
        });

       modelBuilder.Entity<Ubicacione>(entity =>
        {
            // 1. Nombre de la tabla tal cual está en MySQL
            entity.ToTable("ubicaciones"); 

            // 2. Mapeo de Clave Primaria
            entity.HasKey(e => e.Id).HasName("PRIMARY");
            entity.Property(e => e.Id).HasColumnName("ID");

            // 3. Mapeo de Columnas con Guiones Bajos (Snake Case)
            entity.Property(e => e.DireccionFormateada)
                .HasColumnName("Direccion_Formateada")
                .HasColumnType("varchar(255)"); // O "text" según tu BD

            entity.Property(e => e.FechaCreacion)
                .HasColumnName("FechaCreacion")
                .HasDefaultValueSql("CURRENT_TIMESTAMP");

            // 4. Mapeo de Coordenadas (Precisión para GPS)
            entity.Property(e => e.Latitud)
                .HasColumnName("Latitud")
                .HasPrecision(18, 12);

            entity.Property(e => e.Longitud)
                .HasColumnName("Longitud")
                .HasPrecision(18, 12);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}