using System.Text;
using EasyShop.Domain.Constants;
using EasyShop.Domain.Entities;

namespace EasyShop.API.Helpers.Extensions
{
    /// <summary>
    /// Applies pending EF Core migrations and seeds a showcase catalog of men's and women's accessories
    /// (brands, categories and products with colour and size options and variants), plus the
    /// application roles and, optionally, an admin account taken from configuration.
    /// Seeding is idempotent: each block is skipped when its data already exists.
    /// </summary>
    public static class DatabaseMigrator
    {
        public static async Task MigrateDatabaseAsync(this WebApplication app)
        {
            using IServiceScope scope = app.Services.CreateScope();
            IServiceProvider serviceProvider = scope.ServiceProvider;
            AppDbContext dbContext = serviceProvider.GetRequiredService<AppDbContext>();

            await dbContext.Database.MigrateAsync();
            await SeedDataAsync(dbContext);
            await SeedRolesAsync(dbContext);
            await SeedAdminAsync(
                dbContext,
                serviceProvider.GetRequiredService<IPasswordHasher>(),
                app.Configuration);
        }

        // ---------------------------------------------------------------------
        // Seeding
        // ---------------------------------------------------------------------

        private static async Task SeedDataAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            await SeedBrandsAsync(context, cancellationToken);
            await SeedCategoriesAsync(context, cancellationToken);
            await SeedProductsAsync(context, cancellationToken);
        }

        // ------------------------- Roles and admin account -------------------------

        private static async Task SeedRolesAsync(AppDbContext context, CancellationToken cancellationToken = default)
        {
            (string Name, string Description)[] roles =
            {
                (RoleNames.Admin, "Manages the catalog, orders and users."),
                (RoleNames.Customer, "Registered shop customer.")
            };

            List<string> existingNames = await context.ApplicationRoles
                .Select(r => r.Name)
                .ToListAsync(cancellationToken);

            List<ApplicationRole> missingRoles = roles
                .Where(r => !existingNames.Contains(r.Name))
                .Select(r => new ApplicationRole
                {
                    Name = r.Name,
                    Description = r.Description,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow
                })
                .ToList();

            if (missingRoles.Count == 0)
            {
                return;
            }

            context.ApplicationRoles.AddRange(missingRoles);
            await context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// Creates the first admin from the Seed:AdminEmail and Seed:AdminPassword settings
        /// (use user-secrets or environment variables, never appsettings.json).
        /// Does nothing when the settings are missing or the account already exists.
        /// </summary>
        private static async Task SeedAdminAsync(
            AppDbContext context,
            IPasswordHasher passwordHasher,
            IConfiguration configuration,
            CancellationToken cancellationToken = default)
        {
            string? email = configuration["Seed:AdminEmail"]?.Trim().ToLowerInvariant();
            string? password = configuration["Seed:AdminPassword"];

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrEmpty(password))
            {
                return;
            }

            if (await context.ApplicationUsers.AnyAsync(u => u.Email == email, cancellationToken))
            {
                return;
            }

            ApplicationRole adminRole = await context.ApplicationRoles
                .SingleAsync(r => r.Name == RoleNames.Admin, cancellationToken);

            context.ApplicationUsers.Add(new ApplicationUser
            {
                Email = email,
                FirstName = "Admin",
                LastName = "User",
                PasswordHash = passwordHasher.Hash(password),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                ApplicationUserRoles = { new ApplicationUserRole { RoleId = adminRole.Id } }
            });

            await context.SaveChangesAsync(cancellationToken);
        }

        // ------------------------------- Brands (5) -------------------------------
        // Fictional brands, so the showcase store does not use anyone's trademark.

        private static readonly (string Name, string Description)[] BrandSeed =
        {
            ("Nile Atelier", "Leather bags, wallets and belts made to last."),
            ("Sahara Gold", "Everyday jewelry and bracelets with a warm, golden finish."),
            ("Cairo Time", "Watches that pair clean design with dependable movements."),
            ("Luxor Optics", "Sunglasses with UV400 lenses in classic and modern shapes."),
            ("Oasis Studio", "Scarves, wraps and belts in soft, easy-to-style fabrics.")
        };

        private static async Task SeedBrandsAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            if (await context.Brands.AnyAsync(cancellationToken))
            {
                return;
            }

            var random = new Random(20260101);
            var now = DateTime.UtcNow;

            var brands = BrandSeed
                .Select(b => new Brand
                {
                    Name = b.Name,
                    Description = b.Description,
                    IsActive = true,
                    CreatedAt = now.AddDays(-random.Next(60, 400))
                })
                .ToList();

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync(cancellationToken);
        }

        // -------------------- Categories (2 roots + 10 children) --------------------

        private static readonly CategorySeed[] CategorySeedData =
        {
            new("Women", null, "Accessories for her."),
            new("Men", null, "Accessories for him."),
            new("Women's Bags", "Women", "Totes, crossbody bags and clutches."),
            new("Women's Jewelry", "Women", "Earrings, necklaces, rings and bracelets."),
            new("Women's Watches", "Women", "Elegant watches for every day."),
            new("Women's Sunglasses", "Women", "Cat-eye, round and oversized frames."),
            new("Scarves & Wraps", "Women", "Silk scarves and soft wraps."),
            new("Men's Watches", "Men", "Chronographs, dress and field watches."),
            new("Men's Belts", "Men", "Leather and reversible belts."),
            new("Men's Wallets", "Men", "Bifold wallets, card holders and organizers."),
            new("Men's Sunglasses", "Men", "Aviator and wayfarer frames."),
            new("Men's Bracelets", "Men", "Leather and steel bracelets.")
        };

        private static async Task SeedCategoriesAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            if (await context.Categories.AnyAsync(cancellationToken))
            {
                return;
            }

            var random = new Random(20260202);
            var now = DateTime.UtcNow;

            var roots = new Dictionary<string, Category>();
            var categories = new List<Category>();

            // Roots first so children can reference them.
            foreach (var seed in CategorySeedData.Where(c => c.Parent is null))
            {
                var category = new Category
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    IsActive = true,
                    CreatedAt = now.AddDays(-random.Next(60, 400))
                };
                roots[seed.Name] = category;
                categories.Add(category);
            }

            foreach (var seed in CategorySeedData.Where(c => c.Parent is not null))
            {
                var category = new Category
                {
                    Name = seed.Name,
                    Description = seed.Description,
                    ParentCategory = roots[seed.Parent!],
                    IsActive = true,
                    CreatedAt = now.AddDays(-random.Next(10, 300))
                };
                categories.Add(category);
            }

            context.Categories.AddRange(categories);
            await context.SaveChangesAsync(cancellationToken);
        }

        // ----------------------------- Products (25) -----------------------------
        // Prices are in EGP.

        private static readonly ProductSeed[] ProductSeedData =
        {
            // Women's Bags
            new("Leather Tote Bag", "Women's Bags", "Nile Atelier", 1850m),
            new("Leather Crossbody Bag", "Women's Bags", "Nile Atelier", 1350m),
            new("Mini Evening Clutch", "Women's Bags", "Nile Atelier", 950m),

            // Women's Jewelry
            new("Pearl Drop Earrings", "Women's Jewelry", "Sahara Gold", 780m),
            new("Layered Chain Necklace", "Women's Jewelry", "Sahara Gold", 1150m),
            new("Stackable Ring Set", "Women's Jewelry", "Sahara Gold", 640m),
            new("Charm Bracelet", "Women's Jewelry", "Sahara Gold", 890m),

            // Women's Watches
            new("Mesh Strap Watch", "Women's Watches", "Cairo Time", 2450m),
            new("Minimal Ladies Watch", "Women's Watches", "Cairo Time", 1990m),

            // Women's Sunglasses
            new("Cat-Eye Sunglasses", "Women's Sunglasses", "Luxor Optics", 720m),
            new("Oversized Round Sunglasses", "Women's Sunglasses", "Luxor Optics", 680m),

            // Scarves & Wraps
            new("Printed Silk Scarf", "Scarves & Wraps", "Oasis Studio", 540m),
            new("Cashmere-Blend Wrap", "Scarves & Wraps", "Oasis Studio", 1250m),

            // Men's Watches
            new("Chronograph Steel Watch", "Men's Watches", "Cairo Time", 3250m),
            new("Classic Leather Strap Watch", "Men's Watches", "Cairo Time", 2150m),
            new("Field Watch with Canvas Strap", "Men's Watches", "Cairo Time", 1650m),

            // Men's Belts
            new("Full-Grain Leather Belt", "Men's Belts", "Nile Atelier", 690m),
            new("Reversible Dress Belt", "Men's Belts", "Oasis Studio", 560m),

            // Men's Wallets
            new("Bifold Leather Wallet", "Men's Wallets", "Nile Atelier", 820m),
            new("Slim Card Holder", "Men's Wallets", "Nile Atelier", 460m),
            new("Travel Organizer Wallet", "Men's Wallets", "Nile Atelier", 990m),

            // Men's Sunglasses
            new("Aviator Sunglasses", "Men's Sunglasses", "Luxor Optics", 750m),
            new("Wayfarer Sunglasses", "Men's Sunglasses", "Luxor Optics", 690m),

            // Men's Bracelets
            new("Braided Leather Bracelet", "Men's Bracelets", "Sahara Gold", 420m),
            new("Steel Cuff Bracelet", "Men's Bracelets", "Sahara Gold", 580m)
        };

        // Options applied per category. Categories not listed get a single default variant.
        private static readonly Dictionary<string, (string Name, string[] Values)[]> CategoryOptions = new()
        {
            ["Women's Bags"] = new[] { ("Color", new[] { "Black", "Tan", "Burgundy" }) },
            ["Women's Jewelry"] = new[] { ("Finish", new[] { "Gold", "Silver" }) },
            ["Women's Watches"] = new[] { ("Color", new[] { "Silver", "Rose Gold" }) },
            ["Women's Sunglasses"] = new[] { ("Color", new[] { "Black", "Tortoise" }) },
            ["Scarves & Wraps"] = new[] { ("Color", new[] { "Ivory", "Navy", "Rose" }) },
            ["Men's Watches"] = new[] { ("Color", new[] { "Black", "Silver" }) },
            ["Men's Belts"] = new[]
            {
                ("Color", new[] { "Black", "Brown" }),
                ("Size", new[] { "90 cm", "100 cm", "110 cm" })
            },
            ["Men's Wallets"] = new[] { ("Color", new[] { "Black", "Brown" }) },
            ["Men's Sunglasses"] = new[] { ("Color", new[] { "Black", "Tortoise" }) },
            ["Men's Bracelets"] = new[] { ("Color", new[] { "Black", "Brown" }) }
        };

        private static async Task SeedProductsAsync(AppDbContext context, CancellationToken cancellationToken)
        {
            if (await context.Products.AnyAsync(cancellationToken))
            {
                return;
            }

            var brandsByName = (await context.Brands.ToListAsync(cancellationToken))
                .GroupBy(b => b.Name)
                .ToDictionary(g => g.Key, g => g.First());

            var categoriesByName = (await context.Categories.ToListAsync(cancellationToken))
                .GroupBy(c => c.Name)
                .ToDictionary(g => g.Key, g => g.First());

            var random = new Random(20260303);
            var now = DateTime.UtcNow;
            var products = new List<Product>();
            var sequence = 1;

            foreach (var seed in ProductSeedData)
            {
                if (!brandsByName.TryGetValue(seed.Brand, out var brand) ||
                    !categoriesByName.TryGetValue(seed.Category, out var category))
                {
                    // Lookup rows should always exist right after seeding; skip defensively.
                    continue;
                }

                var createdAt = now.AddDays(-random.Next(0, 180)).AddHours(-random.Next(0, 24));
                var isActive = true;

                var product = new Product
                {
                    Name = seed.Name,
                    Slug = BuildSlug(seed.Name),
                    Description = BuildDescription(seed.Name, brand.Name, category.Name),
                    Brand = brand,
                    Category = category,
                    IsActive = isActive,
                    CreatedAt = createdAt,
                    UpdatedAt = random.NextDouble() > 0.5 ? createdAt.AddDays(random.Next(1, 30)) : null
                };

                AddOptionsAndVariants(product, seed.Price, sequence, random);

                products.Add(product);
                sequence++;
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync(cancellationToken);
        }

        private static void AddOptionsAndVariants(Product product, decimal basePrice, int sequence, Random random)
        {
            var brandCode = BuildBrandCode(product.Brand!.Name);
            var skuPrefix = $"{brandCode}-{sequence:D3}";

            var combinations = new List<List<ProductOptionValue>>();

            if (CategoryOptions.TryGetValue(product.Category!.Name, out var optionDefinitions))
            {
                var options = new List<ProductOption>();
                foreach (var (name, values) in optionDefinitions)
                {
                    var option = new ProductOption { Name = name };
                    foreach (var value in values)
                    {
                        option.ProductOptionValues.Add(new ProductOptionValue { Value = value });
                    }
                    options.Add(option);
                    product.ProductOptions.Add(option);
                }

                combinations = BuildCombinations(options);
            }

            if (combinations.Count == 0)
            {
                // No options for this category: a single default variant.
                product.ProductVariants.Add(CreateVariant(product, $"{skuPrefix}-01", basePrice, random));
                return;
            }

            var variantIndex = 1;
            foreach (var combination in combinations)
            {
                var variant = CreateVariant(product, $"{skuPrefix}-{variantIndex:D2}", basePrice, random);
                foreach (var optionValue in combination)
                {
                    variant.ProductVariantOptionValues.Add(new ProductVariantOptionValue
                    {
                        ProductOptionValue = optionValue
                    });
                }
                product.ProductVariants.Add(variant);
                variantIndex++;
            }
        }

        private static ProductVariant CreateVariant(Product product, string sku, decimal basePrice, Random random)
        {
            // About a third of the variants show a crossed-out higher price, rounded to a clean 10 EGP.
            var compareAtPrice = random.NextDouble() < 0.3
                ? Math.Round(basePrice * 1.2m / 10m) * 10m
                : 0m;

            return new ProductVariant
            {
                SKU = sku,
                Price = basePrice,
                CompareAtPrice = compareAtPrice,
                StockQuantity = random.Next(5, 60),
                IsActive = product.IsActive,
                CreatedAt = product.CreatedAt,
                UpdatedAt = product.UpdatedAt ?? product.CreatedAt
            };
        }

        private static List<List<ProductOptionValue>> BuildCombinations(List<ProductOption> options)
        {
            var combinations = new List<List<ProductOptionValue>>();
            if (options.Count == 0)
            {
                return combinations;
            }

            combinations.Add(new List<ProductOptionValue>());
            foreach (var option in options)
            {
                var expanded = new List<List<ProductOptionValue>>();
                foreach (var combination in combinations)
                {
                    foreach (var value in option.ProductOptionValues)
                    {
                        var next = new List<ProductOptionValue>(combination) { value };
                        expanded.Add(next);
                    }
                }
                combinations = expanded;
            }

            return combinations;
        }

        // ---------------------------------------------------------------------
        // Helpers
        // ---------------------------------------------------------------------

        private static string BuildSlug(string value)
        {
            var builder = new StringBuilder(value.Length);
            var lastWasDash = false;

            foreach (var character in value.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    builder.Append(character);
                    lastWasDash = false;
                }
                else if (!lastWasDash && builder.Length > 0)
                {
                    builder.Append('-');
                    lastWasDash = true;
                }
            }

            return builder.ToString().Trim('-');
        }

        private static string BuildBrandCode(string brandName)
        {
            var letters = new string(brandName.Where(char.IsLetter).ToArray()).ToUpperInvariant();
            return letters.Length >= 3 ? letters[..3] : letters.PadRight(3, 'X');
        }

        private static string BuildDescription(string name, string brand, string category)
        {
            return $"{name} by {brand}, from our {category} collection. " +
                   "Designed for everyday wear and easy to gift. " +
                   "Ships within two working days with free returns for 14 days.";
        }

        private sealed record CategorySeed(string Name, string? Parent, string Description);

        private sealed record ProductSeed(string Name, string Category, string Brand, decimal Price);
    }
}
