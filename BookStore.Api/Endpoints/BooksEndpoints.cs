using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Microsoft.EntityFrameworkCore;
using Bookstore.Api.Models;
namespace Bookstore.Api.Endpoints;

public static class BookEndpoints
{
    public static void MapBookEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/books");

        group.MapGet("/", async (AppDbContext db, string? name, string? category) =>
        {

            var query = db.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .AsQueryable();

            if (!string.IsNullOrEmpty(name))
                query = query.Where(b => EF.Functions.ILike(b.Title, $"%{name}%"));

            if (!string.IsNullOrEmpty(category))
                query = query.Where(b => EF.Functions.ILike(b.Category!.Name, $"%{category}%"));

            var books = await query.ToListAsync();
            return Results.Ok(books.Select(b => b.ToDto()));
        });

        group.MapGet("/{id:int}", async (AppDbContext db, int id) =>
        {
            var book = await db.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (book is null) return Results.NotFound();
            return Results.Ok(book.ToDto());
        });

        group.MapPost("/", async (AppDbContext db, CreateBookDto dto) =>
        {
            var book = dto.ToEntity();
            db.Books.Add(book);
            await db.SaveChangesAsync();

            var created = await db.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstAsync(b => b.Id == book.Id);

            return Results.Created($"/api/books/{book.Id}", created.ToDto());
        });

        group.MapPut("/{id:int}", async (AppDbContext db, int id, UpdateBookDto dto) =>
       {
           var book = await db.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstOrDefaultAsync(b => b.Id == id);

           if (book is null) return Results.NotFound();

           book.Title = dto.Title;
           book.Description = dto.Description;
           book.Isbn = dto.Isbn;
           book.Price = dto.Price;
           book.CategoryId = dto.CategoryId;
           book.PublisherId = dto.PublisherId;
           book.StockQuantity = dto.StockQuantity;
           book.CoverImageUrl = dto.CoverImageUrl;

           book.BookAuthors.Clear();
           foreach (var authorId in dto.AuthorIds)
           {
               book.BookAuthors.Add(new BookAuthor { AuthorId = authorId });
           }

           await db.SaveChangesAsync();
           
           var updated = await db.Books
                .Include(b => b.Category)
                .Include(b => b.Publisher)
                .Include(b => b.BookAuthors)
                    .ThenInclude(ba => ba.Author)
                .FirstAsync(b => b.Id == id);
           return Results.Ok(updated.ToDto());
       });

        group.MapDelete("/{id:int}", async (AppDbContext db, int id) =>
        {
            var book = await db.Books.FindAsync(id);
            if (book is null) return Results.NotFound();

            db.Books.Remove(book);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
    }
}