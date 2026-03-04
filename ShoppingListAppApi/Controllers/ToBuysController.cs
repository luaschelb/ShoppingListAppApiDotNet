using Microsoft.AspNetCore.Mvc;

namespace ShoppingListAppApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToBuysController : ControllerBase
    {
        private readonly ILogger<ToBuysController> _logger;

        public ToBuysController(ILogger<ToBuysController> logger)
        {
            _logger = logger;
        }

        [HttpGet(Name = "GetToBuys")]
        public IEnumerable<ToBuy> Get()
        {
            return Enumerable.Range(1, 5).Select(index => new ToBuy
            {
                Name = "Produto qualquer",
                Quantity = 1
            })
            .ToArray();
        }
    }
}
