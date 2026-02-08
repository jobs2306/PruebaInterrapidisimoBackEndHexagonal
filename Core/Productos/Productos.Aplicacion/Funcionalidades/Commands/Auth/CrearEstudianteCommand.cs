namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Auth
{
    using System.Text.RegularExpressions;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using RegistroEstudiantesH.Aplicacion.Dtos.Login.Entrada;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;
    using Transversales.Shared.Excepciones;
    using Transversales.Shared.Utils;

    /// <summary>
    /// Command para crear un estudiante
    /// </summary>
    public class CrearEstudianteCommand : IRequest
    {
        public DtoRegistrar Body { get; set; }

        /// <summary>
        /// Constructor del command
        /// </summary>
        /// <param name="body">Dto con los datos para crear al estudiante</param>
        public CrearEstudianteCommand(DtoRegistrar body)
        {
            Body = body;
        }
    }

    /// <summary>
    /// Manejador del command para crear un estudiante
    /// </summary>
    public class CrearEstudianteCommandHandler : IRequestHandler<CrearEstudianteCommand>
    {
        private readonly IUnitOfWork _unitOfWork;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        public CrearEstudianteCommandHandler(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        /// <summary>
        /// Handle para crear un estudiante
        /// </summary>
        /// <param name="request">Command con la informacion para crear al estudiante</param>
        /// <param name="cancellationToken">Token de cancelacion</param>
        public async Task Handle(CrearEstudianteCommand request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Body.Email))
            {
                throw new BadRequestCustomException("El email es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(request.Body.Nombre))
            {
                throw new BadRequestCustomException("El nombre es obligatorio");
            }

            if (!EsEmailValido(request.Body.Email))
            {
                throw new BadRequestCustomException("El email no tiene un formato válido.");
            }

            if (string.IsNullOrWhiteSpace(request.Body.Password))
            {
                throw new BadRequestCustomException("La contraseña es obligatoria.");
            }

            if (!EsPasswordValido(request.Body.Password))
            {
                throw new BadRequestCustomException("La contraseña debe cumplir los siguiente requisitos: \n " +
                    "Debe tener de 8 a 16 caracteres. \n " +
                    "Debe tener al menos una letra, un número y un simbolo");
            }

            var existeEmail = await _unitOfWork.Repository<Estudiantes, int>().ObtenerEntidadAsync(
                e => e.Email == request.Body.Email,
                true, cancellationToken
                );

            if (existeEmail != null)
            {
                throw new BadRequestCustomException("El email ya se encuentra registrado.");
            }

            var passwordHasher = new PasswordHasher<Estudiantes>();
            var passwordHash = passwordHasher.HashPassword(null!, request.Body.Password);

            var estudiante = new Estudiantes
            {
                Email = request.Body.Email,
                Nombre = request.Body.Nombre,
                PasswordHash = passwordHash,
                FechaRegistro = ConvertidorZonaHoraria.ObtenerHoraActualColombia()
            };

            await _unitOfWork.Repository<Estudiantes, int>().AgregarEntidadAsync(estudiante);
            await _unitOfWork.CompleteAsync();
        }

        /// <summary>
        /// Verifica si el email es valido o no
        /// </summary>
        /// <param name="email">email a verificar</param>
        /// <returns>Si es valido retorna true, en caso contrario retorna false</returns>
        private static bool EsEmailValido(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Metodo para validar que una contrasenia es valida
        /// </summary>
        /// <param name="password">Contrasenia a validar</param>
        /// <returns>Retorna true si es valida, en caso contrario retornar false</returns>
        private bool EsPasswordValido(string password)
        {
            var regex = new Regex(@"^(?=.*[A-Za-z])(?=.*\d)(?=.*[^A-Za-z0-9]).{8,16}$");
            return regex.IsMatch(password);
        }
    }
}
