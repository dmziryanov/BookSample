using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using DAL.Models;

namespace DAL.Context
{
    public class BookContext : DbContext
        {
            public BookContext() : base("DefaultConnection")
            {
            }

            public DbSet<Book> Books { get; set; }

            protected override void OnModelCreating(DbModelBuilder modelBuilder)
            {
                modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
            }
        }
    
}