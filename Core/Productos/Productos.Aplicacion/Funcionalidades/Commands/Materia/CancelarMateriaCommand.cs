namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Materia
{
    using MediatR;
    using RegistroEstudiantesH.Aplicacion.Helpers;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;
    using Transversales.Shared.Excepciones;

    /// <summary>
    /// Command para cancelar una materia
    /// </summary>
    public class CancelarMateriaCommand : IRequest
    {
        public int MateriaId { get; set; }

        /// <summary>
        /// Constructor del command
        /// </summary>
        /// <param name="materiaId">Identificador de la materia a cancelar</param>
        public CancelarMateriaCommand(int materiaId)
        {
            MateriaId = materiaId;
        }
    }

    /// <summary>
    /// Manejador del command para cancelar una materia
    /// </summary>
    public class CancelarMateriaCommandHanlder : IRequestHandler<CancelarMateriaCommand>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IObtenerUsuarioHelper _obtenerUsuarioHelper;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="obtenerUsuarioHelper">Helper para obtener el usuario en sesion</param>
        public CancelarMateriaCommandHanlder(IUnitOfWork unitOfWork, IObtenerUsuarioHelper obtenerUsuarioHelper)
        {
            _unitOfWork = unitOfWork;
            _obtenerUsuarioHelper = obtenerUsuarioHelper;
        }

        /// <summary>
        /// Handle para cancelar una materia
        /// </summary>
        /// <param name="request">Command con la informacion para cancelar la materia</param>
        /// <param name="cancellationToken">Token de cancelacion</param>        
        public async Task Handle(CancelarMateriaCommand request, CancellationToken cancellationToken)
        {
            var estudianteId = _obtenerUsuarioHelper.ObtenerEstudianteIdSesion();

            var inscripcion = await _unitOfWork.Repository<EstudianteMaterias, int>().ObtenerEntidadAsync(
                x => x.EstudianteId == estudianteId && x.MateriaId == request.MateriaId,
                true, cancellationToken
                );

            if (inscripcion == null)
            {
                throw new BadRequestCustomException("El estudiante no está inscrito en la materia.");
            }

            await _unitOfWork.Repository<EstudianteMaterias, int>().EliminarAsync(inscripcion);
            await _unitOfWork.CompleteAsync();
        }
    }
}
