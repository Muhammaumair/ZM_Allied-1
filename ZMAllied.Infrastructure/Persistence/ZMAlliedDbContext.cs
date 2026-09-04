using System.Reflection;
using Microsoft.EntityFrameworkCore;
using ZMAllied.Domain.Entities.Accounting;
using ZMAllied.Domain.Entities.Audit;
using ZMAllied.Domain.Entities.Bilty;
using ZMAllied.Domain.Entities.Dispatch;
using ZMAllied.Domain.Entities.Fleet;
using ZMAllied.Domain.Entities.Identity;
using ZMAllied.Domain.Entities.Organization;
using ZMAllied.Domain.Entities.Parties;
using ZMAllied.Domain.Entities.Trips;

namespace ZMAllied.Infrastructure.Persistence
{
    public class ZMAlliedDbContext : DbContext
    {
        public ZMAlliedDbContext(DbContextOptions<ZMAlliedDbContext> options)
            : base(options)
        {
        }

        // Identity
        public DbSet<User> Users => Set<User>();
        public DbSet<Role> Roles => Set<Role>();
        public DbSet<Permission> Permissions => Set<Permission>();
        public DbSet<UserRole> UserRoles => Set<UserRole>();
        public DbSet<RolePermission> RolePermissions => Set<RolePermission>();

        // Organization
        public DbSet<Company> Companies => Set<Company>();
        public DbSet<Office> Offices => Set<Office>();
        public DbSet<CompanyRate> CompanyRates => Set<CompanyRate>();

        // Parties
        public DbSet<Party> Parties => Set<Party>();
        public DbSet<Supplier> Suppliers => Set<Supplier>();
        public DbSet<PartyTransaction> PartyTransactions => Set<PartyTransaction>();

        // Fleet
        public DbSet<VehicleType> VehicleTypes => Set<VehicleType>();
        public DbSet<Vehicle> Vehicles => Set<Vehicle>();
        public DbSet<Driver> Drivers => Set<Driver>();
        public DbSet<FuelExpense> FuelExpenses => Set<FuelExpense>();
        public DbSet<VehicleMaintenance> VehicleMaintenances => Set<VehicleMaintenance>();

        // Trips
        public DbSet<Trip> Trips => Set<Trip>();
        public DbSet<TripExpense> TripExpenses => Set<TripExpense>();

        // Bilty
        public DbSet<Bilty> Bilties => Set<Bilty>();
        public DbSet<BiltyItem> BiltyItems => Set<BiltyItem>();

        // Accounting
        public DbSet<CashBookEntry> CashBookEntries => Set<CashBookEntry>();
        public DbSet<Payment> Payments => Set<Payment>();
        public DbSet<Receipt> Receipts => Set<Receipt>();
        public DbSet<AccountTransaction> AccountTransactions => Set<AccountTransaction>();

        // Dispatch
        public DbSet<DDR> DDRs => Set<DDR>();

        // Audit
        public DbSet<AuditLog> AuditLogs => Set<AuditLog>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}
