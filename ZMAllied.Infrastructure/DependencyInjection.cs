using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ZMAllied.Application.Interfaces;
using ZMAllied.Application.Services;
using ZMAllied.Domain.Entities.Identity;
using ZMAllied.Infrastructure.Persistence;
using ZMAllied.Infrastructure.Repositories;
using ZMAllied.Infrastructure.Services;

namespace ZMAllied.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            services.AddDbContext<ZMAlliedDbContext>(options =>
                options.UseSqlServer(connectionString, b => b.MigrationsAssembly(typeof(ZMAlliedDbContext).Assembly.FullName)));

            // Repositories
            services.AddScoped<IPartyRepository, PartyRepository>();
            services.AddScoped<ICompanyRepository, CompanyRepository>();
            services.AddScoped<ICompanyRateRepository, CompanyRateRepository>();
            services.AddScoped<ISupplierRepository, SupplierRepository>();
            services.AddScoped<IOfficeRepository, OfficeRepository>();
            services.AddScoped<IVehicleTypeRepository, VehicleTypeRepository>();
            services.AddScoped<IVehicleRepository, VehicleRepository>();
            services.AddScoped<IDriverRepository, DriverRepository>();
            services.AddScoped<ITripRepository, TripRepository>();
            services.AddScoped<IBiltyRepository, BiltyRepository>();
            services.AddScoped<IDDRRepository, DDRRepository>();
            services.AddScoped<ICashBookRepository, CashBookRepository>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();
            services.AddScoped<IReceiptRepository, ReceiptRepository>();
            services.AddScoped<IFuelExpenseRepository, FuelExpenseRepository>();
            services.AddScoped<IVehicleMaintenanceRepository, VehicleMaintenanceRepository>();
            services.AddScoped<IUserRepository, UserRepository>();

            // Application Services
            services.AddScoped<IPartyService, PartyService>();
            services.AddScoped<ICompanyService, CompanyService>();
            services.AddScoped<ICompanyRateService, CompanyRateService>();
            services.AddScoped<ISupplierService, SupplierService>();
            services.AddScoped<IOfficeService, OfficeService>();
            services.AddScoped<IVehicleTypeService, VehicleTypeService>();
            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<IDriverService, DriverService>();
            services.AddScoped<ITripService, TripService>();
            services.AddScoped<IBiltyService, BiltyService>();
            services.AddScoped<IDDRService, DDRService>();
            services.AddScoped<ICashBookService, CashBookService>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IReceiptService, ReceiptService>();
            services.AddScoped<IFuelExpenseService, FuelExpenseService>();
            services.AddScoped<IVehicleMaintenanceService, VehicleMaintenanceService>();
            services.AddScoped<IDashboardService, DashboardService>();
            services.AddScoped<IReportsService, ReportsService>();
            services.AddScoped<IBiltyPrintService, BiltyPrintService>();
            services.AddScoped<IAuthService, AuthService>();

            // Infrastructure Services
            services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();

            // Password Hasher
            services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

            return services;
        }
    }
}
