public class AuthService
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string plainPassword, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(plainPassword, passwordHash);
    }
}