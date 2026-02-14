using AMZN.Data;
using AMZN.Models.User;

namespace AMZN.Models;

public class FormsValidators
{
    private readonly DataContext _dataContext;
    
    public FormsValidators(DataContext dataContext)
    {
        _dataContext = dataContext;
    }

    public Dictionary<string, string> ValidateUser(UserSignUpFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel != null)
        {
            if (string.IsNullOrEmpty(formModel.FirstName))
            {
                errors.Add("firstName", "First name is required");
            }

            if (!string.IsNullOrEmpty(formModel.FirstName))
            {
                if (formModel.FirstName.Length < 2 || formModel.FirstName.Length > 20)
                {
                    errors.Add("firstName", "First name must be between 2 and 20 characters");
                }
            }
            
            if (string.IsNullOrEmpty(formModel.LastName))
            {
                errors.Add("firstName", "First name is required");
            }

            if (!string.IsNullOrEmpty(formModel.LastName))
            {
                if (formModel.LastName.Length < 2 || formModel.LastName.Length > 20)
                {
                    errors.Add("firstName", "First name must be between 2 and 20 characters");
                }
            }
            
            if (string.IsNullOrEmpty(formModel.Email))
            {
                errors.Add("email", "Email is required");
            }

            if (!string.IsNullOrEmpty(formModel.Email))
            {
                if (_dataContext.Users.FirstOrDefault(u => u.Email == formModel.Email) != null)
                {
                    errors.Add("email", "Email already taken");
                }
            }
            
            if (string.IsNullOrEmpty(formModel.Password))
            {
                errors.Add("password", "password is required");
            }

            if (!(formModel.Password.Length >= 8 && formModel.Password.Length <= 20))
            {
                errors.Add("password", "Password must be between 8 and 20 characters");
            }
            
            if (string.IsNullOrEmpty(formModel.ConfirmPassword))
            {
                errors.Add("passwordConfirm", "password's confirmation is required");
            }

            if (!(formModel.ConfirmPassword.Length >= 8 && formModel.ConfirmPassword.Length <= 20))
            {
                errors.Add("passwordConfirm", "Password must be between 8 and 20 characters");
            }
            
            if (!string.Equals(formModel.Password, formModel.ConfirmPassword))
            {
                errors.Add("passwordConfirm", "Passwords do not match");
            }

            if (string.IsNullOrEmpty(formModel.Role))
            {
                errors.Add("role", "Role is required");
            }

            if (!string.IsNullOrEmpty(formModel.Role))
            {
                if (!string.Equals(formModel.Role, "User") && !string.Equals(formModel.Role, "Admin") && !string.Equals(formModel.Role, "Moderator"))
                {
                    errors.Add("role", "Role is not valid");
                }
            }
        }
        else
        {
            errors.Add("formModel", "Form model is null");
        }
        
        return errors;
    }
}