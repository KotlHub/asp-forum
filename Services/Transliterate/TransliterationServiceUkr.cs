namespace ASP_201.Services.Transliterate
{
    public class TransliterationServiceUkr : ITransliterationServiceUkr
    {
        private readonly string _ukrainianSimple = "АБВГҐДЕЗИІКЛМНОПРСТУФабвгґдезиклмнопрстуф";
        private readonly string _englishSimple = "ABVHGDEZYIKLMNOPRSTUFabvhgdezyklmnoprstuf";
        public string Transliterate(string source)
        {
            string result = source;



            result = result.Replace("зг", "zgh").Replace("Зг", "Zgh");
            result = result.Replace("ь", "").Replace("Ь", "").Replace("\'", "");



            for (int i = 0; i < _ukrainianSimple.Length; i++)
            {
                result = result.Replace(_ukrainianSimple[i], _englishSimple[i]);
            }



            result = result.Replace("Ш", "Sh").Replace("ш", "sh");
            result = result.Replace("Х", "Kh").Replace("х", "kh");
            result = result.Replace("Ц", "Ts").Replace("ц", "ts");
            result = result.Replace("Ч", "Ch").Replace("ч", "Ch");
            result = result.Replace("Щ", "Shch").Replace("щ", "Shch");



            result = result.Replace(" Ї", " Yi").Replace(" ї", " yi");
            result = result.Replace("Ї", "I").Replace("ї", "i");



            result = result.Replace(" Й", " Y").Replace(" й", " i");
            result = result.Replace("Й", "Y").Replace("й", "i");



            result = result.Replace(" Є", " Ye").Replace(" є", " ye");
            result = result.Replace("Є", "Ie").Replace("є", "ie");



            result = result.Replace(" Ю", " Yu").Replace(" ю", " yu");
            result = result.Replace("Ю", "Iu").Replace("ю", "iu");



            result = result.Replace(" Я", " Ya").Replace(" я", " ya");
            result = result.Replace("Я", "Ia").Replace("я", "ia");
            result = result.Replace(' ', '-').Replace("+", "plus");
            result = result.Replace(' ', '-');



            return result;
        }
    }
}
