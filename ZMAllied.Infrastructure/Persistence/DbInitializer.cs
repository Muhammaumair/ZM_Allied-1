using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using ZMAllied.Domain.Entities.Identity;

namespace ZMAllied.Infrastructure.Persistence
{
    public static class DbInitializer
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<ZMAlliedDbContext>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<ZMAlliedDbContext>>();
            var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher<User>>();

            try
            {
                // Seed Permissions
                if (!await context.Permissions.AnyAsync())
                {
                    var modules = new Dictionary<string, string[]>
                    {
                        ["Parties"] = new[] { "Parties.View", "Parties.Create", "Parties.Update", "Parties.Delete" },
                        ["Companies"] = new[] { "Companies.View", "Companies.Create", "Companies.Update", "Companies.Delete" },
                        ["CompanyRates"] = new[] { "CompanyRates.View", "CompanyRates.Create", "CompanyRates.Update", "CompanyRates.Delete" },
                        ["Trips"] = new[] { "Trips.View", "Trips.Create", "Trips.Update", "Trips.Delete" },
                        ["Bilty"] = new[] { "Bilty.View", "Bilty.Create", "Bilty.Update", "Bilty.Delete" },
                        ["Bilties"] = new[] { "Bilties.View", "Bilties.Create", "Bilties.Update", "Bilties.Delete", "Bilties.Print" },
                        ["Fleet"] = new[] { "Fleet.View", "Fleet.Create", "Fleet.Update", "Fleet.Delete" },
                        ["CashBook"] = new[] { "CashBook.View", "CashBook.Create", "CashBook.Update", "CashBook.Delete" },
                        ["DDR"] = new[] { "DDR.View", "DDR.Create", "DDR.Update", "DDR.Delete" },
                        ["Suppliers"] = new[] { "Suppliers.View", "Suppliers.Create", "Suppliers.Update", "Suppliers.Delete" },
                        ["Offices"] = new[] { "Offices.View", "Offices.Create", "Offices.Update", "Offices.Delete" },
                        ["VehicleTypes"] = new[] { "VehicleTypes.View", "VehicleTypes.Create", "VehicleTypes.Update", "VehicleTypes.Delete" },
                        ["Vehicles"] = new[] { "Vehicles.View", "Vehicles.Create", "Vehicles.Update", "Vehicles.Delete" },
                        ["Drivers"] = new[] { "Drivers.View", "Drivers.Create", "Drivers.Update", "Drivers.Delete" },
                        ["Reports"] = new[] { "Reports.View" },
                        ["Dashboard"] = new[] { "Dashboard.View" },
                        ["Users"] = new[] { "Users.View", "Users.Create", "Users.Update", "Users.Delete" }
                    };

                    var permissions = new List<Permission>();
                    foreach (var module in modules)
                    {
                        foreach (var permName in module.Value)
                        {
                            permissions.Add(new Permission
                            {
                                Name = permName,
                                Module = module.Key,
                                Description = permName.Replace(".", " "),
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }

                    context.Permissions.AddRange(permissions);
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} permissions.", permissions.Count);
                }

                // Add missing permissions to databases seeded earlier
                var additionalPermissions = new Dictionary<string, string[]>
                {
                    ["VehicleTypes"] = new[] { "VehicleTypes.View", "VehicleTypes.Create", "VehicleTypes.Update", "VehicleTypes.Delete" },
                    ["Vehicles"] = new[] { "Vehicles.View", "Vehicles.Create", "Vehicles.Update", "Vehicles.Delete" },
                    ["Drivers"] = new[] { "Drivers.View", "Drivers.Create", "Drivers.Update", "Drivers.Delete" },
                    ["Bilties"] = new[] { "Bilties.View", "Bilties.Create", "Bilties.Update", "Bilties.Delete", "Bilties.Print" },
                    ["CashBook"] = new[] { "CashBook.View", "CashBook.Create", "CashBook.Update", "CashBook.Delete" },
                    ["DDR"] = new[] { "DDR.View", "DDR.Create", "DDR.Update", "DDR.Delete" },
                    ["Reports"] = new[] { "Reports.View" },
                    ["Dashboard"] = new[] { "Dashboard.View" }
                    , ["Companies"] = new[] { "Companies.View", "Companies.Create", "Companies.Update", "Companies.Delete" }
                    , ["CompanyRates"] = new[] { "CompanyRates.View", "CompanyRates.Create", "CompanyRates.Update", "CompanyRates.Delete" }
                };

                var additionalNames = additionalPermissions.Values.SelectMany(x => x).ToList();
                var existingNames = await context.Permissions
                    .Where(p => additionalNames.Contains(p.Name))
                    .Select(p => p.Name)
                    .ToListAsync();

                var missingPermissions = additionalPermissions
                    .SelectMany(module => module.Value.Select(name => new { module.Key, Name = name }))
                    .Where(x => !existingNames.Contains(x.Name))
                    .Select(x => new Permission
                    {
                        Name = x.Name,
                        Module = x.Key,
                        Description = x.Name.Replace(".", " "),
                        CreatedAt = DateTime.UtcNow
                    })
                    .ToList();

                if (missingPermissions.Count > 0)
                {
                    context.Permissions.AddRange(missingPermissions);
                    await context.SaveChangesAsync();

                    var roles = await context.Roles.ToListAsync();
                    foreach (var role in roles)
                    {
                        var allowedPermissions = role.Name == "Admin"
                            ? missingPermissions
                            : role.Name == "Manager"
                                ? missingPermissions.Where(p => p.Name.EndsWith(".View") || p.Name.EndsWith(".Create") || p.Name.EndsWith(".Update")).ToList()
                                : role.Name == "User"
                                    ? missingPermissions.Where(p => p.Name.EndsWith(".View")).ToList()
                                    : new List<Permission>();

                        foreach (var permission in allowedPermissions)
                        {
                            context.RolePermissions.Add(new RolePermission
                            {
                                RoleId = role.Id,
                                PermissionId = permission.Id,
                                CreatedAt = DateTime.UtcNow
                            });
                        }
                    }
                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded {Count} additional permissions.", missingPermissions.Count);
                }

                // Seed Roles
                if (!await context.Roles.AnyAsync())
                {
                    var adminRole = new Role
                    {
                        Name = "Admin",
                        Description = "Full access to all modules",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var managerRole = new Role
                    {
                        Name = "Manager",
                        Description = "View and manage operational data",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    var userRole = new Role
                    {
                        Name = "User",
                        Description = "View data only",
                        IsActive = true,
                        CreatedAt = DateTime.UtcNow
                    };

                    context.Roles.AddRange(adminRole, managerRole, userRole);
                    await context.SaveChangesAsync();

                    var allPermissions = await context.Permissions.ToListAsync();
                    foreach (var perm in allPermissions)
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = adminRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    var viewPermissions = allPermissions.Where(p => p.Name.EndsWith(".View")).ToList();
                    var managePermissions = allPermissions.Where(p =>
                        p.Name.EndsWith(".Create") || p.Name.EndsWith(".Update")).ToList();

                    foreach (var perm in viewPermissions.Concat(managePermissions))
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = managerRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    foreach (var perm in viewPermissions)
                    {
                        context.RolePermissions.Add(new RolePermission
                        {
                            RoleId = userRole.Id,
                            PermissionId = perm.Id,
                            CreatedAt = DateTime.UtcNow
                        });
                    }

                    await context.SaveChangesAsync();
                    logger.LogInformation("Seeded roles and role-permissions.");
                }

                // Repair Admin access for modules added after an existing database was seeded.
                var requiredAdminPermissionNames = new[]
                {
                    "CashBook.View", "CashBook.Create", "CashBook.Update", "CashBook.Delete",
                    "DDR.View", "DDR.Create", "DDR.Update", "DDR.Delete"
                };
                var existingAdminRole = await context.Roles
                    .FirstOrDefaultAsync(role => role.Name == "Admin" && role.IsActive && !role.IsDeleted);
                if (existingAdminRole is not null)
                {
                    var requiredPermissions = await context.Permissions
                        .Where(permission => requiredAdminPermissionNames.Contains(permission.Name))
                        .ToListAsync();
                    var assignedPermissionIds = await context.RolePermissions
                        .Where(rolePermission => rolePermission.RoleId == existingAdminRole.Id && !rolePermission.IsDeleted)
                        .Select(rolePermission => rolePermission.PermissionId)
                        .ToListAsync();
                    var missingRolePermissions = requiredPermissions
                        .Where(permission => !assignedPermissionIds.Contains(permission.Id))
                        .Select(permission => new RolePermission
                        {
                            RoleId = existingAdminRole.Id,
                            PermissionId = permission.Id,
                            CreatedAt = DateTime.UtcNow
                        })
                        .ToList();

                    if (missingRolePermissions.Count > 0)
                    {
                        context.RolePermissions.AddRange(missingRolePermissions);
                        await context.SaveChangesAsync();
                        logger.LogInformation("Assigned {Count} missing CashBook/DDR permissions to Admin.", missingRolePermissions.Count);
                    }
                }

                // Ensure default Umair admin user exists and password hash is valid
                var umairUser = await context.Users
                    .FirstOrDefaultAsync(u => u.Username.ToLower() == "umair");

                if (umairUser == null)
                {
                    umairUser = new User
                    {
                        FullName = "Umair",
                        Username = "umair",
                        Email = "umair@zmallied.com",
                        IsActive = true,
                        IsDeleted = false,
                        CreatedAt = DateTime.UtcNow
                    };

                    umairUser.PasswordHash = passwordHasher.HashPassword(umairUser, "11223");

                    context.Users.Add(umairUser);
                    await context.SaveChangesAsync();

                    logger.LogInformation("Created default Umair user.");
                }
                else
                {
                    bool needsUpdate = false;

                    if (!umairUser.IsActive)
                    {
                        umairUser.IsActive = true;
                        needsUpdate = true;
                    }

                    if (umairUser.IsDeleted)
                    {
                        umairUser.IsDeleted = false;
                        needsUpdate = true;
                    }

                    var verifyResult = string.IsNullOrEmpty(umairUser.PasswordHash)
                        ? PasswordVerificationResult.Failed
                        : passwordHasher.VerifyHashedPassword(umairUser, umairUser.PasswordHash, "11223");

                    if (verifyResult == PasswordVerificationResult.Failed)
                    {
                        umairUser.PasswordHash = passwordHasher.HashPassword(umairUser, "11223");
                        needsUpdate = true;
                        logger.LogInformation("Repaired password hash for Umair user.");
                    }

                    if (needsUpdate)
                    {
                        umairUser.UpdatedAt = DateTime.UtcNow;
                        await context.SaveChangesAsync();
                    }
                }

                // Ensure Umair has Admin role
                var umairAdminRole = await context.Roles
                    .FirstOrDefaultAsync(r =>
                        r.Name == "Admin" &&
                        r.IsActive &&
                        !r.IsDeleted);

                if (umairAdminRole != null)
                {
                    var existingUserRole = await context.UserRoles
                        .FirstOrDefaultAsync(ur =>
                            ur.UserId == umairUser.Id &&
                            ur.RoleId == umairAdminRole.Id &&
                            !ur.IsDeleted);

                    if (existingUserRole == null)
                    {
                        context.UserRoles.Add(new UserRole
                        {
                            UserId = umairUser.Id,
                            RoleId = umairAdminRole.Id,
                            CreatedAt = DateTime.UtcNow,
                            IsDeleted = false
                        });

                        await context.SaveChangesAsync();

                        logger.LogInformation("Assigned Admin role to Umair.");
                    }
                }

                logger.LogInformation("Default Umair admin user is ready.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }
    }
}
