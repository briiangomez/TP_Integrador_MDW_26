namespace Application.DTO;

public enum AuthStatus
{
    Success,
    BadRequest,
    Unauthorized,
    Forbidden,
    Error
}

public record AuthResult(
    AuthStatus Status,
    TokenResponse? Response = null,
    string? ErrorMessage = null,
    IReadOnlyList<string>? Errors = null)
{
    public static AuthResult Ok(string token) => new(AuthStatus.Success, new TokenResponse(token));
    public static AuthResult BadRequest(IReadOnlyList<string> errors) => new(AuthStatus.BadRequest, Errors: errors);
    public static AuthResult Unauthorized() => new(AuthStatus.Unauthorized);
    public static AuthResult Forbidden(string message) => new(AuthStatus.Forbidden, ErrorMessage: message);
    public static AuthResult Error(string message) => new(AuthStatus.Error, ErrorMessage: message);
}
