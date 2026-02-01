using AMZN.Data.Entities;
using AMZN.DTOs.Auth;

namespace AMZN.Shared.Mapping
{
    public static class UserMapper
    {
        public static UserResponseDto ToResponseDto(this User user)
        {
            if (user == null)
                throw new ArgumentNullException(nameof(user));


            return new UserResponseDto
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                Role = user.Role.ToString(),
                CreatedAtUnix = new DateTimeOffset(user.CreatedAt).ToUnixTimeSeconds()
            };

        }


    }
}
