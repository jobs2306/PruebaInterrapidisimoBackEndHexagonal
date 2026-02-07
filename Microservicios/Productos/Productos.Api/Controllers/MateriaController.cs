namespace RegistroEstudiantesH.Api.Controllers
{
    using System.Net;
    using MediatR;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using RegistroEstudiantesH.Aplicacion.Dtos.Materia.Entrada;
    using RegistroEstudiantesH.Aplicacion.Dtos.Materia.Salida;
    using RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Materia;
    using RegistroEstudiantesH.Aplicacion.Funcionalidades.Queries.Materia;
    using Transversales.Shared.Excepciones;
    using Transversales.Shared.Models;
    using Transversales.Shared.Utils;

    /// <summary>
    /// Controlador para operaciones relacionadas a materias
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class MateriaController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa una instancia de <see cref="MateriaController"/>
        /// </summary>
        /// <param name="mediator">El servicio de Mediator para manejar comandos y consultas</param>
        public MateriaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea una inscripcion de un estudiante a una materia
        /// </summary>
        /// <param name="dto">Dto con datos necesarios para crear la inscripcion</param>
        /// <returns> 
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando crea la inscripcion correctamente
        ///   400BadRequest Si no hay un error en la validacion de datos de entrada.
        ///   404NotFound Si no encuentra la materia, el estudiante o el profesor relacionados al registros.
        ///   401Unauthorized Si no está autenticado.
        ///   500InternalServerError Si ocurrio una falla o errror NO controlado 
        /// </returns>
        /// <response code="200">Cuando crea la inscripcion correctamente</response>
        /// <response code="400">Si encuentra un error.</response>
        /// <response code="404">Si no encuentra alguna entidad.</response>
        /// <response code="401">Si no está autenticado.</response>
        /// <response code="500">Si ocurrio una falla o errror NO controlado</response>
        [Authorize]
        [HttpPost("Inscribir/Estudiante")]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> InscribirMateria([FromBody] DtoEntradaInscribirMateria dto)
        {
            try
            {
                await _mediator.Send(new InscribirMateriaCommand(dto));

                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaExitosa("Estudiante inscrito exitosamente a la materia"));
            }
            catch (BadRequestCustomException ex)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(ex.Message, HttpStatusCode.BadRequest));
            }
            catch (NotFoundCustomException ex)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(ex.Message, HttpStatusCode.NotFound));
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida("Error interno del servidor"));
            }
        }

        /// <summary>
        /// Obtiene el listado de materias indicando si el estudiante autenticado está matriculado
        /// </summary>
        /// <returns>
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando retorna el listado correctamente
        ///   401Unauthorized Si no está autenticado
        ///   500InternalServerError Si ocurre un error no controlado
        /// </returns>
        /// <response code="200">Listado de materias</response>
        /// <response code="401">No autenticado</response>
        /// <response code="500">Error interno del servidor</response>
        [Authorize]
        [HttpGet]
        [ProducesResponseType(typeof(ApiRespuesta<List<DtoSalidaMateria>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ObtenerMaterias()
        {
            try
            {
                var result = await _mediator.Send(new ObtenerMateriasQuery());

                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<List<DtoSalidaMateria>>.RespuestaExitosa(result)
                );
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<string>.RespuestaFallida(
                        "Error interno del servidor",
                        HttpStatusCode.InternalServerError
                    )
                );
            }
        }

        /// <summary>
        /// Obtiene las materias matriculadas por el estudiante, incluyendo los nombres de los compañeros de cada clase
        /// </summary>
        /// <returns>
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando retorna el listado correctamente
        ///   401Unauthorized Si no está autenticado
        ///   500InternalServerError Si ocurre un error no controlado
        /// </returns>
        /// <response code="200">Listado de materias con compañeros</response>
        /// <response code="401">No autenticado</response>
        /// <response code="500">Error interno del servidor</response>
        [Authorize]
        [HttpGet("mis-materias")]
        [ProducesResponseType(typeof(ApiRespuesta<List<DtoSalidaMateriaCompaneros>>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> ObtenerMisMateriasConCompaneros()
        {
            try
            {
                var result = await _mediator.Send(new ObtenerMateriasMatriculadasQuery());

                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<List<DtoSalidaMateriaCompaneros>>.RespuestaExitosa(result)
                );
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<string>.RespuestaFallida("Error interno del servidor",
                    HttpStatusCode.InternalServerError
                    )
                );
            }
        }

        /// <summary>
        /// Cancela la inscripción del estudiante autenticado a una materia
        /// </summary>
        /// <param name="materiaId">Identificador de la materia</param>
        /// <returns>
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando la inscripción se cancela correctamente
        ///   401Unauthorized Si no está autenticado
        ///   400BadRequest Si el estudiante no está inscrito en la materia
        ///   500InternalServerError Si ocurre un error no controlado
        /// </returns>
        /// <response code="200">Inscripción cancelada correctamente</response>
        /// <response code="401">No autenticado</response>
        /// <response code="400">No se encuentra inscrito en la materia</response>
        /// <response code="500">Error interno del servidor</response>
        [Authorize]
        [HttpDelete("{materiaId}")]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(object), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.BadRequest)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CancelarInscripcion(int materiaId)
        {
            try
            {
                await _mediator.Send(new CancelarMateriaCommand(materiaId));

                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<string>.RespuestaExitosa("Inscripción cancelada correctamente")
                );
            }
            catch (BadRequestCustomException ex)
            {
                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<string>.RespuestaFallida(ex.Message, HttpStatusCode.BadRequest)
                );
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(
                    ApiRespuestaHttp<string>.RespuestaFallida(
                        "Error interno del servidor",
                        HttpStatusCode.InternalServerError
                    )
                );
            }
        }
    }
}
