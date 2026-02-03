using Swashbuckle.AspNetCore.Filters;

namespace AMZN.DTOs.Auth.SwaggerExamples
{


    // Register
    public class RegisterRequestExample : IExamplesProvider<RegisterRequestDto>
    {
        public RegisterRequestDto GetExamples() => new()
        {
            FirstName   = "Test FirstName",
            LastName    = "Test LastName",
            Email       = "user@example.com",
            Password    = "P@ssw0rd",
            PasswordRepeat = "P@ssw0rd"
        };
    }

    // Login
    public class LoginRequestExample : IExamplesProvider<LoginRequestDto>
    {
        public LoginRequestDto GetExamples() => new()
        {
            Email    = "user@example.com",
            Password = "P@ssw0rd"
        };
    }





}
