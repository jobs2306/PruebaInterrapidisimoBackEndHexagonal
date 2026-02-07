namespace Transversales.Repositorio.Interfaces
{
    /// <summary>
    /// Interfaz que contiene los metedos de implementacion del patron Unit Of Work
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Guarda los cambios pendientes en la base de datos de forma asíncrona.
        /// </summary>
        /// <returns>El número de entidades afectadas tras guardar los cambios.</returns>
        /// <exception cref="Exception">Se lanza una excepción si ocurre un error al guardar los cambios.</exception>
        Task<int> CompleteAsync();

        /// <summary>
        /// Obtiene una instancia de un repositorio genérico para el tipo de entidad especificado.
        /// Si no existe previamente, se crea una nueva instancia del repositorio genérico.
        /// </summary>
        /// <typeparam name="TEntity">El tipo de la entidad gestionada por el repositorio.</typeparam>
        /// <typeparam name="TId">El tipo del identificador de la entidad.</typeparam>
        /// <returns>Una instancia del repositorio genérico para la entidad especificada.</returns>
        IRepositorioGenerico<TEntity, TId> Repository<TEntity, TId>() where TEntity : class;
    }
}
