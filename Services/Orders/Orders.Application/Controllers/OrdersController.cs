using System;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Orders.Application.Commands;

namespace Orders.Application.Controllers
{
    [Route("api/[controller]")]
//    [Authorize]
    public class OrdersController : Controller
    {
        private readonly IMediator _mediator;
        
        public OrdersController(IMediator mediator)
        {
            _mediator = mediator ?? throw new ArgumentNullException(nameof(mediator));
        }
        
        /// <summary>
        /// Create a new order
        /// </summary>
        /// <param name="createOrderCommand">order dto</param>
        /// <returns>Http response</returns>
        [Route("new")]
        [HttpPost]
        public async Task<IActionResult> CreateOrderAsync([FromBody]CreateOrderCommand createOrderCommand)
        {
            bool commandResult = false;
            commandResult = await _mediator.Send(createOrderCommand);
            
            return commandResult ? Ok() : (IActionResult)BadRequest();
        }
    }
}