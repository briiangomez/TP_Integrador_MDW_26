namespace Application.DTO;

public record LoginRequest(string Username, string Password)
{
    public List<string> Validate()
    {
        var errors = new List<string>();

        if (string.IsNullOrWhiteSpace(Username))
            errors.Add("El campo username es obligatorio.");

        if (string.IsNullOrWhiteSpace(Password))
            errors.Add("El campo password es obligatorio.");

        return errors;
    }
}
