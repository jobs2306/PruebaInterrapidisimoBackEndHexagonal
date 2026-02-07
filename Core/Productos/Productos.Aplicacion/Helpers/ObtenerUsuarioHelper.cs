namespace RegistroEstudiantesH.Aplicacion.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Transversales.Shared.Excepciones;

    /// <summary>
    /// Interfaz para obtener informacion del usuario en sesion
    /// </summary>
    public interface IObtenerUsuarioHelper
    {
        int ObtenerEstudianteIdSesion();
    }

    public class ObtenerUsuarioHelper : IObtenerUsuarioHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        /// <summary>
        /// Constructor de la clase ObtenerUsuarioHelper.
        /// </summary>
        /// <param name="httpContextAccessor">Contexto http de la sesion</param>
        public ObtenerUsuarioHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public int ObtenerEstudianteIdSesion()
        {
            var user = _httpContextAccessor.HttpContext?.User;

            if (user == null || !user.Identity!.IsAuthenticated)
            {
                throw new UnauthorizedCustomException("Usuario no autenticado.");
            }

            var claim = user.FindFirst("EstudianteId");

            if (claim == null)
            {
                throw new UnauthorizedCustomException("No se pudo obtener el estudiante de la sesion.");
            }

            return int.Parse(claim.Value);
        }
    }
}
