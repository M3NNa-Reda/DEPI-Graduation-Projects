using System.Text;
using EasyShop.Domain.Constants;
using EasyShop.Domain.Entities;

namespace EasyShop.API.Helpers.Extensions
{
    /// <summary>
    /// Applies pending EF Core migrations and seeds the catalog (brands, categories and
    /// products with images, options and variants) with realistic sample data, plus the
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

        // ----------------------------- Brands (20) -----------------------------

        private static readonly (string Name, string Description)[] BrandSeed =
        {
            ("NovaTech", "Consumer electronics and smart devices engineered for everyday performance."),
            ("Lumen", "Premium audio and imaging gear for creators and enthusiasts."),
            ("Aurora", "Modern lifestyle technology focused on clean design and usability."),
            ("Vertex", "High-performance computing and gaming hardware."),
            ("Zenith", "Innovative gadgets that push the limits of everyday tech."),
            ("Orbit", "Reliable devices for work, travel and everything in between."),
            ("Nimbus", "Everyday essentials built around comfort and simplicity."),
            ("Quartz", "Precision home appliances for a smarter household."),
            ("Helios", "Outdoor and lighting products powered to go the distance."),
            ("Pulse", "Active gear and audio made to keep you moving."),
            ("Kinetic", "Performance sports equipment for every level of athlete."),
            ("Cobalt", "Durable kitchen and home goods built to last."),
            ("Ember", "Warm, honest products for cooking and self-care."),
            ("Atlas", "Bold, dependable goods for home and adventure."),
            ("Nirvana Living", "Refined home and lifestyle pieces for comfortable living."),
            ("Verve", "Contemporary fashion with a confident, modern edge."),
            ("Solstice", "Timeless wardrobe staples and gentle skincare."),
            ("Foundry", "Craft goods and pantry staples made the traditional way."),
            ("Meridian", "Books, tools and gear for curious, capable people."),
            ("Halo", "Friendly, well-made products for family and pets.")
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
                    LogoUrl = $"https://picsum.photos/seed/{BuildSlug(b.Name)}/200/200",
                    IsActive = true,
                    CreatedAt = now.AddDays(-random.Next(60, 400))
                })
                .ToList();

            context.Brands.AddRange(brands);
            await context.SaveChangesAsync(cancellationToken);
        }

        // -------------------- Categories (20 = 10 roots + 10 children) --------------------

        private static readonly CategorySeed[] CategorySeedData =
        {
            new("Electronics", null, "Phones, computers, audio and cameras for modern life."),
            new("Home & Kitchen", null, "Everything you need for a well-run, comfortable home."),
            new("Fashion", null, "Clothing and footwear for every season and occasion."),
            new("Sports & Outdoors", null, "Gear for training, exploring and the great outdoors."),
            new("Beauty & Personal Care", null, "Skincare, grooming and wellness essentials."),
            new("Grocery", null, "Pantry staples, drinks and everyday food."),
            new("Books & Media", null, "Books, guides and gifts for readers and learners."),
            new("Toys & Games", null, "Fun, safe and educational picks for all ages."),
            new("Automotive", null, "Accessories and gadgets for your vehicle."),
            new("Pet Supplies", null, "Food, toys and comfort for your pets."),
            new("Smartphones", "Electronics", "Flagship and budget phones from leading brands."),
            new("Laptops", "Electronics", "Ultrabooks, creator machines and gaming rigs."),
            new("Audio", "Electronics", "Headphones, earbuds and speakers with rich sound."),
            new("Cameras", "Electronics", "Mirrorless, action and vlogging cameras."),
            new("Men's Clothing", "Fashion", "Jackets, shirts, knitwear and denim for men."),
            new("Women's Clothing", "Fashion", "Dresses, knitwear and denim for women."),
            new("Shoes", "Fashion", "Sneakers, formal shoes and outdoor footwear."),
            new("Kitchenware", "Home & Kitchen", "Cookware, appliances and dining essentials."),
            new("Fitness Equipment", "Sports & Outdoors", "Home gym and training equipment."),
            new("Skin Care", "Beauty & Personal Care", "Serums, creams and treatments for healthy skin.")
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

        // ----------------------------- Products (50) -----------------------------

        private static readonly ProductSeed[] ProductSeedData =
        {
            // Smartphones
            new("Nova X5 Pro 5G Smartphone", "Smartphones", "NovaTech", 899.99m),
            new("Aurora Edge 12 Smartphone", "Smartphones", "Aurora", 749.00m),
            new("Vertex Pixel View 8", "Smartphones", "Vertex", 649.50m),
            new("Zenith Fold Mini", "Smartphones", "Zenith", 1099.00m),
            new("Orbit Lite 4G Smartphone", "Smartphones", "Orbit", 249.99m),

            // Laptops
            new("Nova Book Air 14 Ultrabook", "Laptops", "NovaTech", 1249.00m),
            new("Lumen Pro 16 Creator Laptop", "Laptops", "Lumen", 1899.00m),
            new("Vertex Gaming Rig G7", "Laptops", "Vertex", 1599.99m),
            new("Quartz Slim 13 Laptop", "Laptops", "Quartz", 999.00m),

            // Audio
            new("Pulse Beat Wireless Earbuds", "Audio", "Pulse", 129.99m),
            new("Nimbus Noise-Cancel Headphones", "Audio", "Nimbus", 279.00m),
            new("Helios Soundbar 300W", "Audio", "Helios", 349.50m),
            new("Kinetic Bluetooth Speaker Mini", "Audio", "Kinetic", 59.99m),

            // Cameras
            new("Orbit Mirrorless Camera M50", "Cameras", "Orbit", 899.00m),
            new("Lumen Action Cam 4K", "Cameras", "Lumen", 299.99m),

            // Kitchenware / Home & Kitchen
            new("Cobalt Stainless Cookware Set", "Kitchenware", "Cobalt", 199.00m),
            new("Ember Non-Stick Frying Pan 28cm", "Kitchenware", "Ember", 39.99m),
            new("Atlas Ceramic Dinner Plates (Set of 6)", "Kitchenware", "Atlas", 54.50m),
            new("Nirvana Living Espresso Machine", "Kitchenware", "Nirvana Living", 429.00m),
            new("Meridian Stand Mixer 6L", "Kitchenware", "Meridian", 259.00m),
            new("Quartz Air Purifier HEPA H13", "Home & Kitchen", "Quartz", 219.00m),
            new("Lumen Smart LED Desk Lamp", "Home & Kitchen", "Lumen", 45.99m),
            new("Cobalt Robotic Vacuum Cleaner", "Home & Kitchen", "Cobalt", 329.00m),

            // Men's Clothing
            new("Foundry Classic Denim Jacket", "Men's Clothing", "Foundry", 89.00m),
            new("Verve Slim-Fit Oxford Shirt", "Men's Clothing", "Verve", 45.50m),
            new("Solstice Merino Wool Sweater", "Men's Clothing", "Solstice", 79.00m),

            // Women's Clothing
            new("Aurora Summer Floral Dress", "Women's Clothing", "Aurora", 69.99m),
            new("Nirvana Living Cashmere Cardigan", "Women's Clothing", "Nirvana Living", 129.00m),
            new("Verve High-Waist Skinny Jeans", "Women's Clothing", "Verve", 59.00m),

            // Shoes
            new("Kinetic Runner Pro Sneakers", "Shoes", "Kinetic", 119.99m),
            new("Meridian Leather Oxford Shoes", "Shoes", "Meridian", 149.00m),
            new("Pulse Trail Hiking Boots", "Shoes", "Pulse", 139.50m),

            // Fitness Equipment / Sports
            new("Atlas Adjustable Dumbbell Set 20kg", "Fitness Equipment", "Atlas", 189.00m),
            new("Pulse Yoga Mat Premium 6mm", "Fitness Equipment", "Pulse", 39.00m),
            new("Kinetic Carbon Road Bike Helmet", "Fitness Equipment", "Kinetic", 89.99m),
            new("Helios Camping Lantern Rechargeable", "Sports & Outdoors", "Helios", 34.99m),

            // Skin Care
            new("Halo Vitamin C Brightening Serum", "Skin Care", "Halo", 29.99m),
            new("Solstice Hydrating Face Cream 50ml", "Skin Care", "Solstice", 24.50m),
            new("Ember Argan Oil Hair Mask", "Skin Care", "Ember", 19.99m),

            // Grocery
            new("Foundry Organic Arabica Coffee Beans 1kg", "Grocery", "Foundry", 22.90m),
            new("Cobalt Extra Virgin Olive Oil 1L", "Grocery", "Cobalt", 15.75m),
            new("Nimbus Himalayan Pink Salt 500g", "Grocery", "Nimbus", 6.99m),

            // Books & Media
            new("Meridian The Art of Clean Code (Hardcover)", "Books & Media", "Meridian", 32.00m),
            new("Orbit Astronomy 2026 Wall Calendar", "Books & Media", "Orbit", 14.99m),

            // Toys & Games
            new("Zenith Build-It Robot Kit", "Toys & Games", "Zenith", 59.99m),
            new("Halo Wooden Puzzle Set", "Toys & Games", "Halo", 24.99m),

            // Automotive
            new("Vertex Dash Cam 2K Pro", "Automotive", "Vertex", 129.00m),
            new("Kinetic Car Phone Mount Magnetic", "Automotive", "Kinetic", 19.99m),

            // Pet Supplies
            new("Atlas Premium Dog Food 12kg", "Pet Supplies", "Atlas", 54.99m),
            new("Nimbus Cat Scratching Post", "Pet Supplies", "Nimbus", 39.00m)
        };

        // Options applied per category. Products in unlisted categories get a single default variant.
        private static readonly Dictionary<string, (string Name, string[] Values)[]> CategoryOptions = new()
        {
            ["Smartphones"] = new[] { ("Color", new[] { "Midnight Black", "Silver", "Ocean Blue" }) },
            ["Laptops"] = new[] { ("Color", new[] { "Space Gray", "Silver" }) },
            ["Audio"] = new[] { ("Color", new[] { "Black", "White" }) },
            ["Cameras"] = new[] { ("Color", new[] { "Black" }) },
            ["Men's Clothing"] = new[]
            {
                ("Color", new[] { "Navy", "Charcoal" }),
                ("Size", new[] { "S", "M", "L" })
            },
            ["Women's Clothing"] = new[]
            {
                ("Color", new[] { "Black", "Beige" }),
                ("Size", new[] { "XS", "S", "M" })
            },
            ["Shoes"] = new[]
            {
                ("Color", new[] { "Black", "White" }),
                ("Size", new[] { "40", "41", "42" })
            }
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
                var isActive = random.NextDouble() > 0.15;

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

                AddImages(product, random);
                AddOptionsAndVariants(product, seed.Price, sequence, random);

                products.Add(product);
                sequence++;
            }

            context.Products.AddRange(products);
            await context.SaveChangesAsync(cancellationToken);
        }

        private static void AddImages(Product product, Random random)
        {
            var imageCount = random.Next(2, 5); // 2..4 images
            var slug = product.Slug;

            for (var i = 0; i < imageCount; i++)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = $"https://picsum.photos/seed/{slug}-{i + 1}/800/800",
                    IsPrimary = i == 0,
                    DisplayOrder = i
                });
            }
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
            // Small per-variant price variation keeps the data from looking perfectly uniform.
            var price = decimal.Round(basePrice * (0.97m + (decimal)random.NextDouble() * 0.08m), 2);

            // ~60% of variants advertise a higher "compare at" price.
            var compareAtPrice = random.NextDouble() < 0.6
                ? decimal.Round(price * (1.15m + (decimal)random.NextDouble() * 0.25m), 2)
                : 0m;

            return new ProductVariant
            {
                SKU = sku,
                Price = price,
                CompareAtPrice = compareAtPrice,
                StockQuantity = random.Next(0, 400),
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
            return $"{name} from {brand} blends dependable quality with a clean, modern design. " +
                   $"A standout choice in our {category} range, tested for everyday use and backed by " +
                   $"the EasyShop satisfaction guarantee. Ships fast with easy 30-day returns.";
        }

        private sealed record CategorySeed(string Name, string? Parent, string Description);

        private sealed record ProductSeed(string Name, string Category, string Brand, decimal Price);
    }
}
