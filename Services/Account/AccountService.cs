using AMZN.DTOs.Auth;
using AMZN.Repositories.Users;
using AMZN.Security.Passwords;
using AMZN.Services.Auth;
using AMZN.Shared.Exceptions;
using AMZN.Shared.Exceptions.Errors;

namespace AMZN.Services.Account;

public class AccountService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    
    public AccountService(
        IUserRepository userRepository,
        ILogger<AuthService> logger,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
    }

    
    public async Task<ProfileUpdateResponseDto> UpdateProfileAsync(ProfileUpdateRequestDto dto, string userId)
    {
        var user = _userRepository.GetUserByIdAsync(userId).Result;
        if (user == null)
        {
            throw new ApiException(ErrorCodes.UserNotFound, "User not found", StatusCodes.Status404NotFound);
        }
        string email = dto.Email.Trim().ToLowerInvariant();
        if (email != user.Email)
        {
            if (await _userRepository.IsEmailTakenAsync(email))
            {
                throw new ApiException(ErrorCodes.AuthEmailTaken, "Email already taken",
                    StatusCodes.Status409Conflict);
            }
            user.Email = email;
        }
        if (!string.IsNullOrEmpty(dto.FirstName)) user.FirstName = dto.FirstName.Trim();
        if (!string.IsNullOrEmpty(dto.LastName)) user.LastName = dto.LastName.Trim();
        if (!string.IsNullOrEmpty(dto.Password)) user.PasswordHash = _passwordHasher.HashPassword(dto.Password);
        if (!string.IsNullOrEmpty(dto.PhoneNumber)) user.PhoneNumber = dto.PhoneNumber;
        if (dto.BirthDate != null)
        {
            if (CheckUserAge(dto.BirthDate) < 18)
            {
                throw new ApiException(ErrorCodes.ValidationError, "User must be at least 18 years old",
                    StatusCodes.Status403Forbidden);
            }
            user.BirthDate = dto.BirthDate;
        }

        await _userRepository.UpdateUserAsync(user);
        return new ProfileUpdateResponseDto()
        {
            Id = user.Id,
            FirstName = user.FirstName,
            LastName = user.LastName,
            Email = user.Email,
            Role = user.Role.ToString(),
            PhoneNumber = user.PhoneNumber,
            BirthDate = user.BirthDate,
        };
    }
    
    
    public int CheckUserAge(DateTime? birthDate)
    {
        if (birthDate != null)
        {
            int age = DateTime.Now.Year - birthDate.Value.Year;
            if (DateTime.Now.Month < birthDate.Value.Month ||
                (DateTime.Now.Month == birthDate.Value.Month && DateTime.Now.Day < birthDate.Value.Day))
            {
                age--;
            }
            return age;
        }
        return 0;
    }
}