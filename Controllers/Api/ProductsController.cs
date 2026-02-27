using AMZN.DTOs.Products;
using AMZN.Services.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AMZN.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _products;

        public ProductsController(ProductService products) 
        {
            _products = products;            
        }


        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDetailsDto>> GetById([FromRoute] Guid id)
        {
            var dto = await _products.GetByIdAsync(id);
            return Ok(dto);
        }



    }


}
