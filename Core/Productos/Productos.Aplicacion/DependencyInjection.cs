namespace RegistroEstudiantesH.Aplicacion
{
    using Microsoft.Extensions.DependencyInjection;
    using RegistroEstudiantesH.Aplicacion.Helpers;

    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IObtenerUsuarioHelper, ObtenerUsuarioHelper>();

            return services;
        }
    }
}
