using Basket.API.Model;
using Basket.API.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;

namespace Basket.API.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BasketController : Controller
    {
        private readonly IBasketRepository _repository;
        private readonly ILogger<BasketController> _logger;

        public BasketController(IBasketRepository repository, ILogger<BasketController> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        [HttpGet("{customerId}")]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> GetBasketByIdAsync(string customerId)
        {
            var basket = await _repository.GetBasketAsync(customerId);

            return Ok(basket ?? new CustomerBasket(customerId, new List<BasketItem>()));
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerBasket), (int)HttpStatusCode.OK)]
        public async Task<ActionResult<CustomerBasket>> UpdateBasketAsync([FromBody] CustomerBasket basket) => 
            Ok(await _repository.UpdateBasketAsync(basket));

        [HttpDelete("{customerId}")]
        [ProducesResponseType(typeof(bool), (int) HttpStatusCode.OK)]
        public async Task DeleteBasketByIdAsync(string customerId) =>
            await _repository.DeleteBasketAsync(customerId);

        public IActionResult Index()
        {
            return View();
        }
    }
}
