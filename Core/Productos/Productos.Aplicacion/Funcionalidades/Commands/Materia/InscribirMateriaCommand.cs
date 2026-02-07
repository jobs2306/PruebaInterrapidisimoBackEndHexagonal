namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Materia
{
    using MediatR;
    using Microsoft.EntityFrameworkCore;
    using RegistroEstudiantesH.Aplicacion.Dtos.Materia.Entrada;
    using RegistroEstudiantesH.Aplicacion.Helpers;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;
    using Transversales.Shared.Excepciones;
    using Transversales.Shared.Utils;

    /// <summary>
    /// Command para inscribir una materia
    /// </summary>
    public class InscribirMateriaCommand : IRequest
    {
        public DtoEntradaInscribirMateria Body { get; set; }

        /// <summary>
        /// Constructor del comando
        /// </summary>
        /// <param name="body">Dto con los datos para inscribir una materia</param>
        public InscribirMateriaCommand(DtoEntradaInscribirMateria body)
        {
            Body = body;
        }
    }

    /// <summary>
    /// Manejador del comando para inscribir una materia
    /// </summary>
    public class InscribirMateriaCommandHandler : IRequestHandler<InscribirMateriaCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObtenerUsuarioHelper _obtenerUsuarioHelper;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="obtenerUsuarioHelper">Helper para obtener usuario de la sesion</param>
        public InscribirMateriaCommandHandler(IUnitOfWork unitOfWork, IObtenerUsuarioHelper obtenerUsuarioHelper)
        {
            _unitOfWork = unitOfWork;
            _obtenerUsuarioHelper = obtenerUsuarioHelper;
        }

        /// <summary>
        /// Handle para inscribir una materia
        /// </summary>
        /// <param name="request">Command con informacion para inscribir la materia</param>
        /// <param name="cancellationToken">Token de cancelacion</param>
        public async Task Handle(InscribirMateriaCommand request, CancellationToken cancellationToken)
        {
            var estudianteId = _obtenerUsuarioHelper.ObtenerEstudianteIdSesion();

            // Validar estudiante
            var estudiante = await _unitOfWork.Repository<Estudiantes, int>().ObtenerEntidadIncludesAsync(
                x => x.EstudianteId == estudianteId,
                [
                    q => q.Include(e => e.EstudianteMaterias)!
                ], true, cancellationToken);

            if (estudiante == null)
            {
                throw new NotFoundCustomException("El estudiante no existe.");
            }

            // Validar máximo 3 materias
            if (estudiante.EstudianteMaterias!.Count >= 3)
            {
                throw new BadRequestCustomException("El estudiante ya tiene el máximo de materias permitidas.");
            }

            // Validar materia
            var materia = await _unitOfWork.Repository<Materias, int>().ObtenerEntidadIncludesAsync(
                m => m.MateriaId == request.Body.MateriaId,
                [
                    q => q.Include(m => m.ProfesorMaterias)!
                        .ThenInclude(pm => pm.Profesor)!
                ],
                true, cancellationToken);

            if (materia == null)
            {
                throw new NotFoundCustomException("La materia no existe.");
            }

            // Validar profesor
            var profesor = materia.ProfesorMaterias?.FirstOrDefault()?.Profesor;

            if (materia.ProfesorMaterias == null || materia.ProfesorMaterias.Count != 1)
            {
                throw new BadRequestCustomException("Configuración inválida de la materia.");
            }

            // Validar que no esté inscrito en la misma materia
            bool yaInscrito = estudiante.EstudianteMaterias.Any(em => em.MateriaId == request.Body.MateriaId);

            if (yaInscrito)
            {
                throw new BadRequestCustomException("El estudiante ya está inscrito en esta materia.");
            }

            // Validar que no repita profesor
            bool repiteProfesor = estudiante.EstudianteMaterias.Any(em => em.ProfesorId == profesor.ProfesorId);

            if (repiteProfesor)
            {
                throw new BadRequestCustomException("El estudiante no puede repetir profesor.");
            }

            // Crear inscripción
            var estudianteMateria = new EstudianteMaterias
            {
                EstudianteId = estudianteId,
                MateriaId = request.Body.MateriaId,
                ProfesorId = profesor!.ProfesorId,
                FechaRegistro = ConvertidorZonaHoraria.ObtenerHoraActualColombia()
            };

            await _unitOfWork.Repository<EstudianteMaterias, int>().AgregarEntidadAsync(estudianteMateria);
            await _unitOfWork.CompleteAsync();
        }
    }
}
