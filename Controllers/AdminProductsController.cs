using AMZN.Models.Product;
using AMZN.Services.Admin;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Controllers;


[Route("Admin/Products")]
public class AdminProductsController : Controller
{
    private readonly AdminProductService _adminProductService;

    public AdminProductsController(AdminProductService adminProductService)
    {
        _adminProductService = adminProductService;
    }


    [HttpGet("Create")]
    [AllowAnonymous]
    public async Task<IActionResult> Create()
    {
        if (!(User?.Identity?.IsAuthenticated ?? false))
            return View("NotAuthorized");

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

        try
        {
            await _adminProductService.CreateAsync(form);
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