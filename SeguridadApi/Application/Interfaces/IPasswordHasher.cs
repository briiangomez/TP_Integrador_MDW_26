namespace Application.Interfaces;

public interface IPasswordHasher
{
    bool Verify(string plainText, string hash);
}
