using Application.Interfaces;
using Infrastructure.Persistence;
using Infrastructure.Repositories;
using Infrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")!;
        var jwtSettings = new JwtSettings
        {
            Secret = configuration["Jwt:Secret"] ?? "SuperSecretoParaElTpDeArquitectura2026!",
            ExpirationHours = int.Parse(configuration["Jwt:ExpirationHours"] ?? "1")
        };

        services.AddSingleton(new DbConnectionFactory(connectionString));
        services.AddSingleton(jwtSettings);

        services.AddScoped<IClienteServicioRepository, ClienteServicioRepository>();
        services.AddScoped<IUsuarioRepository, UsuarioRepository>();
        services.AddScoped<IRolAudienciaRepository, RolAudienciaRepository>();
        services.AddScoped<IAudienciaRepository, AudienciaRepository>();

        services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
        services.AddSingleton<IJwtTokenGenerator, JwtTokenGenerator>();

        return services;
    }
}
