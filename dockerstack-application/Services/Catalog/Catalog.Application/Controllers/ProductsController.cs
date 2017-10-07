using Catalog.Application.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class ProductsController : Controller
    {
        private readonly ICatalogRepository _catalogRepository;
        
        public ProductsController(ICatalogRepository catalogRepository)
        {
            _catalogRepository = catalogRepository;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_catalogRepository.GetAllProducts());
        }
    }
}