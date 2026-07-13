namespace Bookstore.Api.Dtos;

using Bookstore.Api.Models;

public record PublisherDto(
    int Id,
    string Name
);

public record CreatePublisherDto(
    string Name
);

public record UpdatePublisherDto(
    string Name
);

public static class PublisherMapping
{
    public static PublisherDto ToDto(this Publisher publisher) =>
        new PublisherDto(publisher.Id, publisher.Name);

    public static Publisher ToEntity(this CreatePublisherDto dto) =>
        new Publisher { Name = dto.Name };
}