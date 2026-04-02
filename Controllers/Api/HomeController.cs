using AMZN.DTOs.Home;
using AMZN.DTOs.Products;
using AMZN.Services.Home;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AMZN.Controllers.Api
{
    [Route("api/home")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly HomeService _homeService;

        public HomeController(HomeService homeService)
        {
            _homeService = homeService;
        }


        [HttpGet]
        [AllowAnonymous]
        [OutputCache(Duration = 15)]
        public async Task<ActionResult<HomeResponseDto>> GetHome()
        {
            HomeResponseDto dto = await _homeService.GetHomeAsync();
            return Ok(dto);
        }

        [HttpGet("last-viewed")]
        [AllowAnonymous]
        public async Task<ActionResult<List<ProductCardDto>>> GetLastViewed([FromQuery] HomeLastViewedQueryDto query)
        {
            List<ProductCardDto> dto = await _homeService.GetLastViewedAsync(query.ProductIds);
            return Ok(dto);
        }
    }
}