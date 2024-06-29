using API.Models;
using Microsoft.EntityFrameworkCore;
using System.Xml;

namespace API
{
    public class AppDbContext : DbContext
    {
        public DbSet<Contact> Contacts { get; set; }
        private readonly IConfiguration _configuration;

        public AppDbContext(DbContextOptions<AppDbContext> options, IConfiguration configuration)
        : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(_configuration.GetConnectionString("SqlServerConnectionString"));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Contact>().ToTable("Contacts");

            modelBuilder.Entity<Contact>().HasData(
                new Contact { 
                    Id = 1,
                    Name = "POTUS", 
                    Address = "White House, 1600 Pennsylvania Avenue, Northwest, Washington, District of Columbia, DC",
                    Phone = "202-456-1111",
                    Email = "comments@whitehouse.gov",
                    IsDeleted = false
                }
            );
        }
    }
}
