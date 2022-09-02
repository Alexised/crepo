using CargaAmbulatoria.EntityFramework.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace CargaAmbulatoria.EntityFramework.DataBase
{
    public class CargaAmbulatoriaDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Document> Documents { get; set; }
        public DbSet<Cohort> Cohorts { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json")
                .Build();
            optionsBuilder.UseSqlServer(configuration.GetConnectionString("CargaAmbulatoria"), builder =>
            {
                builder.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null);
            });
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {

            var userId = Guid.NewGuid();
            builder.Entity<User>().HasData(new User
            {
                UserId = userId.ToString(),
                Name = "Admin",
                Email = "admin@coosalud.com",
                Status = Enums.UserStatusEnum.Enabled,
                Role = Enums.UserRoleEnum.Admin,
                PasswordStored = "NhPGnizPnx4RQRSNWFXaRw==", // Coosalud2022*
            });

            userId = Guid.NewGuid();

            builder.Entity<User>().HasData(new User
            {
                UserId = userId.ToString(),
                Name = "Agent",
                Email = "agent@coosalud.com",
                Status = Enums.UserStatusEnum.Enabled,
                Role = Enums.UserRoleEnum.Agent,
                PasswordStored = "NhPGnizPnx4RQRSNWFXaRw==", // Coosalud2022*
            });

            builder.Entity<Cohort>().HasData(new Cohort
            {
                CohortId = 1,
                Name = "ERC"
            });

        }
    }
}
