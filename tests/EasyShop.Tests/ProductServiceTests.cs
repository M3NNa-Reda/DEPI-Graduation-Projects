using EasyShop.Application.Interfaces.Repositories;
using EasyShop.Application.Services;
using EasyShop.Domain.Entities;

namespace EasyShop.Tests;

public class ProductServiceTests
{
    private readonly Mock<IUnitOfWork> _uow = new();
    private readonly Mock<IProductRepository> _products = new();
    private readonly Mock<IProductVariantRepository> _variants = new();
    private readonly ProductService _service;

    public ProductServiceTests()
    {
        _uow.SetupGet(u => u.Products).Returns(_products.Object);
        _uow.SetupGet(u => u.ProductVariants).Returns(_variants.Object);
        _service = new ProductService(_uow.Object);
    }

    [Fact]
    public async Task Delete_deactivates_the_product_and_all_of_its_variants_in_one_save()
    {
        var product = new Product { Id = 10, IsActive = true };
        var variants = new List<ProductVariant>
        {
            new() { Id = 1, ProductId = 10, IsActive = true },
            new() { Id = 2, ProductId = 10, IsActive = true }
        };
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(variants);

        await _service.DeleteProductAsync(product);

        Assert.False(product.IsActive);
        Assert.All(variants, v => Assert.False(v.IsActive));
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task SetPrimaryImage_trims_the_link_and_saves_once()
    {
        await _service.SetPrimaryImageAsync(10, "  https://example.com/a.jpg  ");

        _products.Verify(r => r.SetPrimaryImageAsync(10, "https://example.com/a.jpg"), Times.Once);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task SetPrimaryImage_passes_null_through_so_the_picture_can_be_removed()
    {
        await _service.SetPrimaryImageAsync(10, null);

        _products.Verify(r => r.SetPrimaryImageAsync(10, null), Times.Once);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task AddVariant_rejects_a_sku_that_another_option_already_uses()
    {
        _products.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Product { Id = 10, IsActive = true });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(new[] { new ProductVariant { Id = 5, SKU = "TEE-M" } });

        var variant = new ProductVariant { ProductId = 10, SKU = "TEE-M", Price = 100m, StockQuantity = 3 };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddVariantAsync(variant));
        _uow.Verify(u => u.CompleteAsync(), Times.Never);
    }

    [Theory]
    [InlineData(0, 0, 5)]
    [InlineData(100, 50, 5)]
    [InlineData(100, 0, -1)]
    public async Task AddVariant_rejects_invalid_price_original_price_or_stock(double price, double compareAt, int stock)
    {
        _products.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Product { Id = 10, IsActive = true });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(Array.Empty<ProductVariant>());

        var variant = new ProductVariant
        {
            ProductId = 10, SKU = "TEE-L", Price = (decimal)price, CompareAtPrice = (decimal)compareAt, StockQuantity = stock
        };

        await Assert.ThrowsAsync<ArgumentException>(() => _service.AddVariantAsync(variant));
    }

    [Fact]
    public async Task AddVariant_saves_a_trimmed_sku_and_keeps_it_hidden_for_an_inactive_product()
    {
        _products.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Product { Id = 10, IsActive = false });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(Array.Empty<ProductVariant>());

        var variant = new ProductVariant { ProductId = 10, SKU = "  TEE-XL  ", Price = 250m, StockQuantity = 8, IsActive = true };

        await _service.AddVariantAsync(variant);

        Assert.Equal("TEE-XL", variant.SKU);
        Assert.False(variant.IsActive);
        _variants.Verify(r => r.AddAsync(variant), Times.Once);
        _uow.Verify(u => u.CompleteAsync(), Times.Once);
    }

    [Fact]
    public async Task ActiveVariants_are_empty_for_an_inactive_product()
    {
        _products.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Product { Id = 10, IsActive = false });

        var variants = await _service.GetActiveVariantsAsync(10);

        Assert.Empty(variants);
    }

    [Fact]
    public async Task ActiveVariants_are_sorted_by_price()
    {
        _products.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(new Product { Id = 10, IsActive = true });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(new[]
            {
                new ProductVariant { Id = 1, Price = 300m },
                new ProductVariant { Id = 2, Price = 100m },
                new ProductVariant { Id = 3, Price = 200m }
            });

        var variants = (await _service.GetActiveVariantsAsync(10)).Select(v => v.Id).ToList();

        Assert.Equal(new[] { 2, 3, 1 }, variants);
    }

    [Fact]
    public async Task ProductCards_use_the_lowest_price_and_the_primary_image()
    {
        _products.Setup(r => r.GetPrimaryImageUrlsAsync()).ReturnsAsync(new Dictionary<int, string> { [10] = "https://img/10.jpg" });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(new[]
            {
                new ProductVariant { ProductId = 10, Price = 250m, StockQuantity = 0 },
                new ProductVariant { ProductId = 10, Price = 199m, StockQuantity = 4 }
            });

        var cards = await _service.GetProductCardsAsync();

        var card = Assert.Single(cards).Value;
        Assert.Equal(199m, card.FromPrice);
        Assert.True(card.InStock);
        Assert.Equal("https://img/10.jpg", card.ImageUrl);
    }

    [Fact]
    public async Task ProductCards_report_a_product_as_out_of_stock_when_every_variant_is_empty()
    {
        _products.Setup(r => r.GetPrimaryImageUrlsAsync()).ReturnsAsync(new Dictionary<int, string>());
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(new[] { new ProductVariant { ProductId = 10, Price = 50m, StockQuantity = 0 } });

        var cards = await _service.GetProductCardsAsync();

        Assert.False(cards[10].InStock);
    }

    [Fact]
    public async Task ProductCards_keep_a_product_that_has_a_picture_but_no_variant_on_sale()
    {
        _products.Setup(r => r.GetPrimaryImageUrlsAsync()).ReturnsAsync(new Dictionary<int, string> { [20] = "https://img/20.jpg" });
        _variants.Setup(r => r.FindByConditionAsync(It.IsAny<System.Linq.Expressions.Expression<Func<ProductVariant, bool>>>()))
            .ReturnsAsync(Array.Empty<ProductVariant>());

        var cards = await _service.GetProductCardsAsync();

        Assert.Null(cards[20].FromPrice);
        Assert.Equal("https://img/20.jpg", cards[20].ImageUrl);
    }
}
