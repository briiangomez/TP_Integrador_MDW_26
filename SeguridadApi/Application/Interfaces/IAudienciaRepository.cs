namespace Application.Interfaces;

public interface IAudienciaRepository
{
    // true si el audience existe y está ACTIVE
    Task<bool> ExistsActiveAsync(string audiencia);
}
