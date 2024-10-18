using BusinessCardAPI.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace BusinessCardAPI.Data
{
    public class BusinessCardDBContext : DbContext
    {

        public BusinessCardDBContext(DbContextOptions<BusinessCardDBContext> options) : base(options)
        {
        }

        #region Entities
        public DbSet<BusinessCard> BusinessCards { get; set; }

        #endregion

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
