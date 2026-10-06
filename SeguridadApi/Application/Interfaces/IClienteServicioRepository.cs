using Domain.Entities;

namespace Application.Interfaces;

public interface IClienteServicioRepository
{
    Task<AuthSubject?> GetByClientIdAsync(string clientId);
}
