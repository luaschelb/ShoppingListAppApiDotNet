using FirebaseAdmin;
using FirebaseAdmin.Auth;
using Google.Apis.Auth.OAuth2;
using Microsoft.AspNetCore.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Reflection;

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
cred = GoogleCredential.GetApplicationDefault();

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


// CORS: origens configuráveis por variável de ambiente (comma-separated)
var allowed = Environment.GetEnvironmentVariable("CORS_ALLOWED_ORIGINS") ?? "http://localhost:3000";
var origins = allowed.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

builder.Services.AddCors(options =>
{
    options.AddPolicy("DefaultCorsPolicy", policy =>
    {
        policy.WithOrigins(origins)
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "ToDo API",
        Description = "An ASP.NET Core Web API for managing ToDo items",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    // using System.Reflection;
    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("DefaultCorsPolicy");

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
