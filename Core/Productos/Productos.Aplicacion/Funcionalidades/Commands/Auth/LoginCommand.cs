namespace RegistroEstudiantesH.Aplicacion.Funcionalidades.Commands.Auth
{
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Text;
    using MediatR;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.Extensions.Configuration;
    using Microsoft.IdentityModel.Tokens;
    using RegistroEstudiantesH.Aplicacion.Dtos.Login.Entrada;
    using RegistroEstudiantesH.Aplicacion.Dtos.Login.Salida;
    using RegistroEstudiantesH.Dominio.Entidades;
    using Transversales.Repositorio.Interfaces;
    using Transversales.Shared.Excepciones;

    /// <summary>
    /// Command para iniciar sesion
    /// </summary>
    public class LoginCommand : IRequest<DtoRespuestaAuth>
    {
        public DtoLogin Body { get; set; }

        /// <summary>
        /// Constructor del comando
        /// </summary>
        /// <param name="body">Dto con los datos para inciar sesion</param>
        public LoginCommand(DtoLogin body)
        {
            Body = body;
        }
    }

    /// <summary>
    /// Manejadro del comando para iniciar sesion
    /// </summary>
    public class LoginCommandHandler : IRequestHandler<LoginCommand, DtoRespuestaAuth>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Constructor del manejador
        /// </summary>
        /// <param name="unitOfWork">Unidad de trabajo</param>
        /// <param name="configuration">Configuracion para jwt</param>
        public LoginCommandHandler(IUnitOfWork unitOfWork,
            IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _configuration = configuration;
        }

        /// <summary>
        /// Handle para iniciar sesion
        /// </summary>
        /// <param name="request">Command con los datos para iniciar la sesion</param>
        /// <param name="cancellationToken">Token de cancelacion</param>
        /// <returns>Retorna dto con el token, nombre y fecha de expiracion de la sesion del usuario</returns>
        public async Task<DtoRespuestaAuth> Handle(LoginCommand request, CancellationToken cancellationToken)
        {
            PasswordHasher<object> _passwordHasher = new();

            var estudiante = await _unitOfWork.Repository<Estudiantes, int>().ObtenerEntidadAsync(
                x => x.Email == request.Body.Email, true, cancellationToken
                );

            if (estudiante == null)
            {
                throw new UnauthorizedCustomException("Credenciales inválidas.");
            }

            var result = _passwordHasher.VerifyHashedPassword(
                null!,
                estudiante.PasswordHash,
                request.Body.Password);

            if (result == PasswordVerificationResult.Failed)
            {
                throw new UnauthorizedCustomException("Credenciales inválidas.");
            }

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, estudiante.EstudianteId.ToString()),
                new Claim(JwtRegisteredClaimNames.Email, estudiante.Email),
                new Claim("EstudianteId", estudiante.EstudianteId.ToString())
            };

            var expireMinutes = int.Parse(_configuration["Jwt:ExpireMinutes"]!);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expireMinutes),
                signingCredentials: creds);

            return new DtoRespuestaAuth
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Nombre = estudiante.Nombre,
                Expira = token.ValidTo
            };

        }
    }
}
