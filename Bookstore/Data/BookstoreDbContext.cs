using Bookstore.Model;
using Microsoft.EntityFrameworkCore;

namespace Bookstore.Data
{
    /// <summary>
    /// Represents the database context for the Bookstore application.
    /// </summary>
    public class BookstoreDbContext : DbContext
    {

        public DbSet<Admin> Admins { get; set; }


        public DbSet<Author> Authors { get; set; }


        public DbSet<Book> Books { get; set; }


        public DbSet<Category> Categories { get; set; }


        public DbSet<Customer> Customers { get; set; }


        public DbSet<OrderHistory> OrderHistories { get; set; }

        public DbSet<OrderDetail>OrderDetails { get; set; }
        public DbSet<Review> Reviews { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BookstoreDbContext"/> class.
        /// </summary>
        /// <param name="options">The options to be used by the DbContext.</param>
        public BookstoreDbContext(DbContextOptions<BookstoreDbContext> options) : base(options)
        {
        }

        /// <summary>
        /// Configures the entity relationships and table mappings.
        /// </summary>
        /// <param name="modelBuilder">The model builder used to configure the entity types.</param>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Map entities to tables and set primary keys
            modelBuilder.Entity<Admin>().ToTable("Admins").HasKey(x => x.Id);
            modelBuilder.Entity<Category>().ToTable("Categories").HasKey(x => x.Id);
            modelBuilder.Entity<Customer>().ToTable("Customers").HasKey(x => x.Id);
            modelBuilder.Entity<OrderHistory>().ToTable("OrderHistory").HasKey(x => x.Id);
            modelBuilder.Entity<OrderDetail>().ToTable("OrderDetails").HasKey(x => x.Id);
            modelBuilder.Entity<Review>().ToTable("Reviews").HasKey(x => x.Id);
            modelBuilder.Entity<Author>().ToTable("Authors").HasKey(x => x.Id);
            modelBuilder.Entity<Book>().ToTable("Books").HasKey(x => x.Id);

            // Configure relationships for Book entity
            modelBuilder.Entity<Book>()
                .HasOne(b => b.Author)
                .WithMany(a => a.Books)
                .HasForeignKey(b => b.AuthorId);

            modelBuilder.Entity<Book>()
                .HasOne(b => b.Category)
                .WithMany(c => c.Books)
                .HasForeignKey(b => b.CategoryID);

            // Configure relationships for OrderHistory entity
            modelBuilder.Entity<OrderHistory>()
                .HasOne(oh => oh.Customer)
                .WithMany(c => c.OrderHistories)
                .HasForeignKey(oh => oh.CustomerID);

            modelBuilder.Entity<OrderHistory>()
                .HasOne(oh => oh.Book)
                .WithMany(b => b.OrderHistories)
                .HasForeignKey(oh => oh.BookID);
            // Configure one-to-one relationship
            modelBuilder.Entity<OrderHistory>()
                .HasOne(o => o.Detail)
                .WithOne(d => d.OrderHistory)
                .HasForeignKey<OrderDetail>(d => d.OrderHistoryId);
            // Configure relationships for Review entity
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Customer)
                .WithMany(c => c.Reviews)
                .HasForeignKey(r => r.CustomerID);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Book)
                .WithMany(b => b.Reviews)
                .HasForeignKey(r => r.BookID);

            base.OnModelCreating(modelBuilder);
        }
    }
}
