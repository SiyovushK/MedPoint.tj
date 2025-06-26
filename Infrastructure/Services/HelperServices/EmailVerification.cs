namespace Infrastructure.Services.HelperServices;

public class EmailVerification
{
    public bool EmailVerificationMethod(string email)
    {
        string[] allowedDomains = {
            "@gmail.com", "@mail.ru", "@yahoo.com", "@yandex.ru",
            "@outlook.com", "@hotmail.com", "@icloud.com",
            "@bk.ru", "@list.ru", "@inbox.ru"
        };
        
        return allowedDomains.Any(d => email.EndsWith(d, StringComparison.OrdinalIgnoreCase));
    }
}