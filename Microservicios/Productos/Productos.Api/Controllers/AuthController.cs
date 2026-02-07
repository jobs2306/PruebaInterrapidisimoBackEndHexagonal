namespace RegistroEstudiantesH.Api.Controllers
{
    using System.Net;
    using MediatR;
    using Microsoft.AspNetCore.Mvc;
    using RegistroEstudiantesH.Aplicacion.Dtos.Login.Entrada;
    using RegistroEstudiantesH.Aplicacion.Dtos.Login.Salida;
    using RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Auth;
    using Transversales.Shared.Excepciones;
    using Transversales.Shared.Models;
    using Transversales.Shared.Utils;

    /// <summary>
    /// Controlador para autorizacion
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        /// <summary>
        /// Inicializa una instancia de <see cref="ControllerBase"/>
        /// </summary>
        /// <param name="mediator">El servicio de Mediator para manejar comandos y consultas</param>
        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Crea a un usuario
        /// </summary>
        /// <param name="dto">Dto con datos necesarios para crear al usuario</param>
        /// <returns> 
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando crea al usuario
        ///   500InternalServerError Si ocurrio una falla o errror NO controlado 
        /// </returns>
        /// <response code="200">Cuando inicia sesión correctamente</response>
        /// <response code="500">Si ocurrio una falla o errror NO controlado</response>
        [HttpPost("Registrar")]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Registrar([FromBody] DtoRegistrar dto)
        {
            try
            {
                await _mediator.Send(new CrearEstudianteCommand(dto));
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaExitosa("Usuario registrado exitosamente"));
            }
            catch (BadRequestCustomException ex)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(ex.Message,
                        HttpStatusCode.BadRequest
                ));
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(
                        "Error interno del servidor",
                        HttpStatusCode.InternalServerError
                ));
            }
        }

        /// <summary>
        /// Inicia la sesion de un usuario
        /// </summary>
        /// <param name="dto">Dto con datos necesarios para iniciar sesion</param>
        /// <returns> 
        /// Retorna una ApiRespuesta con StatusCodes
        ///   200OK Cuando crea inicia sesion
        ///   401Unauthorized Si no inicia la sesion.
        ///   500InternalServerError Si ocurrio una falla o errror NO controlado 
        /// </returns>
        /// <response code="200">Cuando inicia sesión correctamente</response>
        /// <response code="401">Si no está autenticado.</response>
        /// <response code="500">Si ocurrio una falla o errror NO controlado</response>
        [HttpPost("login")]
        [ProducesResponseType(typeof(ApiRespuesta<DtoRespuestaAuth>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(string), (int)HttpStatusCode.Unauthorized)]
        [ProducesResponseType(typeof(ApiRespuesta<string>), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> Login([FromBody] DtoLogin dto)
        {
            try
            {
                var result = await _mediator.Send(new LoginCommand(dto));
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<DtoRespuestaAuth>.RespuestaExitosa(result));
            }
            catch (UnauthorizedCustomException ex)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(ex.Message, HttpStatusCode.Unauthorized));
            }
            catch (Exception)
            {
                return ApiRespuestaUtil.Convertir(ApiRespuestaHttp<string>.RespuestaFallida(
                        "Error interno del servidor",
                        HttpStatusCode.InternalServerError
                ));
            }
        }
    }
}
