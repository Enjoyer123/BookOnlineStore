using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Api.Endpoints;

public static class PublisherEndpoints
{
    public static void MapPublisherEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/publishers");

        group.MapGet("/", async (AppDbContext db) =>
        {
            var publishers = await db.Publishers.ToListAsync();
            return Results.Ok(publishers.Select(p => p.ToDto()));
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var publisher = await db.Publishers.FindAsync(id);
            if (publisher is null) return Results.NotFound();

            return Results.Ok(publisher.ToDto());
        });

        group.MapPost("/", async (AppDbContext db, CreatePublisherDto dto) =>
        {
            var publisher = dto.ToEntity();

            db.Publishers.Add(publisher);
            await db.SaveChangesAsync();

            return Results.Created($"/api/publishers/{publisher.Id}", publisher.ToDto());
        });

        group.MapPut("/{id:int}", async (AppDbContext db, int id, UpdatePublisherDto dto) =>
        {
            var publisher = await db.Publishers.FindAsync(id);
            if (publisher is null) return Results.NotFound();

            publisher.Name = dto.Name;

            await db.SaveChangesAsync();

            return Results.Ok(publisher.ToDto());
        });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var publisher = await db.Publishers.FindAsync(id);
            if (publisher is null) return Results.NotFound();

            db.Publishers.Remove(publisher);
            await db.SaveChangesAsync();

            return Results.NoContent();
        });
    }
}