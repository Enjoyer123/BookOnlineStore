using Microsoft.EntityFrameworkCore;
using Bookstore.Api.Models;

namespace Bookstore.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();
    public DbSet<BookAuthor> BookAuthors => Set<BookAuthor>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Publisher> Publishers => Set<Publisher>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // BookId and AuthorId are composite primary keys for the BookAuthor entity
        modelBuilder.Entity<BookAuthor>()
            .HasKey(ba => new { ba.BookId, ba.AuthorId });

        // Restrict delete behavior for the relationships between Book and Category, and Book and Publisher
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Category)
            .WithMany()
            .HasForeignKey(b => b.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // Restrict delete behavior for the relationship between Book and Publisher
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Publisher)
            .WithMany()
            .HasForeignKey(b => b.PublisherId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}