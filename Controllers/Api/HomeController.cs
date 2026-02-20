using AMZN.DTOs.Home;
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
        private readonly HomeService _home;

        public HomeController(HomeService home)
        {
            _home = home;
        }

        [HttpGet]
        [AllowAnonymous]
        [OutputCache(PolicyName = "HomePage")]
        public async Task<ActionResult<HomeResponseDto>> Get([FromQuery] int take = 20)
        {
            var dto = await _home.GetHomeAsync(take);
            return Ok(dto);
        }
    }
}