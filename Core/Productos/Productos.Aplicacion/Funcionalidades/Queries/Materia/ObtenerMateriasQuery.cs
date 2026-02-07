namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Queries.Materia
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using RegistroEstudiantesH.Aplicacion.Dtos.Materia.Salida;
    using RegistroEstudiantesH.Aplicacion.Helpers;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;

    /// <summary>
    /// Query para obtener las materias indicando cuales tiene inscritas
    /// </summary>
    public class ObtenerMateriasQuery : IRequest<List<DtoSalidaMateria>>
    {
        /// <summary>
        /// Constructor de la query
        /// </summary>
        public ObtenerMateriasQuery() { }
    }

    /// <summary>
    /// Manejador de la query para obtener las materias
    /// </summary>
    public class ObtenerMateriasQueryHandler : IRequestHandler<ObtenerMateriasQuery, List<DtoSalidaMateria>>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObtenerUsuarioHelper _obtenerUsuarioHelper;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="obtenerUsuarioHelper">Helper para obtener usuario de la sesion</param>
        public ObtenerMateriasQueryHandler(IUnitOfWork unitOfWork, IObtenerUsuarioHelper obtenerUsuarioHelper)
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
        public async Task<List<DtoSalidaMateria>> Handle(ObtenerMateriasQuery request, CancellationToken cancellationToken)
        {
            var estudianteId = _obtenerUsuarioHelper.ObtenerEstudianteIdSesion();

            // Todas las materias, se incluyen el EstudianteMateria siempre y cuando sea el del usuario de la sesion y profesor de cada materia
            var materiasProfesores = (await _unitOfWork.Repository<Materias, int>().ObtenerEntidadesIncludesAsync(
                null,
                [
                    q => q.Include(x => x.EstudianteMaterias!
                        .Where(e => e.EstudianteId == estudianteId)),
                    q => q.Include(x => x.ProfesorMaterias)!
                        .ThenInclude(p => p.Profesor)!
                ], null, true, cancellationToken)).ToList();

            // Todas las materias con su profesor
            var materias = materiasProfesores.Select(m => new DtoSalidaMateria
            {
                MateriaId = m.MateriaId,
                Nombre = m.Nombre,

                Profesor = m.ProfesorMaterias!
                        .Select(pm => pm.Profesor!.Nombre)
                        .FirstOrDefault() ?? string.Empty,

                Matriculada = m.EstudianteMaterias != null && m.EstudianteMaterias.Any(),
            })
            .OrderBy(m => m.Matriculada)
            .ToList();

            return materias;
        }
    }
}
