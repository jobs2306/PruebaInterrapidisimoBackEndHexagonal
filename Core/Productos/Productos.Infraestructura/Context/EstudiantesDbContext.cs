namespace Productos.Infraestructura.Context
{
    using Microsoft.EntityFrameworkCore;
    using Productos.Infraestructura.Context.Extensiones;

    /// <summary>
    /// Contexto de base de datos para productos
    /// </summary>
    public class EstudiantesDbContext : DbContext
    {
        /// <summary>
        /// Constructor de la clase
        /// </summary>
        /// <param name="options"> Opciones de configuración del contexto </param>
        public EstudiantesDbContext(DbContextOptions<EstudiantesDbContext> options) : base(options)
        { }

        /// <summary>
        /// Método que se encarga de configurar las entidades del contexto
        /// </summary>
        /// <param name="modelBuilder"> Constructor de modelos </param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.AgregarEntidadesProductos();
            base.OnModelCreating(modelBuilder);
        }
    }
}
