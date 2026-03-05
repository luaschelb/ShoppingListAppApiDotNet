using System;
using System.IO;
using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Bind na porta definida pelo ambiente (Cloud Run define PORT)
var portEnv = Environment.GetEnvironmentVariable("PORT") ?? "8080";
if (!int.TryParse(portEnv, out var port)) port = 8080;
builder.WebHost.ConfigureKestrel(options =>
{
    options.ListenAnyIP(port);
});

// Credenciais: arquivo, JSON em env var ou ADC (metadata server)
GoogleCredential cred;
var credEnv = Environment.GetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS");
if (!string.IsNullOrEmpty(credEnv))
{
    if (File.Exists(credEnv))
    {
        using var fs = File.OpenRead(credEnv);
        cred = GoogleCredential.FromStream(fs);
        Console.WriteLine($"Using credentials file: {credEnv}");
    }
    else if (credEnv.TrimStart().StartsWith("{"))
    {
        cred = GoogleCredential.FromJson(credEnv);
        Console.WriteLine("Using credentials JSON from env var");
    }
    else
    {
        throw new FileNotFoundException($"Credential file not found: {credEnv}");
    }
}
else
{
    cred = GoogleCredential.GetApplicationDefault();
    Console.WriteLine("Using Application Default Credentials");
}

var defaultApp = FirebaseApp.Create(new AppOptions
{
    Credential = cred,
});
Console.WriteLine(defaultApp.Name); // "[DEFAULT]"


// Retrieve services by passing the defaultApp variable...
var defaultAuth = FirebaseAuth.GetAuth(defaultApp);

// ... or use the equivalent shorthand notation
defaultAuth = FirebaseAuth.DefaultInstance;

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
