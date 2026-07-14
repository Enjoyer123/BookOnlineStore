using Bookstore.Api.Data;
using Bookstore.Api.Dtos;
using Bookstore.Api.Models;
using Microsoft.EntityFrameworkCore;
namespace Bookstore.Api.Endpoints;

using System.Security.Claims;
using System.Text.RegularExpressions;

public static class CartEndpoints
{
    public static void MapCartEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/carts").RequireAuthorization();

        group.MapGet("/", async (AppDbContext db, ClaimsPrincipal user) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var cart = await db.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                db.Carts.Add(cart);
                await db.SaveChangesAsync();
            }

            return Results.Ok(cart.ToDto());
        });

        group.MapPost("/items", async (AppDbContext db, ClaimsPrincipal user, AddCartItemDto dto) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var cart = await db.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null)
            {
                cart = new Cart { UserId = userId };
                db.Carts.Add(cart);
                await db.SaveChangesAsync();
            }

            var existingItem = cart.Items.FirstOrDefault(ci => ci.BookId == dto.BookId);
            if (existingItem != null)
            {
                existingItem.Quantity += dto.Quantity;
            }
            else
            {
                var book = await db.Books.FindAsync(dto.BookId);
                if (book is null) return Results.NotFound("Book not found");

                var newItem = new CartItem
                {
                    BookId = dto.BookId,
                    Book = book, 
                    Quantity = dto.Quantity,
                    CartId = cart.Id
                };
                cart.Items.Add(newItem);
            }

            await db.SaveChangesAsync();
            return Results.Ok(cart.ToDto());
        });
        group.MapPut("/items/{id:int}", async (AppDbContext db, ClaimsPrincipal user, int id, UpdateCartItemDto dto) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var cart = await db.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null) return Results.NotFound("Cart not found");

            var item = cart.Items.FirstOrDefault(ci => ci.Id == id);
            if (item is null) return Results.NotFound("Item not found");

            item.Quantity = dto.Quantity;
            await db.SaveChangesAsync();
            return Results.Ok(cart.ToDto());
        });

        group.MapDelete("/items/{id:int}", async (AppDbContext db, ClaimsPrincipal user, int id) =>
        {
            var userId = int.Parse(user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "0");

            var cart = await db.Carts
                .Include(c => c.Items)
                    .ThenInclude(ci => ci.Book)
                .FirstOrDefaultAsync(c => c.UserId == userId);

            if (cart is null) return Results.NotFound("Cart not found");

            var item = cart.Items.FirstOrDefault(ci => ci.Id == id);
            if (item is null) return Results.NotFound("Item not found");

            cart.Items.Remove(item);
            await db.SaveChangesAsync();
            return Results.Ok(cart.ToDto());
        });
    }
}