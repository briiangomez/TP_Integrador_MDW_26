using Application.Interfaces;
using Dapper;
using Domain.Entities;
using Infrastructure.Persistence;

namespace Infrastructure.Repositories;

public class UsuarioRepository : IUsuarioRepository
{
    private readonly DbConnectionFactory _factory;

    public UsuarioRepository(DbConnectionFactory factory)
    {
        _factory = factory;
    }

    public async Task<AuthSubject?> GetByUsernameAsync(string username)
    {
        using var connection = _factory.CreateConnection();

        var sqlUser = @"SELECT id::text AS Id, password_hash AS SecretHash, rol AS Rol, status AS Status 
                        FROM usuarios WHERE username = @Username";
        return await connection.QuerySingleOrDefaultAsync<AuthSubject>(sqlUser, new { Username = username });
    }
}
