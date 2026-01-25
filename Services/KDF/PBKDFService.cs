namespace AMZN.Services.KDF;

public class PBKDFService : IKDFService
{
    private int _iterationCount = 3;
    private int _keyLength = 20;

    public string DerivedKey(string password, string salt)
    {
        string t = password + salt;
        for (int i = 0; i < _iterationCount; i++)
            t = Hash(t);

        return t[.._keyLength];
    }
    

    private string Hash(string input)
    {
        return Convert.ToHexString(
            System.Security.Cryptography.SHA1.HashData(
                System.Text.Encoding.UTF8.GetBytes(input)));
    }
}