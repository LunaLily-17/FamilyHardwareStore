using HardwareStore.Application.Abstractions;
using HardwareStore.Domain.Entities;
using HardwareStore.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace HardwareStore.Infrastructure.Persistence;

public static class DbInitializer
{
    public static async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken = default)
    {
        await using var scope = services.CreateAsyncScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        var passwordHasher = scope.ServiceProvider.GetRequiredService<IPasswordHasher>();

        await dbContext.Database.EnsureCreatedAsync(cancellationToken);

        if (!await dbContext.Users.AnyAsync(cancellationToken))
        {
            await dbContext.Users.AddAsync(new User
            {
                Username = "admin",
                DisplayName = "Store Admin",
                Role = UserRole.Admin,
                PasswordHash = passwordHasher.Hash("Admin@123")
            }, cancellationToken);
        }

        if (!await dbContext.Categories.AnyAsync(cancellationToken))
        {
            await dbContext.Categories.AddRangeAsync(
                [
                    new Category { Name = "Fasteners", Description = "Nails, screws, bolts, washers." },
                    new Category { Name = "Plumbing", NameMm = "ပိုက်နှင့်ရေပိုက်ပစ္စည်း", Description = "Pipes, fittings, valves, taps." },
                    new Category { Name = "Electrical", NameMm = "လျှပ်စစ်ပစ္စည်း", Description = "Wires, bulbs, breakers, switches." }
                ],
                cancellationToken);
        }

        if (!await dbContext.AppSettings.AnyAsync(cancellationToken))
        {
            await dbContext.AppSettings.AddRangeAsync(
                [
                    new AppSetting { Key = "ShopName", Value = "Family Hardware Store", Description = "Displayed on receipts and reports." },
                    new AppSetting { Key = "ShopNameMm", Value = "မိသားစု ဟာ့ဒ်ဝဲဆိုင်", Description = "Burmese shop name for receipts." },
                    new AppSetting { Key = "ShopPhone", Value = "+44 0000 000000", Description = "Printed on receipts." },
                    new AppSetting { Key = "ShopAddress", Value = "Main Street", Description = "Printed on receipts." },
                    new AppSetting { Key = "ReceiptFooter", Value = "Thank you for shopping with us.", Description = "Receipt footer text." }
                ],
                cancellationToken);
        }

        if (!await dbContext.Users.AnyAsync(x => x.Role == UserRole.Cashier, cancellationToken))
        {
            await dbContext.Users.AddAsync(new User
            {
                Username = "cashier",
                DisplayName = "Store Cashier",
                Role = UserRole.Cashier,
                PasswordHash = passwordHasher.Hash("Cashier@123")
            }, cancellationToken);
        }

        await dbContext.SaveChangesAsync(cancellationToken);
    }
}
