using Microsoft.AspNetCore.Http;

namespace EasyShop.MVC.Services;

public interface IProductImageStorage
{
    /// <summary>Validates and stores an uploaded picture, returning the site-relative URL to save on the product.</summary>
    /// <exception cref="ArgumentException">The file is empty, too large, or not a JPEG, PNG or WebP image.</exception>
    Task<string> SaveAsync(IFormFile file, CancellationToken cancellationToken = default);

    /// <summary>Removes a picture this storage created. Links to other sites and unknown paths are ignored.</summary>
    void DeleteIfStored(string? imageUrl);

    /// <summary>True for a link the product form accepts: an http(s) address or a picture stored by this class.</summary>
    bool IsAcceptedUrl(string? imageUrl);
}
