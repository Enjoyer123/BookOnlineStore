namespace Bookstore.Api.Models;


public class Book{
    public int Id {get; set;}
    public string Title {get; set;} = string.Empty;
    public string Isbn {get; set;} = string.Empty;
    public decimal Price {get; set;}
    public string Description {get; set;} = string.Empty;
    public string CoverImageUrl {get; set;} = string.Empty;
    public int StockQuantity {get; set;}

    public int PublisherId { get; set; }
    public Publisher? Publisher { get; set; }

    public int CategoryId { get; set; }
    public Category? Category { get; set; }

    public ICollection<BookAuthor> BookAuthors { get; set; } = new List<BookAuthor>();

}