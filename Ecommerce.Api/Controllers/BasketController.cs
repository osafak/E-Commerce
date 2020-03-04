using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Service.Query.Basket;

namespace Ecommerce.Api.Controllers
{
    [ApiController]
    [Route("api/")]
    public class BasketController : Controller
    {
        private readonly IMediator _mediator;
        public BasketController(IMediator mediator)
        {
            _mediator = mediator;
        }
        [Route("basket"), HttpPost]
        public async Task<IActionResult> CreateBasket([FromBody]BasketSaveQuery request)
        {

            var result = await _mediator.Send(request);
            return Ok(result);
        }

        [Route("basket/{basketId}"), HttpGet]
        public async Task<IActionResult> GetBasket([FromRoute]int basketId)
        {
            var result = await _mediator.Send(new GetBasketByIdQuery { BasketId = basketId });
            return Ok(result);
        }
    }
}