using AMZN.Data;
using AMZN.Models.Category;
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

    public Dictionary<string, string> ValidateCategory(CategoryFormModel? formModel)
    {
        Dictionary<string, string> errors = new();
        if (formModel != null)
        {
            if (string.IsNullOrEmpty(formModel.Name))
            {
                errors.Add("name", "Name is required");
            }
            
            if (!string.IsNullOrEmpty(formModel.Name))
            {
                if (formModel.Name.Length < 2 || formModel.Name.Length > 20)
                {
                    errors.Add("name", "Name must be between 2 and 20 characters");
                }
                if (_dataContext.Categories.FirstOrDefault(c => c.Name == formModel.Name) != null)
                {
                    errors.Add("name", "Name already taken");
                }
            }

            if (string.IsNullOrEmpty(formModel.Description))
            {
                errors.Add("description", "Description is required");
            }
            
            if (!string.IsNullOrEmpty(formModel.Description))
            {
                if (formModel.Description.Length < 2 || formModel.Description.Length > 100)
                {
                    errors.Add("description", "Description must be between 2 and 100 characters");
                }
            }

            if (!string.IsNullOrEmpty(formModel.ParentCategory))
            {
                if (_dataContext.Categories.FirstOrDefault(c => c.Id.ToString() == formModel.ParentCategory) == null)
                {
                    errors.Add("parentCategory", "Parent category does not exist");
                }
            }

            if (formModel.Image == null || formModel.Image.Length == 0)
            {
                errors.Add("image", "Image is required");
            }
        }
        else
        {
            errors.Add("formModel", "Form model is null");
        }
        
        return errors;
    }
    
}