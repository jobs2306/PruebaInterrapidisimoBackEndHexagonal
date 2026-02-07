namespace Transversales.Repositorio.Interfaces
{
    using Microsoft.EntityFrameworkCore.Query;
    using System.Linq.Expressions;

    /// <summary>
    /// Interfaz que define un patrón de repositorio genérico para gestionar entidades en un almacén de datos.
    /// </summary>
    /// <typeparam name="TEntidad">El tipo de la entidad.</typeparam>
    /// <typeparam name="TipoId">El tipo del identificador de la entidad.</typeparam>
    public interface IRepositorioGenerico<TEntidad, TipoId>
        where TEntidad : class
    {
        /// <param name="disableTracking">Indica si se debe deshabilitar el seguimiento.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene la entidad que coincide con los criterios.</returns>
        Task<TEntidad> ObtenerEntidadAsync(Expression<Func<TEntidad, bool>>? filtro = null,
                                           bool disableTracking = true,
                                           CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera de manera asíncrona una entidad por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la entidad.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene la entidad con el identificador especificado.</returns>
        Task<TEntidad> ObtenerPorIdAsync(TipoId id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera de manera asíncrona una entidad por su identificador.
        /// </summary>
        /// <param name="id">El identificador de la entidad.</param>
        /// <param name="disableTracking">Indica si se debe deshabilitar el seguimiento.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene la entidad con el identificador especificado.</returns>
        Task<TEntidad> ObtenerPorIdAsync(TipoId id, bool disableTracking, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera todas las entidades de manera asíncrona.
        /// </summary>
        /// <param name="conTracking">Indica si trackea las entidades, por defecto true</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene una lista de solo lectura de entidades.</returns>
        Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(bool conTracking, CancellationToken cancellationToken = default);

        /// <inheritdoc cref="ObtenerTodosAsync(bool,System.Threading.CancellationToken)"/>W
        Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera de manera asíncrona las entidades que coinciden con un predicado especificado.
        /// </summary>
        /// <param name="filtro">Una función para filtrar las entidades.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <param name="conTracking">Indica si trackea las entidades, por defecto true</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene una lista de solo lectura de entidades que coinciden con el predicado.</returns>
        Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(bool conTracking, Expression<Func<TEntidad, bool>>? filtro = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Recupera de manera asíncrona las entidades que coinciden con un predicado especificado.
        /// </summary>W
        /// <param name="filtro">Unafunción para filtrar entidades</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene una lista de solo lectura de entidades que coinciden con el predicado.</returns>
        Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(Expression<Func<TEntidad, bool>>? filtro = null, CancellationToken cancellationToken = default);

        /// <summary>
        /// Obtiene entidad por Id
        /// </summary>
        Task<TEntidad> ObtenerEntidadProyeccionesAsync(Expression<Func<TEntidad, bool>>? filtro = null, Expression<Func<TEntidad, TEntidad>>? proyeccion = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);
        /// <summary>
        /// Obtiene entidades
        /// </summary>
        Task<IEnumerable<TEntidad>> ObtenerEntidadesProyeccionesAsync(Expression<Func<TEntidad, bool>>? filtro = null, Expression<Func<TEntidad, TEntidad>>? proyeccion = null,
            Func<IQueryable<TEntidad>, IOrderedQueryable<TEntidad>>? orderBy = null,
            bool disableTracking = true, CancellationToken cancellationToken = default);

        /// <param name="filtro">Una función para filtrar las entidades.</param>
        /// <param name="includes">Lista de entidades relacionadas a incluir como expresiones.</param>
        /// <param name="orderBy">Una función para ordenar las entidades.</param>
        /// <param name="disableTracking">Indica si se debe deshabilitar el seguimiento.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene una lista de solo lectura de entidades que coinciden con los criterios.</returns>
        Task<IEnumerable<TEntidad>> ObtenerEntidadesIncludesAsync(
            Expression<Func<TEntidad, bool>>? filtro = null,
            List<Func<IQueryable<TEntidad>, IIncludableQueryable<TEntidad, object>>>? includes = null,
            Func<IQueryable<TEntidad>, IOrderedQueryable<TEntidad>>? orderBy = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);

        /// <param name="filtro">Una función para filtrar las entidades.</param>
        /// <param name="includes">Lista de entidades relacionadas a incluir como expresiones.</param>
        /// <param name="orderBy">Una función para ordenar las entidades.</param>
        /// <param name="disableTracking">Indica si se debe deshabilitar el seguimiento.</param>
        /// <param name="cancellationToken">Token para monitorear solicitudes de cancelación.</param>
        /// <returns>Una tarea que representa la operación asincrónica, que contiene una lista de solo lectura de entidades que coinciden con los criterios.</returns>
        Task<TEntidad> ObtenerEntidadIncludesAsync(
            Expression<Func<TEntidad, bool>>? filtro = null,
            List<Func<IQueryable<TEntidad>, IIncludableQueryable<TEntidad, object>>>? includes = null,
            bool disableTracking = true,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Crea entidad
        /// </summary>
        Task<TEntidad> AgregarEntidadAsync(TEntidad entidad);

        /// <summary>
        /// Crea varias entidades de manera asíncrona.
        /// </summary>
        /// <param name="entidades">Las entidades a agregar.</param>
        /// <returns>Una tarea que representa la operación asincrónica.</returns>
        Task AgregarVariosAsync(IEnumerable<TEntidad> entidades);

        /// <summary>
        /// Actualiza entidad
        /// </summary>
        Task ActualizarAsync(TEntidad entidad);

        /// <summary>
        /// Elimina entidad
        /// </summary>
        Task EliminarAsync(TEntidad entidades);

        /// <summary>
        /// Eliminar varios
        /// </summary>
        Task EliminarVariosAsync(IEnumerable<TEntidad> entidad);

        /// <summary>
        /// Obtiene una consulta de tipo <see cref="IQueryable{TEntidad}"/> que permite realizar operaciones de filtrado, ordenación y proyección
        /// sobre la entidad <typeparamref name="TEntidad"/> en el almacén de datos.
        /// </summary>
        /// <returns>Una consulta de tipo <see cref="IQueryable{TEntidad}"/> para la entidad <typeparamref name="TEntidad"/>.</returns>
        IQueryable<TEntidad> AsQueryable();
    }
}
