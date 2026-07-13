using Microsoft.EntityFrameworkCore;
using Bookstore.Api.Data;
using Bookstore.Api.Endpoints;
using Scalar.AspNetCore; 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference(); 
}

app.UseHttpsRedirection();

app.MapCategoryEndpoints();
app.MapAuthorEndpoints();
app.MapPublisherEndpoints();
app.MapBookEndpoints();

app.Run();

