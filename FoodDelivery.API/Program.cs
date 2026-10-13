using FoodDelivery.Data;
using Microsoft.EntityFrameworkCore;
using FoodDelivery.Data.Repositories.Interfaces;
using FoodDelivery.Data.Repositories.Implementations;
using FoodDelivery.API.Mappings;
using FoodDelivery.API.Middleware;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration
    .GetConnectionString("FoodDeliveryConnection")
    ?? throw new InvalidOperationException(
        "Database connection string is missing");

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(connectionString);
});

builder.Services.AddAutoMapper(typeof(ItemMappingProfile));

builder.Services.AddScoped(
    typeof(IGenricRepository<>),
    typeof(GenericRepository<>));


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();


app.UseMiddleware<GlobalExceptionMiddleware>();

app.UseMiddleware<CommonResponseMiddleware>();

app.MapControllers();

app.Run();
