namespace Bookstore.Api.Dtos;
using Bookstore.Api.Models;
public record CategoryDto(
    int Id,
    string Name
);

public record CreateCategoryDto(
    string Name
);

public record UpdateCategoryDto(
    string Name
);

public static class CategoryMapping
{
    public static CategoryDto ToDto(this Category category) =>
        new CategoryDto(category.Id, category.Name);

    public static Category ToEntity(this CreateCategoryDto dto) =>
        new Category { Name = dto.Name };
}