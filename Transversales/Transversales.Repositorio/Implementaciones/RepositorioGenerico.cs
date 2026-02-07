namespace Transversales.Repositorio.Implementaciones
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Query;
    using System.Linq;
    using System.Linq.Expressions;
    using Transversales.Repositorio.Interfaces;

    /// <summary>
    /// Clase que implementa la interfaz IRepositorio generico
    /// </summary>
    /// <typeparam name="TEntidad">Entidad a la cual se le realizaran las operaciones crud</typeparam>
    /// <typeparam name="TipoId">Tipo de id que maneja la entidad</typeparam>
    public class RepositorioGenerico<TEntidad, TipoId> : IRepositorioGenerico<TEntidad, TipoId>
        where TEntidad : class
    {
        protected readonly DbContext _dbContext;

        public RepositorioGenerico(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // </inheritdoc>
        public async Task ActualizarAsync(TEntidad entidad)
        {
            var entityType = _dbContext.Model.FindEntityType(typeof(TEntidad))!;
            var keyProp = entityType.FindPrimaryKey()!.Properties.First();
            var keyName = keyProp.Name;
            var keyValue = typeof(TEntidad).GetProperty(keyName)!.GetValue(entidad);

            var localEntry = _dbContext.ChangeTracker
                .Entries<TEntidad>()
                .FirstOrDefault(e =>
                    e.Property(keyName).CurrentValue!.Equals(keyValue)
                );

            if (localEntry != null)
            {
                localEntry.CurrentValues.SetValues(entidad);
            }
            else
            {
                _dbContext.Set<TEntidad>().Attach(entidad);
                _dbContext.Entry(entidad).State = EntityState.Modified;
            }

            await Task.CompletedTask;
        }

        // </inheritdoc>
        public async Task<TEntidad> AgregarEntidadAsync(TEntidad entidad)
        {
            await _dbContext.Set<TEntidad>().AddAsync(entidad);
            return entidad;
        }

        // </inheritdoc>
        public async Task AgregarVariosAsync(IEnumerable<TEntidad> entidades)
        {
            await Task.Run(() => _dbContext.Set<TEntidad>().AddRange(entidades));
        }

        // </inheritdoc>
        public IQueryable<TEntidad> AsQueryable()
        {
            return _dbContext.Set<TEntidad>().AsQueryable();
        }

        // </inheritdoc>
        public async Task EliminarAsync(TEntidad entidad)
        {
            await Task.Run(() => _dbContext.Set<TEntidad>().Remove(entidad));
        }

        // </inheritdoc>
        public async Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(bool conTracking, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (!conTracking) return await query.AsNoTracking().ToListAsync(cancellationToken);
            return await query.ToListAsync(cancellationToken);
        }

        // </inheritdoc>
        public async Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            return await query.ToListAsync(cancellationToken);
        }

        // </inheritdoc>
        public async Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(bool conTracking, Expression<Func<TEntidad, bool>>? filtro = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (filtro is not null)
                query = query.Where(filtro);

            if (conTracking) return await query.ToListAsync(cancellationToken);
            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        // </inheritdoc>
        public async Task<IReadOnlyList<TEntidad>> ObtenerTodosAsync(Expression<Func<TEntidad, bool>>? filtro = null, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (filtro is not null)
                query = query.Where(filtro);

            return await query.AsNoTracking().ToListAsync(cancellationToken);
        }

        // </inheritdoc>
        public async Task<TEntidad> ObtenerEntidadAsync(
            Expression<Func<TEntidad, bool>>? filtro,
            bool disableTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (disableTracking) query = query.AsNoTracking();
            if (filtro != null) query = query.Where(filtro);
            return (await query.FirstOrDefaultAsync(cancellationToken))!;
        }

        // </inheritdoc>
        public async Task<IEnumerable<TEntidad>> ObtenerEntidadesIncludesAsync(Expression<Func<TEntidad, bool>>? filtro = null,
            List<Func<IQueryable<TEntidad>, IIncludableQueryable<TEntidad, object>>>? includes = null,
            Func<IQueryable<TEntidad>, IOrderedQueryable<TEntidad>>? orderBy = null, bool disableTracking = true,
            CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include(query);
                }
            }
            if (filtro != null) query = query.Where(filtro);
            if (orderBy != null) return await orderBy(query).ToListAsync(cancellationToken);
            else
            {
                return await query.ToListAsync(cancellationToken);
            }
        }

        // </inheritdoc>
        public async Task<TEntidad> ObtenerEntidadIncludesAsync(Expression<Func<TEntidad, bool>>? filtro = null, List<Func<IQueryable<TEntidad>, IIncludableQueryable<TEntidad, object>>>? includes = null,
            bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (disableTracking) query = query.AsNoTracking();
            if (includes != null)
            {
                foreach (var include in includes)
                {
                    query = include(query);
                }
            }
            if (filtro != null) query = query.Where(filtro);
            return (await query.FirstOrDefaultAsync(cancellationToken))!;
        }

        // </inheritdoc>
        public async Task<TEntidad> ObtenerEntidadProyeccionesAsync(Expression<Func<TEntidad, bool>>? filtro = null, Expression<Func<TEntidad, TEntidad>>? proyeccion = null,
            bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (disableTracking) query = query.AsNoTracking();
            if (filtro is not null) query = query.Where(filtro);
            if (proyeccion is not null) query = query.Select(proyeccion);
            return (await query.FirstOrDefaultAsync(cancellationToken))!;
        }

        // </inheritdoc>
        public async Task<IEnumerable<TEntidad>> ObtenerEntidadesProyeccionesAsync(Expression<Func<TEntidad, bool>>? filtro = null, Expression<Func<TEntidad, TEntidad>>? proyeccion = null,
            Func<IQueryable<TEntidad>, IOrderedQueryable<TEntidad>>? orderBy = null,
            bool disableTracking = true, CancellationToken cancellationToken = default)
        {
            IQueryable<TEntidad> query = _dbContext.Set<TEntidad>();
            if (disableTracking) query = query.AsNoTracking();
            if (filtro is not null) query = query.Where(filtro);
            if (proyeccion is not null) query = query.Select(proyeccion);
            if (orderBy != null) return await orderBy(query).ToListAsync(cancellationToken);
            return (await query.ToListAsync(cancellationToken))!;
        }

        // </inheritdoc>
        public async Task<TEntidad> ObtenerPorIdAsync(TipoId id, CancellationToken cancellationToken = default)
        {
            if (id == null!) return null!;
            return (await _dbContext.Set<TEntidad>().FindAsync(new object[] { id }, cancellationToken))!;
        }

        // </inheritdoc>
        public async Task<TEntidad> ObtenerPorIdAsync(TipoId id, bool disableTracking, CancellationToken cancellationToken = default)
        {
            if (id == null!) return null!;

            var entidad = await _dbContext.Set<TEntidad>().FindAsync(new object[] { id }, cancellationToken);

            if (disableTracking && entidad != null)
            {
                _dbContext.Entry(entidad).State = EntityState.Detached;
            }

            return entidad!;
        }

        // </inheritdoc>
        public async Task EliminarVariosAsync(IEnumerable<TEntidad> entidades)
        {
            await Task.Run(() => _dbContext.Set<TEntidad>().RemoveRange(entidades));
        }
    }
}
