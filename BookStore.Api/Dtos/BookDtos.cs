using Bookstore.Api.Models;

namespace Bookstore.Api.Dtos;

public record BookDto(
    int Id,
    string Title,
    string Description,
    string Isbn,
    decimal Price,
    int CategoryId,
    string CategoryName,     
    int PublisherId,
    string PublisherName,   
    int StockQuantity,
    string CoverImageUrl,
    List<AuthorDto> Authors
);

public record CreateBookDto(
    string Title,
    string Description,
    string Isbn,
    decimal Price,
    int CategoryId,
    int PublisherId,
    int StockQuantity,
    string CoverImageUrl,
    List<int> AuthorIds
);

public record UpdateBookDto(
    string Title,
    string Description,
    string Isbn,
    decimal Price,
    int CategoryId,
    int PublisherId,
    int StockQuantity,
    string CoverImageUrl,
    List<int> AuthorIds
);

public static class BookMapping
{
    public static BookDto ToDto(this Book book) => new(
        book.Id,
        book.Title,
        book.Description,
        book.Isbn,
        book.Price,
        book.CategoryId,
        book.Category?.Name ?? "",      
        book.PublisherId,
        book.Publisher?.Name ?? "",     
        book.StockQuantity,
        book.CoverImageUrl,
        book.BookAuthors.Select(ba => new AuthorDto(ba.Author!.Id, ba.Author.Name)).ToList()
    );

    public static Book ToEntity(this CreateBookDto dto) => new()
    {
        Title = dto.Title,
        Description = dto.Description,
        Isbn = dto.Isbn,
        Price = dto.Price,
        CategoryId = dto.CategoryId,
        PublisherId = dto.PublisherId,
        StockQuantity = dto.StockQuantity,
        CoverImageUrl = dto.CoverImageUrl,
        BookAuthors = dto.AuthorIds
        .Select(authorId => new BookAuthor { AuthorId = authorId })
        .ToList()
    };
}

