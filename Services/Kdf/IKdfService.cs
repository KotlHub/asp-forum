namespace ASP_201.Services.Kdf
{
    public interface IKdfService
    {
        String GetDerivedKey(String password, String salt);
    }
}
