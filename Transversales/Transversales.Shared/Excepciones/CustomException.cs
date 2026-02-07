namespace Transversales.Shared.Excepciones
{
    using System.Net;

    /// <summary>
    /// Excepción base de la aplicación que incluye un HttpStatusCode.
    /// </summary>
    public abstract class CustomException : Exception
    {
        /// <summary>
        /// Código HTTP asociado a la excepción.
        /// </summary>
        public HttpStatusCode StatusCode { get; }

        protected CustomException(HttpStatusCode statusCode, string message)
            : base(message)
        {
            StatusCode = statusCode;
        }

        protected CustomException(HttpStatusCode statusCode, string message, Exception? innerException)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }

    /// <summary>
    /// Error de validación o datos de entrada inválidos (400).
    /// </summary>
    public class BadRequestCustomException : CustomException
    {
        public BadRequestCustomException(string message)
            : base(HttpStatusCode.BadRequest, message)
        {
        }

        public BadRequestCustomException(string message, Exception? innerException)
            : base(HttpStatusCode.BadRequest, message, innerException)
        {
        }
    }

    /// <summary>
    /// Recurso no encontrado (404).
    /// </summary>
    public class NotFoundCustomException : CustomException
    {
        public NotFoundCustomException(string message)
            : base(HttpStatusCode.NotFound, message)
        {
        }

        public NotFoundCustomException(string message, Exception? innerException)
            : base(HttpStatusCode.NotFound, message, innerException)
        {
        }
    }

    /// <summary>
    /// No autorizado (401).
    /// </summary>
    public class UnauthorizedCustomException : CustomException
    {
        public UnauthorizedCustomException(string message)
            : base(HttpStatusCode.Unauthorized, message)
        {
        }

        public UnauthorizedCustomException(string message, Exception? innerException)
            : base(HttpStatusCode.Unauthorized, message, innerException)
        {
        }
    }

    /// <summary>
    /// Operación no permitida por conflicto de estado (409).
    /// </summary>
    public class ConflictCustomException : CustomException
    {
        public ConflictCustomException(string message)
            : base(HttpStatusCode.Conflict, message)
        {
        }

        public ConflictCustomException(string message, Exception? innerException)
            : base(HttpStatusCode.Conflict, message, innerException)
        {
        }
    }
}
