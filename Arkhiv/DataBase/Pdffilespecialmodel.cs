using Microsoft.EntityFrameworkCore;
using SammanWebSite.Models;

namespace SammanWebSite.DataBase
{
    public class PdffilespecialmodelDbContext : DbContext
    {
        public DbSet<PdfFileSpecialModel> Special { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlite("Data Source=Database/Base/special.db");

        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<PdfFileSpecialModel>()
                .Property(u => u.Id)
                .ValueGeneratedOnAdd();
        }
    }
}
