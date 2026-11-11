using EasyShop.MVC.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace EasyShop.Tests;

public class ProductImageStorageTests : IDisposable
{
    private static readonly byte[] PngHeader = { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A, 0, 0, 0, 0 };

    private readonly string _webRoot = Path.Combine(Path.GetTempPath(), "easyshop-tests-" + Guid.NewGuid().ToString("N"));
    private readonly ProductImageStorage _storage;

    public ProductImageStorageTests()
    {
        var environment = new Mock<IWebHostEnvironment>();
        environment.SetupGet(e => e.WebRootPath).Returns(_webRoot);
        _storage = new ProductImageStorage(environment.Object);
    }

    public void Dispose()
    {
        if (Directory.Exists(_webRoot))
            Directory.Delete(_webRoot, recursive: true);
    }

    private static IFormFile File(byte[] content, string name = "photo.png")
    {
        return new FormFile(new MemoryStream(content), 0, content.Length, "ImageFile", name);
    }

    [Fact]
    public async Task Save_stores_a_png_and_returns_a_site_relative_url()
    {
        var url = await _storage.SaveAsync(File(PngHeader));

        Assert.StartsWith("/uploads/products/", url);
        Assert.EndsWith(".png", url);
        Assert.True(System.IO.File.Exists(Path.Combine(_webRoot, "uploads", "products", Path.GetFileName(url))));
    }

    [Fact]
    public async Task Save_ignores_the_file_name_and_checks_the_real_content()
    {
        var script = System.Text.Encoding.ASCII.GetBytes("<?php echo 1; ?>");

        await Assert.ThrowsAsync<ArgumentException>(() => _storage.SaveAsync(File(script, "photo.png")));
    }

    [Fact]
    public async Task Save_rejects_files_over_the_size_limit()
    {
        var big = new byte[ProductImageStorage.MaxFileSizeBytes + 1];
        PngHeader.CopyTo(big, 0);

        await Assert.ThrowsAsync<ArgumentException>(() => _storage.SaveAsync(File(big)));
    }

    [Fact]
    public async Task Delete_removes_a_stored_picture_and_leaves_external_links_alone()
    {
        var url = await _storage.SaveAsync(File(PngHeader));
        var path = Path.Combine(_webRoot, "uploads", "products", Path.GetFileName(url));

        _storage.DeleteIfStored("https://example.com/a.jpg");
        Assert.True(System.IO.File.Exists(path));

        _storage.DeleteIfStored(url);
        Assert.False(System.IO.File.Exists(path));
    }

    [Theory]
    [InlineData("https://example.com/a.jpg", true)]
    [InlineData("/uploads/products/abc123.png", true)]
    [InlineData("/uploads/products/../secret.png", false)]
    [InlineData("/uploads/products/sub/abc.png", false)]
    [InlineData("javascript:alert(1)", false)]
    [InlineData("", false)]
    public void IsAcceptedUrl_only_allows_web_links_and_stored_pictures(string url, bool expected)
    {
        Assert.Equal(expected, _storage.IsAcceptedUrl(url));
    }
}
