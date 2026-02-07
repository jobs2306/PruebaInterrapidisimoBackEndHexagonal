namespace Transversales.Repositorio.Implementaciones
{
    using Microsoft.EntityFrameworkCore;
    using System.Collections;
    using Transversales.Repositorio.Interfaces;

    /// <summary>
    /// Clase que implementa la unidad de trabajo (UnitOfWork) para la gestión de transacciones y repositorios.
    /// Proporciona métodos para gestionar el ciclo de vida del contexto de base de datos y los repositorios genéricos.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private Hashtable? _repositories;
        private readonly DbContext _dbContext;

        /// <summary>
        /// Constructor que inicializa el contexto de la base de datos.
        /// </summary>
        /// <param name="dbContext">Instancia de DbContext que representa la conexión a la base de datos.</param>
        public UnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }


        public async Task<int> CompleteAsync()
        {
            try
            {
                var x = await _dbContext.SaveChangesAsync();
                _dbContext.ChangeTracker.Clear();
                return x;
            }
            catch (Exception e)
            {
                throw new Exception("Error transacción: " + e.Message);
            }
        }

        /// <summary>
        /// Libera los recursos utilizados por el contexto de la base de datos.
        /// </summary>
        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public IRepositorioGenerico<TEntity, TId> Repository<TEntity, TId>() where TEntity : class
        {
            if (_repositories == null)
            {
                _repositories = new Hashtable();
            }

            var type = typeof(TEntity).Name;
            if (!_repositories.ContainsKey(type))
            {
                var repositoryType = typeof(RepositorioGenerico<,>);
                var repositoryInstance = Activator
                    .CreateInstance(repositoryType.MakeGenericType(typeof(TEntity), typeof(TId)), _dbContext);
                _repositories.Add(type, repositoryInstance);

            }
            return (IRepositorioGenerico<TEntity, TId>)_repositories[type]!;
        }
    }
}
