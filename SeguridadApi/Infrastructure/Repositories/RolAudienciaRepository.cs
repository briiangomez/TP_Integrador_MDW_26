using Application.Interfaces;
using Dapper;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

// Verifica los permisos en la tabla unificada rol_audiencias
public class RolAudienciaRepository : IRolAudienciaRepository
{
    private readonly DbConnectionFactory _factory;

    public RolAudienciaRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> HasAudiencePermissionAsync(string rol, string audiencia)
    {
        using var connection = _factory.CreateConnection();

        var sql = @"SELECT 1 FROM rol_audiencias ra 
                    JOIN audiencias a ON a.id = ra.audiencia_id 
                    WHERE ra.rol = @Rol AND ra.audiencia_id = @Audience AND a.status = 'ACTIVE'";

        var hasPermission = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { Rol = rol, Audience = audiencia });
        return hasPermission.HasValue;
    }

    public async Task<IReadOnlyList<string>> GetAudiencesByRolAsync(string rol)
    {
        using var connection = _factory.CreateConnection();

        var sql = @"SELECT ra.audiencia_id FROM rol_audiencias ra 
                    JOIN audiencias a ON a.id = ra.audiencia_id 
                    WHERE ra.rol = @Rol AND a.status = 'ACTIVE'
                    ORDER BY ra.audiencia_id";

        var audiencias = await connection.QueryAsync<string>(sql, new { Rol = rol });
        return audiencias.ToList();
    }
}
