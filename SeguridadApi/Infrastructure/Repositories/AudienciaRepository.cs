using Application.Interfaces;
using Dapper;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class AudienciaRepository : IAudienciaRepository
{
    private readonly DbConnectionFactory _factory;

    public AudienciaRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<bool> ExistsActiveAsync(string audiencia)
    {
        using var connection = _factory.CreateConnection();

        var sql = "SELECT 1 FROM audiencias WHERE id = @Audience AND status = 'ACTIVE'";

        var exists = await connection.QuerySingleOrDefaultAsync<int?>(sql, new { Audience = audiencia });
        return exists.HasValue;
    }
}
