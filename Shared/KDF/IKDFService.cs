namespace AMZN.Services.KDF;

public interface IKDFService
{
    string DerivedKey(string password, string salt);
}