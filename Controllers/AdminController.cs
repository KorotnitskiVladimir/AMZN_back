using System.ComponentModel;
using AMZN.Data;
using AMZN.Data.Entities;
using AMZN.Models;
using AMZN.Models.Action;
using AMZN.Models.Category;
using AMZN.Models.User;
using AMZN.Security.Passwords;
using AMZN.Services.Admin;
using AMZN.Services.Storage.Cloud;
using AMZN.Services.Storage.Local;
using Microsoft.AspNetCore.Mvc;
using Azure.Storage.Blobs;

namespace AMZN.Controllers;

public class AdminController : Controller
{
    private readonly DataContext _dataContext;
    private readonly IPasswordHasher _passwordHasher;
    private readonly FormsValidators _formsValidator;
    private readonly DataAccessor _dataAccessor;
    private readonly ILocalsStorageService _localsStorageService;
    private readonly ICloudStorageService _cloudStorageService;
    private readonly AdminCategoryService _adminCategoryService;
    private readonly AdminUserService _adminUserService;
    private readonly AdminActionService _adminActionService;
    
    public AdminController(DataContext dataContext,
        IPasswordHasher passwordHasher,
        FormsValidators formsValidator,
        DataAccessor dataAccessor,
        ILocalsStorageService localsStorageService,
        ICloudStorageService cloudStorageService,
        AdminCategoryService adminCategoryService,
        AdminUserService adminUserService,
        AdminActionService adminActionService)
    {
        _dataContext = dataContext;
        _passwordHasher = passwordHasher;
        _formsValidator = formsValidator;
        _dataAccessor = dataAccessor;
        _localsStorageService = localsStorageService;
        _cloudStorageService = cloudStorageService;
        _adminCategoryService = adminCategoryService;
        _adminUserService = adminUserService;
        _adminActionService = adminActionService;
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
    public async Task<JsonResult> Register(UserSignUpFormModel? model)
    {
        if (model == null)
        {
            return Json(new { success = false, message = "Form model is null" });
        }
        
        (bool success, object message) = await _adminUserService.RegisterUser(model);
        return Json(new { success, message });
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
            Categories = _adminCategoryService.GetAll()
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

        (bool success, object message) = _adminCategoryService.AddCategory(model);
        return Json(new { success, message });
    }

    public FileResult Image([FromRoute] string id)
    {
        return File(System.IO.File.ReadAllBytes(_localsStorageService.GetRealPath(id)), "image/jpeg");
    }
    
    public IActionResult Action()
    {
        ActionViewModel viewModel = new();
        return View(viewModel);
    }

    [HttpPost]
    public JsonResult AddAction(ActionFormModel? model)
    {
        if (model == null)
        {
            return Json(new { success = false, message = "Form model is null" });
        }

        (bool success, object message) = _adminActionService.AddAction(model);
        return Json(new { success, message });
    }
    
}