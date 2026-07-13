namespace Bookstore.Api.Dtos;
using Bookstore.Api.Models;
public record AuthorDto(
    int Id,
    string Name
);

public record CreateAuthorDto(
    string Name
);

public record UpdateAuthorDto(
    string Name
);

public static class AuthorMapping
{
    public static AuthorDto ToDto(this Author author) =>
        new AuthorDto(author.Id, author.Name);

    public static Author ToEntity(this CreateAuthorDto dto) =>
        new Author { Name = dto.Name };
}