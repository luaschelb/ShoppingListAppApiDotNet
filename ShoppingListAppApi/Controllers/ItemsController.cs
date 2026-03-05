using Google.Cloud.Firestore;
using Microsoft.AspNetCore.Mvc;

namespace ShoppingListAppApi.Controllers
{
    /// <summary>
    /// Controller responsible for managing shopping list items stored in Firestore.
    /// </summary>
    /// <remarks>
    /// This controller provides CRUD operations for items in the "tobuy" collection
    /// of the Firestore database.
    /// </remarks>
    [ApiController]
    [Route("[controller]")]
    public class ItemsController : ControllerBase
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly FirestoreDb _db;

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemsController"/> class.
        /// </summary>
        /// <param name="logger">Logger used to record application events.</param>
        public ItemsController(ILogger<ItemsController> logger)
        {
            _logger = logger;
            var projectId = Environment.GetEnvironmentVariable("GOOGLE_CLOUD_PROJECT");

            _db = new FirestoreDbBuilder
            {
                ProjectId = projectId,
                DatabaseId = "shopping-list-app-db"
            }.Build();
        }

        /// <summary>
        /// Retrieves all items from the shopping list.
        /// </summary>
        /// <returns>
        /// A list of items stored in the Firestore "tobuy" collection.
        /// </returns>
        /// <response code="200">Returns the list of items.</response>
        [HttpGet(Name = "GetItems")]
        public async Task<IActionResult> Get()
        {
            var itens = new List<object>();
            QuerySnapshot snapshot = await _db.Collection("tobuy").GetSnapshotAsync();

            foreach (DocumentSnapshot document in snapshot.Documents)
            {
                itens.Add(document.ToDictionary());
            }

            return Ok(itens);
        }

        /// <summary>
        /// Adds a new item to the shopping list.
        /// </summary>
        /// <param name="toBuy">
        /// </param>
        /// <returns>A confirmation that the item was created.</returns>
        /// <response code="200">Item successfully created.</response>
        /// <response code="400">Invalid request body.</response>
        [HttpPost(Name = "PostItem")]
        public async Task<IActionResult> Post([FromBody] Item toBuy)
        {
            DocumentReference docRef = _db.Collection("tobuy").Document();
            await docRef.SetAsync(toBuy);

            return Ok();
        }

        /// <summary>
        /// Updates an existing shopping list item.
        /// </summary>
        /// <param name="id">The ID of the Firestore document to update.</param>
        /// <param name="toBuy">
        /// </param>
        /// <returns>A confirmation that the item was updated.</returns>
        /// <response code="200">Item successfully updated.</response>
        /// <response code="404">Item not found.</response>
        [HttpPut(Name = "PutItem")]
        public async Task<IActionResult> Put(string id, [FromBody] Item toBuy)
        {
            DocumentReference docRef = _db.Collection("tobuy").Document(id);
            await docRef.SetAsync(toBuy, SetOptions.Overwrite);

            return Ok();
        }

        /// <summary>
        /// Deletes an item from the shopping list.
        /// </summary>
        /// <param name="id">The ID of the Firestore document to delete.</param>
        /// <returns>A confirmation that the item was deleted.</returns>
        /// <response code="200">Item successfully deleted.</response>
        /// <response code="404">Item not found.</response>
        [HttpDelete(Name = "DeleteItem")]
        public async Task<IActionResult> Delete(string id)
        {
            DocumentReference docRef = _db.Collection("tobuy").Document(id);
            await docRef.DeleteAsync();

            return Ok();
        }
    }
}