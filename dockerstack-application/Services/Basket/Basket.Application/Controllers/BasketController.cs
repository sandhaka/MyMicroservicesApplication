using Basket.Application.Infrastructure.Repositories;
using Basket.Application.Models;
using Basket.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Application.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _basketRepository;
        private readonly IIdentityService _identityService;
        
        public BasketController(IBasketRepository basketRepository,
            IIdentityService identityService)
        {
            _basketRepository = basketRepository;
            _identityService = identityService;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(_basketRepository.GetBasketAsync(_identityService.GetUserIdentity()));
        }

        [HttpDelete]
        public IActionResult Delete()
        {
            return Ok(_basketRepository.DeleteBasketAsync(_identityService.GetUserIdentity()));
        }

        [HttpPost]
        public IActionResult Post([FromBody] CustomerBasket customerBasket)
        {
            var basket = _basketRepository.UpdateBasketAsync(customerBasket);
            return Ok(basket);
        }
    }
}