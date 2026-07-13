using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Api.Endpoints;

public static class CategoryEndpoints
{
    public static void MapCategoryEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/categories");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var categorys = await db.Categories.ToListAsync();
            return Results.Ok(categorys.Select(c => c.ToDto()));
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null) return Results.NotFound();
            return Results.Ok(category.ToDto());
        });

        group.MapPost("/", async (AppDbContext db, CreateCategoryDto dto) =>
        {
            var category = dto.ToEntity();
            db.Categories.Add(category);
            await db.SaveChangesAsync();
            return Results.Created($"/api/categories/{category.Id}", category.ToDto());
        }); 

        group.MapPut("/{id:int}", async (AppDbContext db, int id, UpdateCategoryDto dto) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null) return Results.NotFound();

            category.Name = dto.Name;
            await db.SaveChangesAsync();
            return Results.Ok(category.ToDto());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var category = await db.Categories.FindAsync(id);
            if (category is null) return Results.NotFound();

            db.Categories.Remove(category);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}