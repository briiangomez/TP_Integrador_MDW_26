using Application.DTO;
using Application.Interfaces;

namespace Application.UseCases;

public class LoginUsuarioUseCase
{
    private readonly IUsuarioRepository _usuarios;
    private readonly IRolAudienciaRepository _rolAudiencias;
    private readonly IPasswordHasher _hasher;
    private readonly IJwtTokenGenerator _jwt;

    public LoginUsuarioUseCase(
        IUsuarioRepository usuarios,
        IRolAudienciaRepository rolAudiencias,
        IPasswordHasher hasher,
        IJwtTokenGenerator jwt)
    {
        _usuarios = usuarios;
        _rolAudiencias = rolAudiencias;
        _hasher = hasher;
        _jwt = jwt;
    }

    public async Task<AuthResult> ExecuteAsync(LoginRequest req)
    {
        try
        {
            var errors = req.Validate();
            if (errors.Count > 0)
                return AuthResult.BadRequest(errors);

            var user = await _usuarios.GetByUsernameAsync(req.Username);

            if (user == null || user.Status != "ACTIVE")
                return AuthResult.Unauthorized();

            if (!_hasher.Verify(req.Password, user.SecretHash))
                return AuthResult.Unauthorized();

            var audiences = await _rolAudiencias.GetAudiencesByRolAsync(user.Rol);
            if (audiences.Count == 0)
                return AuthResult.Forbidden($"El rol '{user.Rol}' no tiene ningún audience habilitado.");

            var token = _jwt.GenerateToken(req.Username, audiences, user.Rol);
            return AuthResult.Ok(token);
        }
        catch (Exception ex)
        {
            return AuthResult.Error(ex.Message);
        }
    }
}
