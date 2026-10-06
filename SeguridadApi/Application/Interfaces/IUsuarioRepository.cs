using Domain.Entities;

namespace Application.Interfaces;

public interface IUsuarioRepository
{
    Task<AuthSubject?> GetByUsernameAsync(string username);
}
