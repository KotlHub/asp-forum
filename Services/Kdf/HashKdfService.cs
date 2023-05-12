using ASP_201.Services.Hash;

namespace ASP_201.Services.Kdf
{
    public class HashKdfService : IKdfService
    {
        readonly IHashService _hashService;

        public HashKdfService(IHashService hashService)
        {
            _hashService = hashService;
        }

        public string GetDerivedKey(String password, String salt)
        {
            return _hashService.Hash(salt + password);
        }
    }
}
