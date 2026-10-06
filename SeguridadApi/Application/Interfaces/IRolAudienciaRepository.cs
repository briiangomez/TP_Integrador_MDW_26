namespace Application.Interfaces;

public interface IRolAudienciaRepository
{
    Task<bool> HasAudiencePermissionAsync(string rol, string audiencia);
    Task<IReadOnlyList<string>> GetAudiencesByRolAsync(string rol);
}
