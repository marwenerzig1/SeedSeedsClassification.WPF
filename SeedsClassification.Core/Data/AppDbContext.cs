using Microsoft.EntityFrameworkCore;
using SeedsClassification.Core.Models;

namespace SeedsClassification.Core.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Experience> Experiences { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder options)
        {
            options.UseSqlite("Data Source=knn.db");
        }
    }
}