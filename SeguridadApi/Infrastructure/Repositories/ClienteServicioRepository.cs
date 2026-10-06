using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class ClienteServicioRepository : IClienteServicioRepository
{
    private readonly DbConnectionFactory _factory;

    public ClienteServicioRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<AuthSubject?> GetByClientIdAsync(string clientId)
    {
        using var connection = _factory.CreateConnection();

        var sqlClient = @"SELECT client_id AS Id, client_secret_hash AS SecretHash, rol AS Rol, status AS Status 
                          FROM clientes_servicio WHERE client_id = @ClientId";
        return await connection.QuerySingleOrDefaultAsync<AuthSubject>(sqlClient, new { ClientId = clientId });
    }
}
