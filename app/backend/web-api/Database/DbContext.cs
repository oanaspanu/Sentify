using Microsoft.EntityFrameworkCore;
using AnalysisAPI.Models;

namespace AnalysisAPI.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Analysis> Analyses { get; set; }
        public DbSet<SentimentDistribution> SentimentDistributions { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Analysis>()
                .HasOne(a => a.User) 
                .WithMany() 
                .HasForeignKey(a => a.UserId);

            modelBuilder.Entity<SentimentDistribution>()
                .HasOne(s => s.Analysis)
                .WithMany(a => a.SentimentDistribution)
                .HasForeignKey(s => s.AnalysisId);
        }
    }
}
