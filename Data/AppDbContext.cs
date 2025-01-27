using Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<PrivateCustomer> PrivateCustomers { get; set; }
        public DbSet<BusinessCustomer> BusinessCustomers { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<PrivateInsurance> PrivateInsurances { get; set; }
        public DbSet<BusinessInsurance> BusinessInsurances { get; set; }
        public DbSet<Insured> Insured { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<EmployeeRole> EmployeeRoles { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<VehicleInsurance> VehiclesInsurances { get; set; }
        public DbSet<RealEstateInsurance> RealEstatesInsurances { get; set; }
        public DbSet<PaymentPlan> PaymentPlans { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<CustomerProspectInformation> CustomerProspectInformation { get; set; }
        public DbSet<PrivateInsuranceOptionalExtras> PrivateInsuranceOptionalExtras { get; set; }
        public DbSet<Zone> Zones { get; set; }
        public DbSet<InsuranceStatus> InsuranceStatuses { get; set; }
        public DbSet<LiabilityInsurance> LiabilityInsurances { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>()
                .ToTable("Customers");

            modelBuilder.Entity<PrivateCustomer>()
                .ToTable("PrivateCustomers")
                .HasBaseType<Customer>();

            modelBuilder.Entity<BusinessCustomer>()
                .ToTable("BusinessCustomers")
                .HasBaseType<Customer>();

            modelBuilder.Entity<BusinessInsurance>()
                .ToTable("BusinessInsurances");

            modelBuilder.Entity<LiabilityInsurance>()
                .ToTable("LiabilityInsurances")
                .HasBaseType<BusinessInsurance>();

            modelBuilder.Entity<RealEstateInsurance>()
                .ToTable("RealEstateInsurances")
                .HasBaseType<BusinessInsurance>();

            modelBuilder.Entity<VehicleInsurance>()
                .ToTable("VehicleInsurances")
                .HasBaseType<BusinessInsurance>();

            modelBuilder.Entity<BusinessInsurance>(entity =>
            {
                entity.Property(e => e.Premium)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<Employee>(entity =>
            {
                entity.Property(e => e.Comission)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<LiabilityInsurance>(entity =>
            {
                entity.Property(e => e.Deductible)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<PrivateInsurance>(entity =>
            {
                entity.Property(e => e.Premium)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<RealEstateInsurance>(entity =>
            {
                entity.Property(e => e.InventoryPrice)
                      .HasPrecision(18, 2);
                entity.Property(e => e.InventoryValue)
                      .HasPrecision(18, 2);
                entity.Property(e => e.PropertyValue)
                      .HasPrecision(18, 2);
                entity.Property(e => e.RealEstatePremium)
                      .HasPrecision(18, 2);
            });

            modelBuilder.Entity<VehicleInsurance>(entity =>
            {
                entity.Property(e => e.Debuctible)
                      .HasPrecision(18, 2);
            });

            base.OnModelCreating(modelBuilder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("SUP13DB");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}