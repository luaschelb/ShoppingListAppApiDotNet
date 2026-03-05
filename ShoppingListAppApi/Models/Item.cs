using Google.Cloud.Firestore;

namespace ShoppingListAppApi.Models
{
    [FirestoreData]
    public class Item
    {
        [FirestoreDocumentId]
        public string? Id { get; set; }
        [FirestoreProperty]
        public string? Name { get; set; }
        [FirestoreProperty]
        public int Quantity { get; set; }
    }
}
