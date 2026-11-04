using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace EasyShop.MVC.Services;

public class ProductImageStorage : IProductImageStorage
{
    public const long MaxFileSizeBytes = 2 * 1024 * 1024;

    private const string UrlPrefix = "/uploads/products/";

    private readonly string _folder;

    public ProductImageStorage(IWebHostEnvironment environment)
    {
        _folder = Path.Combine(environment.WebRootPath, "uploads", "products");
    }

    public async Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default)
    {
        if (file.Length == 0)
            throw new ArgumentException("The selected file is empty.");

        if (file.Length > MaxFileSizeBytes)
            throw new ArgumentException("The picture must be 2 MB or smaller.");

        await using var input = file.OpenReadStream();
        var header = new byte[12];
        var read = await input.ReadAsync(header.AsMemory(0, header.Length), cancellationToken);

        // The extension comes from the file's real content, never from the name the browser sent.
        var extension = DetectExtension(header.AsSpan(0, read))
            ?? throw new ArgumentException("Only JPG, PNG and WebP pictures are allowed.");

        Directory.CreateDirectory(_folder);
        var fileName = $"{Guid.NewGuid():N}{extension}";

        input.Position = 0;
        await using var output = File.Create(Path.Combine(_folder, fileName));
        await input.CopyToAsync(output, cancellationToken);

        return UrlPrefix + fileName;
    }

    public void DeleteIfStored(string? imageUrl)
    {
        if (string.IsNullOrEmpty(imageUrl) || !imageUrl.StartsWith(UrlPrefix, StringComparison.Ordinal))
            return;

        // GetFileName drops any directory part, so a crafted value cannot point outside the folder.
        var path = Path.Combine(_folder, Path.GetFileName(imageUrl));
        if (File.Exists(path))
            File.Delete(path);
    }

    public bool IsAcceptedUrl(string? imageUrl)
    {
        if (string.IsNullOrWhiteSpace(imageUrl))
            return false;

        if (imageUrl.StartsWith(UrlPrefix, StringComparison.Ordinal))
            return imageUrl.IndexOfAny(new[] { '/', '\\' }, UrlPrefix.Length) < 0 && !imageUrl.Contains("..");

        return Uri.TryCreate(imageUrl, UriKind.Absolute, out var uri)
            && (uri.Scheme == Uri.UriSchemeHttps || uri.Scheme == Uri.UriSchemeHttp);
    }

    private static string? DetectExtension(ReadOnlySpan<byte> header)
    {
        if (header.Length >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF)
            return ".jpg";

        if (header.Length >= 8 && header[..8].SequenceEqual(new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }))
            return ".png";

        if (header.Length >= 12
            && header[..4].SequenceEqual("RIFF"u8)
            && header[8..12].SequenceEqual("WEBP"u8))
            return ".webp";

        return null;
    }
}
