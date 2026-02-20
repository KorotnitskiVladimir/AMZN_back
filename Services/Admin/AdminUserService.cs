using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.Models;
using AMZN.Models.User;
using AMZN.Repositories.Users;
using AMZN.Security.Passwords;
using Microsoft.AspNetCore.Identity;

namespace AMZN.Services.Admin;

public class AdminUserService
{
    private readonly FormsValidators _formsValidator;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public AdminUserService(FormsValidators formsValidator, 
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _formsValidator = formsValidator;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    public async Task<(bool success, object message)> RegisterUser(UserSignUpFormModel formModel)
    {
        Dictionary<string, string> errors = _formsValidator.ValidateUser(formModel);
        if (errors.Count == 0)
        {
            Guid userId = Guid.NewGuid();
            UserRole userRole = UserRole.User;
            switch (formModel.Role)
            {
                case "User": userRole = UserRole.User; break;
                case "Admin": userRole = UserRole.Admin; break;
                case "Moderator": userRole = UserRole.Moderator; break;
                default: ; break;
            }

            User user = new()
            {
                Id = userId,
                FirstName = formModel.FirstName.Trim(),
                LastName = formModel.LastName.Trim(),
                Email = formModel.Email,
                PasswordHash = _passwordHasher.HashPassword(formModel.Password),
                Role = userRole,
                CreatedAt = DateTime.UtcNow,
            };

            await _userRepository.AddUserAsync(user);
            return (true, "User registered successfully");
        }
        return (false, errors.Values);
    }
}

