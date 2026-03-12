using AMZN.DTOs.Brands;
using AMZN.DTOs.Common;
using AMZN.DTOs.Products;
using AMZN.Services.Products;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;

namespace AMZN.Controllers.Api
{
    [Route("api/products")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        public ProductsController(ProductService products) 
        {
            _productService = products;            
        }


        [HttpGet("{id:guid}")]
        [AllowAnonymous]
        public async Task<ActionResult<ProductDetailsDto>> GetById([FromRoute] Guid id)
        {
            var dto = await _productService.GetByIdAsync(id);
            return Ok(dto);
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<PagedResult<ProductCardDto>>> GetCatalogPage([FromQuery] ProductListQueryDto q)
        {
            var dto = await _productService.GetCatalogPageAsync(q);
            return Ok(dto);
        }


        [HttpGet("brands")]
        [AllowAnonymous]
        [OutputCache(Duration = 30, VaryByQueryKeys = new[] { "categoryId" })]
        public async Task<ActionResult<List<BrandDto>>> GetCatalogBrands([FromQuery] Guid? categoryId)
        {
            var dto = await _productService.GetCatalogBrandsAsync(categoryId);
            return Ok(dto);
        }

    }


}
