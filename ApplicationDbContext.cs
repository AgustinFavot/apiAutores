using apiVS.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;


namespace apiVS
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions option) : base(option)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            //Mi identidad AutoresLibros va a tener una llave primaria compuesta
            modelBuilder.Entity<AuthorsBooks>().HasKey(al => new { al.AuthorId, al.BookId });
        }

        public DbSet<Book> Books { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<AuthorsBooks> AuthorsBooks { get; set; }

    }
}
