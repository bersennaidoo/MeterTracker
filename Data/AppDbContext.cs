using Microsoft.EntityFrameworkCore;
using MeterTracker.Models;

namespace MeterTracker.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Reading> Readings { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Reading>()
                .HasIndex(r => r.ReadingDate);
        }
    }
}
