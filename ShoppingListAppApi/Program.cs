// Initialize the default app
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Google.Cloud.Firestore;

var defaultApp = FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.GetApplicationDefault(),
});
Console.WriteLine(defaultApp.Name); // "[DEFAULT]"

// Retrieve services by passing the defaultApp variable...
var defaultAuth = FirebaseAuth.GetAuth(defaultApp);

// ... or use the equivalent shorthand notation
defaultAuth = FirebaseAuth.DefaultInstance;

var projectId = "shopping-list-app-db"; // your Firebase project ID
FirestoreDb db = FirestoreDb.Create(projectId);

CollectionReference collection = db.Collection("tobuy");
QuerySnapshot snapshot = await collection.GetSnapshotAsync();

foreach (DocumentSnapshot document in snapshot.Documents)
{
    if (document.Exists)
    {
        string name = document.GetValue<string>("name");
        int quantity = document.GetValue<int>("Quantity");

        Console.WriteLine($"Name: {name}, Quantity: {quantity}");
    }
}









// rest of the code remains unchanged

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
