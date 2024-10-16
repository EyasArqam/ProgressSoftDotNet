using BusinessCardAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardAPI.Data
{
    public class BusinessCardDBContext : DbContext
    {
    
        #region Entities
        public DbSet<BusinessCard> BusinessCards { get; set; }

        #endregion

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);

            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");


            optionsBuilder.UseSqlServer(connectionString, serverOptions =>
            {
                serverOptions.EnableRetryOnFailure(
                    maxRetryCount: 3,
                    maxRetryDelay: TimeSpan.FromSeconds(3),
                    errorNumbersToAdd: null);
            });
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BusinessCard>().HasKey(bc => bc.Id);

            modelBuilder.Entity<BusinessCard>()
                        .HasIndex(bc => bc.Name);

            modelBuilder.Entity<BusinessCard>()
                        .HasIndex(bc => bc.DateOfBirth);

            modelBuilder.Entity<BusinessCard>()
                        .HasIndex(bc => bc.Phone);

            modelBuilder.Entity<BusinessCard>()
                        .HasIndex(bc => bc.Gender);

            modelBuilder.Entity<BusinessCard>()
                        .HasIndex(bc => bc.Email);

        }
    }
}
