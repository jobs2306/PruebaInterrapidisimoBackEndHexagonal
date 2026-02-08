namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Queries.Materia
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using RegistroEstudiantesH.Aplicacion.Dtos.Materia.Salida;
    using RegistroEstudiantesH.Aplicacion.Helpers;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;

    /// <summary>
    /// Query para obtener el listado de materias matriculadas incluyendo los companieros de cada materia
    /// </summary>
    public class ObtenerMateriasMatriculadasQuery : IRequest<List<DtoSalidaMateriaCompaneros>>
    {
        /// <summary>
        /// Constructor de la query
        /// </summary>
        public ObtenerMateriasMatriculadasQuery()
        { }
    }

    /// <summary>
    /// Manejador para obtener las materias matriculadas por el usuario y los companieros de cada materia
    /// </summary>
    public class ObtenerMateriasMatriculadasQueryHandler : IRequestHandler<ObtenerMateriasMatriculadasQuery,
        List<DtoSalidaMateriaCompaneros>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObtenerUsuarioHelper _obtenerUsuarioHelper;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="obtenerUsuarioHelper">Helper para obtener usuario de la sesion</param>
        public ObtenerMateriasMatriculadasQueryHandler(IUnitOfWork unitOfWork, IObtenerUsuarioHelper obtenerUsuarioHelper)
        {
            _unitOfWork = unitOfWork;
            _obtenerUsuarioHelper = obtenerUsuarioHelper;
        }

        /// <summary>
        /// Handle para obtener el listado de materias indicando cuales tiene inscritas el usuario de la sesion
        /// </summary>
        /// <param name="request">query</param>
        /// <param name="cancellationToken">Token de cancelacion</param>
        /// <returns>Lisado de materias</returns>
        public async Task<List<DtoSalidaMateriaCompaneros>> Handle(ObtenerMateriasMatriculadasQuery request, CancellationToken cancellationToken)
        {
            var estudianteId = _obtenerUsuarioHelper.ObtenerEstudianteIdSesion();

            // Todas las materias, se incluyen el EstudianteMateria siempre y cuando sea el del usuario de la sesion y profesor de cada materia
            var materiasProfesores = (await _unitOfWork.Repository<Materias, int>().ObtenerEntidadesIncludesAsync(
                x => x.EstudianteMaterias!.Any(e => e.EstudianteId == estudianteId),
                [
                    q => q.Include(x => x.EstudianteMaterias)!
                        .ThenInclude(e => e.Estudiante)!,
                    q => q.Include(x => x.ProfesorMaterias)!
                        .ThenInclude(p => p.Profesor)!
                ], null, true, cancellationToken)).ToList();

            // Todas las materias con su profesor
            var materias = materiasProfesores.Select(m => new DtoSalidaMateriaCompaneros
            {
                MateriaId = m.MateriaId,
                Nombre = m.Nombre,
                Creditos = m.Creditos,

                Profesor = m.ProfesorMaterias!
                        .Select(pm => pm.Profesor!.Nombre)
                        .FirstOrDefault() ?? string.Empty,

                Matriculada = m.EstudianteMaterias != null && m.EstudianteMaterias.Any(),

                Companeros = m.EstudianteMaterias?
                    .Where(em => em.EstudianteId != estudianteId)
                    .Select(e => e.Estudiante!.Nombre)?.ToList()
            })
            .OrderBy(m => m.Matriculada)
            .ToList();

            return materias;
        }
    }
}
