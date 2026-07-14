namespace Bookstore.Api.Dtos;
using Bookstore.Api.Models;

public record CartItemDto(int Id, int BookId, string BookTitle, decimal BookPrice, int Quantity);
public record CartDto(int Id, List<CartItemDto> Items);

public record AddCartItemDto(int BookId, int Quantity);

public record UpdateCartItemDto(int Quantity);

public static class CartMapping
{
    public static CartDto ToDto(this Cart cart) =>
        new CartDto(
            cart.Id,
            cart.Items.Select(ci => new CartItemDto(
                ci.Id,
                ci.BookId,
                ci.Book.Title,
                ci.Book.Price,
                ci.Quantity
            )).ToList()
        );  
}