namespace ASP_201.Services.Random
{
    public class RandomServiceV1 : IRandomService
    {
        const string _codeChars = "0123456789abcdefghijklmnopqrstuvwxyz";
        String _safeChars = new String(Enumerable.Range(28, 107).Select(x => (char)x).ToArray());
        readonly System.Random _random = new();
        public string ConfirmCode(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _codeChars[_random.Next(_codeChars.Length)];
            }
            return new String(chars);
        }

        public string RandomString(int length)
        {
            char[] chars = new char[length];
            for (int i = 0; i < length; i++)
            {
                chars[i] = _safeChars[_random.Next(_safeChars.Length)];
            }
            return new String(chars);
        }
    }
}
