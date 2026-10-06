namespace Application.DTO;

public record TokenRequest(string ClientId, string ClientSecret, string Audience)
{
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(ClientId))
            errors.Add("El campo clientId es obligatorio.");

        if (string.IsNullOrWhiteSpace(ClientSecret))
            errors.Add("El campo clientSecret es obligatorio.");

        if (string.IsNullOrWhiteSpace(Audience))
            errors.Add("El campo audience es obligatorio.");

        return errors;
    }
}
