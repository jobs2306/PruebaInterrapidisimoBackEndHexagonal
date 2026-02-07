namespace Productos.Infraestructura.Context.Extensiones
{
    using Microsoft.EntityFrameworkCore;
    using RegistroEstudiantesH.Dominio.Entidades;

    /// <summary>
    /// Clase de extension para entidades de productos
    /// </summary>
    public static class EstudiantesExtensionModelBuilder
    {
        /// <summary>
        /// Metodo de extension para entidades del modelBuilder
        /// </summary>
        /// <param name="modelBuilder"> ModelBuilder </param>
        /// <returns> ModelBuilder </returns>
        public static ModelBuilder AgregarEntidadesProductos(this ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Estudiantes>(entity =>
            {
                entity.HasKey(e => e.EstudianteId).HasName("PK_Estudiantes");

                entity.HasIndex(e => e.Email, "IX_Estudiantes_Email").IsUnique();

                entity.Property(e => e.Email)
                    .IsRequired()
                    .HasMaxLength(150);
                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
                entity.Property(e => e.PasswordHash).IsRequired();
            });

            modelBuilder.Entity<EstudianteMaterias>(entity =>
            {
                entity.HasKey(e => e.EstudianteMateriaId).HasName("PK_EstudianteMaterias");

                entity.HasIndex(e => new { e.EstudianteId, e.MateriaId, e.ProfesorId },
                    "IX_EstudianteMaterias_EstudianteId_MateriaId_ProfesorId").IsUnique();

                entity.HasIndex(e => e.MateriaId, "IX_EstudianteMaterias_MateriaId");

                entity.HasIndex(e => e.ProfesorId, "IX_EstudianteMaterias_ProfesorId");

                entity.HasOne(d => d.Estudiante).WithMany(p => p.EstudianteMaterias).HasForeignKey(d => d.EstudianteId);

                entity.HasOne(d => d.Materia).WithMany(p => p.EstudianteMaterias)
                    .HasForeignKey(d => d.MateriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Profesor).WithMany(p => p.EstudianteMaterias)
                    .HasForeignKey(d => d.ProfesorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Materias>(entity =>
            {
                entity.HasKey(e => e.MateriaId).HasName("PK_Materias");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            modelBuilder.Entity<ProfesorMaterias>(entity =>
            {
                entity.HasKey(e => e.MateriaId).HasName("PK_ProfesorMaterias");

                entity.HasIndex(e => e.MateriaId, "IX_ProfesorMaterias_MateriaId");

                entity.HasIndex(e => new { e.ProfesorId, e.MateriaId }, "IX_ProfesorMaterias_ProfesorId_MateriaId").IsUnique();

                entity.HasOne(d => d.Materia).WithMany(p => p.ProfesorMaterias)
                    .HasForeignKey(d => d.MateriaId)
                    .OnDelete(DeleteBehavior.ClientSetNull);

                entity.HasOne(d => d.Profesor).WithMany(p => p.ProfesorMaterias)
                    .HasForeignKey(d => d.ProfesorId)
                    .OnDelete(DeleteBehavior.ClientSetNull);
            });

            modelBuilder.Entity<Profesores>(entity =>
            {
                entity.HasKey(e => e.ProfesorId).HasName("PK_Profesores");

                entity.Property(e => e.Nombre)
                    .IsRequired()
                    .HasMaxLength(100);
            });

            return modelBuilder;
        }
    }
}
