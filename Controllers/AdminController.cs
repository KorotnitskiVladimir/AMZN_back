using System.ComponentModel;
using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.Models;
using AMZN.Models.Category;
using AMZN.Models.User;
using AMZN.Security.Passwords;
using AMZN.Services.Storage.Local;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Controllers;

public class AdminController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly FormsValidators _formsValidator;
    private readonly DataAccessor _dataAccessor;
    private readonly ILocalsStorageService _localsStorageService;
    
    public AdminController(DataContext dataContext,
        IPasswordHasher passwordHasher,
        FormsValidators formsValidator,
        DataAccessor dataAccessor,
        ILocalsStorageService localsStorageService)
    {
        _dataContext = dataContext;
        _passwordHasher = passwordHasher;
        _formsValidator = formsValidator;
        _dataAccessor = dataAccessor;
        _localsStorageService = localsStorageService;
    }

    public IActionResult SignUp()
    {
        UserSignUpViewModel viewModel = new()
        {
            User = new()
        };

        return View(viewModel);
    }

    [HttpPost]
    public JsonResult Register(UserSignUpFormModel? model)
    {
        if (model == null)
        {
            return Json(new { success = false, message = "Form model is null" });
        }
        
        Dictionary<string, string> errors = _formsValidator.ValidateUser(model);
        if (errors.Count == 0)
        {
            Guid userId = Guid.NewGuid();
            UserRole userRole = UserRole.User;
            switch (model.Role)
            {
                case "User": userRole = UserRole.User; break;
                case "Admin": userRole = UserRole.Admin; break;
                case "Moderator": userRole = UserRole.Moderator; break;
                default: ; break;
            }


            User user = new()
            {
                Id = userId,
                FirstName = model.FirstName.Trim(),
                LastName = model.LastName.Trim(),
                Email = model.Email,
                PasswordHash = _passwordHasher.HashPassword(model.Password),
                Role = userRole,
                CreatedAt = DateTime.UtcNow,
            };
            _dataContext.Users.Add(user);
            _dataContext.SaveChanges();
            return Json(new { success = true, message = "User registered successfully" });
        }
        else
        {
            return Json(new { success = false, message = errors.Values });
        }
    }
    
    public IActionResult Login()
    {
        UserRefreshToken token;
        try
        {
            token = _dataAccessor.Authenticate(Request);
        }
        catch (Win32Exception e)
        {
            return Json(new { status = e.ErrorCode, message = e.Message });
        }
        
        HttpContext.Session.SetString("userId", token.UserId.ToString());
        return Json(new { status = 200, message = "OK" });
    }

    public IActionResult Category()
    {
        CategoryViewModel viewModel = new()
        {
            Categories = new()
        };
        return View(viewModel);
    }
    
    [HttpPost]
    public JsonResult AddCategory(CategoryFormModel? model)
    {
        if (model == null)
        {
            return Json(new { success = false, message = "Form model is null" });
        }
        
        Dictionary<string, string> errors = _formsValidator.ValidateCategory(model);
        if (errors.Count == 0)
        {
            Guid id = Guid.NewGuid();
            var parent = _dataContext.Categories.FirstOrDefault(c => c.Id.ToString() == model.ParentCategory);
            Guid? parentId = null;
            if (parent != null)
            {
                parentId = parent.Id;
            }
            Category category = new()
            {
                Id = id,
                Name = model.Name,
                Description = model.Description,
                ImageUrl = _localsStorageService.SaveFile(model.Image),
                CreatedAt = DateTime.UtcNow,
            };
            if (parent != null)
            {
                category.ParentId = parentId;
                category.ParentCategory = parent;
            }
            _dataContext.Categories.Add(category);
            _dataContext.SaveChanges();
            return Json(new { success = true, message = "Category added successfully" });
        }
        return Json(new { success = false, message = errors.Values });
    }
}