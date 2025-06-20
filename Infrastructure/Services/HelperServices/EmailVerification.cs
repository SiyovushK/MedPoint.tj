namespace Infrastructure.Services.HelperServices;

public class EmailVerification
{
    public bool EmailVerificationMethod(string email)
    {
        string[] allowedDomains = { "@gmail.com", "@mail.ru" };
        return allowedDomains.Any(d => email.EndsWith(d, StringComparison.OrdinalIgnoreCase));
    }
}