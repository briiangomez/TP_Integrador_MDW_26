namespace Domain.Entities;

public class AuthSubject
{
    public string Id { get; set; } = string.Empty;
    public string SecretHash { get; set; } = string.Empty;
    public string Rol { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
}
