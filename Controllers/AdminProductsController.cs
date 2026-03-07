using AMZN.Models.Product;
using AMZN.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AMZN.Controllers;


[Route("Admin/Products")]
public class AdminProductsController : Controller
{
    private readonly AdminProductService _adminProductService;
    private readonly ILogger<AdminProductsController> _logger;

    public AdminProductsController(AdminProductService adminProductService, ILogger<AdminProductsController> logger)
    {
        _adminProductService = adminProductService;
        _logger = logger;
    }


    [HttpGet("Create")]
    [Authorize]
    public async Task<IActionResult> Create()
    {
        var vm = await _adminProductService.BuildCreateVmAsync();
        return View(vm);
    }


    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public async Task<IActionResult> Create(ProductCreateFormModel form)
    {
        if (!ModelState.IsValid)
        {
            var vmInvalid = await _adminProductService.BuildCreateVmAsync(form);
            return View(vmInvalid);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (userId == null || !Guid.TryParse(userId, out var sellerId))
        {
            _logger.LogError("Auth claims NameIdentifier missing/invalid. TraceId={TraceId}, Path={Path}",
                HttpContext.TraceIdentifier,
                HttpContext.Request.Path);

            return Forbid();
        }

        try
        {
            await _adminProductService.CreateAsync(form, sellerId);
            TempData["Success"] = "Product created";
            return RedirectToAction(nameof(Create));
        }
        catch (InvalidOperationException ex)
        {
            var vmError = await _adminProductService.BuildCreateVmAsync(form, ex.Message);
            return View(vmError);
        }
    }


}