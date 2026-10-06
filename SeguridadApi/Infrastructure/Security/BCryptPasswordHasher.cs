using Application.Interfaces;

namespace Infrastructure.Security;

public class BCryptPasswordHasher : IPasswordHasher
{
    public bool Verify(string plainText, string hash) => BCrypt.Net.BCrypt.Verify(plainText, hash);
}
