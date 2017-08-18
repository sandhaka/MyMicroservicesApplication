using Basket.Application.Infrastructure.Repositories;
using Basket.Application.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _basketRepository;
        
        public BasketController(IBasketRepository basketRepository)
        {
            _basketRepository = basketRepository;
        }

        [HttpGet("{id}")]
        public IActionResult Get(string id)
        {
            return Ok(_basketRepository.GetBasketAsync(id));
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(string id)
        {
            return Ok(_basketRepository.DeleteBasketAsync(id));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CustomerBasket customerBasket)
        {
            var basket = _basketRepository.UpdateBasketAsync(customerBasket);
            return Ok(basket);
        }
    }
}