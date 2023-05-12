using System.Text.RegularExpressions;

namespace ASP_201.Services.Validate
{
    public class ValidationServiceV1 : IValidateService
    {
        public bool Validate(String Source, params ValidationTerms[] terms)
        {
            if (terms.Length == 0)
            {
                throw new ArgumentException("No terms for validation");
            }
            if (terms.Length == 1 && terms[0] == ValidationTerms.None)
            {
                return true;
            }
            bool result = true;
            if (terms.Contains(ValidationTerms.NotEmpty))
            {
                result &= ValidateNotEmpty(Source);
                // result = result && ValidateNotEmpty(source);
            }
            if (terms.Contains(ValidationTerms.Login))
            {
                result &= ValidateLogin(Source);
            }
            if (terms.Contains(ValidationTerms.Email))
            {
                result &= ValidateEmail(Source);
            }
            if (terms.Contains(ValidationTerms.RealName))
            {
                result &= ValidateRealName(Source);
            }
            if (terms.Contains(ValidationTerms.Password))
            {
                result &= ValidatePassword(Source);
            }
            return result;
        }
        private static bool ValidateRegex(String Source, String pattern)
        {
            return Regex.IsMatch(Source, pattern);
        }
        private static bool ValidateLogin(String Source)
        {
            string pattern = @"^\w{3,}$";
            return ValidateRegex(Source, pattern);
        }
        private static bool ValidateEmail(String Source)
        {
            string pattern = @"^[\w.%+-]+@[\w.-]+\.[a-zA-Z]{2,}$";
            return ValidateRegex(Source, pattern);
        }
        private static bool ValidateRealName(String Source)
        {
            string pattern = @"^.+$";
            return ValidateRegex(Source, pattern);
        }
        private static bool ValidatePassword(String Source)
        {
            return Source.Length > 3;
        }
        private static bool ValidateNotEmpty(String Source)
        {
            return !string.IsNullOrEmpty(Source);
        }
    }
}
