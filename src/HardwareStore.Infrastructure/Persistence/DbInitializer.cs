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
                    new Category { Name = "Fasteners", NameMm = "သံချောင်း၊ စကရူး နှင့် ချိတ်ဆက်ပစ္စည်း", Description = "Nails, screws, bolts, washers." },
                    new Category { Name = "Plumbing", NameMm = "ပိုက်နှင့်ရေပိုက်ပစ္စည်း", Description = "Pipes, fittings, valves, taps." },
                    new Category { Name = "Electrical", NameMm = "လျှပ်စစ်ပစ္စည်း", Description = "Wires, bulbs, breakers, switches." }
                ],
                cancellationToken);
        }

        if (!await dbContext.Suppliers.AnyAsync(cancellationToken))
        {
            await dbContext.Suppliers.AddRangeAsync(
                [
                    new Supplier
                    {
                        Name = "Mandalay Building Supply",
                        ContactPerson = "U Aung Min",
                        Phone = "09 420000001",
                        Address = "Mandalay"
                    },
                    new Supplier
                    {
                        Name = "Yangon Electrical Mart",
                        ContactPerson = "Daw Mya Mya",
                        Phone = "09 420000002",
                        Address = "Yangon"
                    }
                ],
                cancellationToken);
        }

        if (!await dbContext.AppSettings.AnyAsync(cancellationToken))
        {
            await dbContext.AppSettings.AddRangeAsync(
                [
                    new AppSetting { Key = "ShopName", Value = "မိသားစု ဟာ့ဒ်ဝဲဆိုင်", Description = "ဘောင်ချာနှင့် အစီရင်ခံစာများတွင် ပြသမည့် ဆိုင်အမည်" },
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

        if (!await dbContext.Products.AnyAsync(cancellationToken))
        {
            var fasteners = await dbContext.Categories.FirstAsync(x => x.Name == "Fasteners", cancellationToken);
            var plumbing = await dbContext.Categories.FirstAsync(x => x.Name == "Plumbing", cancellationToken);
            var electrical = await dbContext.Categories.FirstAsync(x => x.Name == "Electrical", cancellationToken);

            await dbContext.Products.AddRangeAsync(
                [
                    new Product
                    {
                        Name = "1 inch Nail",
                        NameMm = "၁ လက်မ သံချောင်း",
                        Sku = "NAIL-1IN",
                        Barcode = "111000001",
                        CategoryId = fasteners.Id,
                        UnitType = UnitType.Piece,
                        CostPrice = 80,
                        SalePrice = 100,
                        StockOnHand = 500,
                        ReorderLevel = 100
                    },
                    new Product
                    {
                        Name = "PVC Pipe 1/2 inch",
                        NameMm = "ပီဗီစီ ပိုက် ၁/၂ လက်မ",
                        Sku = "PVC-12",
                        Barcode = "222000002",
                        CategoryId = plumbing.Id,
                        UnitType = UnitType.Meter,
                        CostPrice = 1200,
                        SalePrice = 1500,
                        StockOnHand = 80,
                        ReorderLevel = 15
                    },
                    new Product
                    {
                        Name = "LED Bulb 12W",
                        NameMm = "အယ်လ်အီးဒီ မီးလုံး ၁၂W",
                        Sku = "LED-12W",
                        Barcode = "333000003",
                        CategoryId = electrical.Id,
                        UnitType = UnitType.Piece,
                        CostPrice = 2500,
                        SalePrice = 3200,
                        StockOnHand = 60,
                        ReorderLevel = 10
                    }
                ],
                cancellationToken);

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
