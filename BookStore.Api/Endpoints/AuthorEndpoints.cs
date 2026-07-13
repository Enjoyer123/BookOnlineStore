using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Api.Endpoints;

public static class AuthorEndpoints
{
    public static void MapAuthorEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/authors");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var authors = await db.Authors.ToListAsync();
            return Results.Ok(authors.Select(a => a.ToDto()));
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var author = await db.Authors.FindAsync(id);
            if (author is null) return Results.NotFound();
            return Results.Ok(author.ToDto());
        });

        group.MapPost("/", async (AppDbContext db, CreateAuthorDto dto) =>
        {
            var author = dto.ToEntity();
            db.Authors.Add(author);
            await db.SaveChangesAsync();
            return Results.Created($"/api/authors/{author.Id}", author.ToDto());
        }); 

        group.MapPut("/{id:int}", async (AppDbContext db, int id, UpdateAuthorDto dto) =>
        {
            var author = await db.Authors.FindAsync(id);
            if (author is null) return Results.NotFound();

            author.Name = dto.Name;
            await db.SaveChangesAsync();
            return Results.Ok(author.ToDto());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var author = await db.Authors.FindAsync(id);
            if (author is null) return Results.NotFound();

            db.Authors.Remove(author);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}