using Application.DTO;
using Application.Interfaces;

namespace Application.UseCases;

public class EmitirTokenClienteUseCase
{
    private readonly IClienteServicioRepository _clientes;
    private readonly IRolAudienciaRepository _rolAudiencias;
    private readonly IAudienciaRepository _audiencias;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public EmitirTokenClienteUseCase(
        IClienteServicioRepository clientes,
        IRolAudienciaRepository rolAudiencias,
        IAudienciaRepository audiencias,
        IPasswordHasher hasher,
        IJwtTokenGenerator jwt)
    {
        _clientes = clientes;
        _rolAudiencias = rolAudiencias;
        _audiencias = audiencias;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResult> ExecuteAsync(TokenRequest req)
    {
        try
        {
            var errors = req.Validate();
            if (errors.Count > 0)
                return AuthResult.BadRequest(errors);

            var client = await _clientes.GetByClientIdAsync(req.ClientId);

            if (client == null || client.Status != "ACTIVE")
                return AuthResult.Unauthorized();

            if (!_hasher.Verify(req.ClientSecret, client.SecretHash))
                return AuthResult.Unauthorized();

            if (!await _audiencias.ExistsActiveAsync(req.Audience))
                return AuthResult.BadRequest(new[] { $"El audience '{req.Audience}' no existe o no está activo." });

            if (!await _rolAudiencias.HasAudiencePermissionAsync(client.Rol, req.Audience))
                return AuthResult.Forbidden($"El cliente '{client.Id}' no tiene acceso al audience '{req.Audience}'.");

            var token = _jwt.GenerateToken(client.Id, new[] { req.Audience }, client.Rol);
            return AuthResult.Ok(token);
        }
        catch (Exception ex)
        {
            return AuthResult.Error(ex.Message);
        }
    }
}
