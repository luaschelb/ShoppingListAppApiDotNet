using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingListAppApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ToBuysController : ControllerBase
    {
        private readonly ILogger<ToBuysController> _logger;
        private readonly FirestoreDb _db;

        public ToBuysController(ILogger<ToBuysController> logger)
        {
            _logger = logger;
            var projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");

            _db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = "shopping-list-app-db"
            }.Build();
        }

        [HttpGet(Name = "GetToBuys")]
        public async Task<IActionResult> Get()
        {
            var itens = new List<object>();
            QuerySnapshot snapshot = await _db.Collection("tobuy").GetSnapshotAsync();
            foreach(DocumentSnapshot document in snapshot.Documents)
            {
                itens.Add(document.ToDictionary());
            }
            return Ok(itens);
        }
    }
}
