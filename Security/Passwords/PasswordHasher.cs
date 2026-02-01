namespace AMZN.Security.Passwords
{
    // Install-Package BCrypt.Net-Next

    public class PasswordHasher : IPasswordHasher
    {
        private const int WorkFactor = 10;       // WF - параметр слонжости,  больше = медленнее хеширование, но устойчивее к брутфорсу. при желании вынести в сеттингс

        public string HashPassword(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password, workFactor: WorkFactor);
        }

        public bool VerifyPassword(string password, string passwordHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, passwordHash);
        }
    }
}
