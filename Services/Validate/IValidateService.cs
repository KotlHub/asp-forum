namespace ASP_201.Services.Validate
{
    public interface IValidateService
    {
        bool Validate(String Source, params ValidationTerms[] terms);
    }
}
