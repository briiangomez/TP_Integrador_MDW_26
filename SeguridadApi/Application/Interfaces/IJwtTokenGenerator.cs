namespace Application.Interfaces;

public interface IJwtTokenGenerator
{
    // Un audience => "aud": "x" | Varios => "aud": ["x", "y"] (RFC 7519 §4.1.3)
    string GenerateToken(string subject, IEnumerable<string> audiences, string role);
}
