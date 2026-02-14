using Microsoft.AspNetCore.Mvc;

namespace AMZN.Models.User;

public class UserSignUpFormModel
{
    [FromForm(Name = "firstName")]
    public string FirstName { get; set; } = null!;
    
    [FromForm(Name = "lastName")]
    public string LastName { get; set; } = null!;
    
    [FromForm(Name = "email")]
    public string Email { get; set; } = null!;
    
    [FromForm(Name = "password")]
    public string Password { get; set; } = null!;
    
    [FromForm(Name = "confirmPassword")]
    public string ConfirmPassword { get; set; } = null!;

    [FromForm(Name = "role")]
    public string Role { get; set; } = null!;
}