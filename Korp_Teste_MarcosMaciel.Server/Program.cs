using Korp_Teste_MarcosMaciel.BillingService.Services;
using Korp_Teste_MarcosMaciel.InventoryService.Services;
using Korp_Teste_MarcosMaciel.Server.Data;
using Korp_Teste_MarcosMaciel.Server.Data.Interfaces;
using Korp_Teste_MarcosMaciel.Server.DTOs;
using Korp_Teste_MarcosMaciel.Server.Exceptions;
using Korp_Teste_MarcosMaciel.Server.Services.Billing;
using Korp_Teste_MarcosMaciel.Server.Services.Inventory;
using Korp_Teste_MarcosMaciel.Server.Services.NotasFiscais;
using Korp_Teste_MarcosMaciel.Server.Services.Products;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularApp", policy =>
    {
       policy
           .WithOrigins(
               "http://localhost:4200",
               "https://localhost:4200",
               "http://127.0.0.1:4200",
               "https://127.0.0.1:4200",
               "http://localhost:51075",
               "https://localhost:51075",
               "http://127.0.0.1:51075",
               "https://127.0.0.1:51075")
           .AllowAnyHeader()
           .AllowAnyMethod();
    });
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<INotaFiscalRepository, NotaFiscalRepository>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<NotaFiscalService>();
builder.Services.AddScoped<NotaFiscalImpressaoService>();
builder.Services.AddScoped<InventoryService>();
builder.Services.AddScoped<BillingService>();
builder.Services.AddHttpClient<IInventoryClient, InventoryHttpClient>(client =>
{
    var baseUrl = builder.Configuration["InventoryServiceBaseUrl"] ?? "http://localhost:5214";
    client.BaseAddress = new Uri(baseUrl);
});
builder.Services.AddHttpClient<IBillingClient, BillingHttpClient>(client =>
{
    var baseUrl = builder.Configuration["BillingServiceBaseUrl"] ?? "http://localhost:5215";
    client.BaseAddress = new Uri(baseUrl);
});

var app = builder.Build();

app.UseDefaultFiles();
app.UseStaticFiles();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(exceptionHandlerApp =>
{
    exceptionHandlerApp.Run(async context =>
    {
        var exception = context.Features.Get<IExceptionHandlerFeature>()?.Error;
        var statusCode = exception switch
        {
            NotFoundException => StatusCodes.Status404NotFound,
            DomainException => StatusCodes.Status409Conflict,
            InvalidOperationException => StatusCodes.Status400BadRequest,
            ArgumentException => StatusCodes.Status400BadRequest,
            UnauthorizedAccessException => StatusCodes.Status401Unauthorized,
            TimeoutException => StatusCodes.Status503ServiceUnavailable,
            HttpRequestException => StatusCodes.Status503ServiceUnavailable,
            _ => StatusCodes.Status500InternalServerError
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(new ApiErrorResponse
        {
            Message = exception?.Message ?? "Ocorreu um erro inesperado.",
            Details = statusCode == StatusCodes.Status500InternalServerError
                ? "O sistema falhou ao processar a solicitação."
                : null,
            TimestampUtc = DateTime.UtcNow
        });
    });
});

app.UseCors("AllowAngularApp");
app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapFallbackToFile("/index.html");

app.Run();
