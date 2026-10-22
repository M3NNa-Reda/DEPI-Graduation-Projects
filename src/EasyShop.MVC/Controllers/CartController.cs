using EasyShop.Application.DTOs.Cart;

namespace EasyShop.MVC.Controllers;

[Authorize]
public class CartController : Controller
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    // GET: /Cart
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var cart = await _cartService.GetCartAsync(User.GetUserId());
        return View(cart);
    }

    // POST: /Cart/Add
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(int productVariantId, int quantity = 1)
    {
        try
        {
            await _cartService.AddToCartAsync(User.GetUserId(), new AddToCartDto
            {
                ProductVariantId = productVariantId,
                Quantity = quantity
            });

            TempData["Success"] = "Item added to your cart.";
            return RedirectToAction(nameof(Index));
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
            return RedirectToPreviousPage();
        }
    }

    // Sends the customer back to the page the form was posted from (for example the product page),
    // falling back to the cart when the referer is missing or not a local address.
    private IActionResult RedirectToPreviousPage()
    {
        if (Uri.TryCreate(Request.Headers.Referer.ToString(), UriKind.Absolute, out var referer)
            && Url.IsLocalUrl(referer.PathAndQuery))
        {
            return LocalRedirect(referer.PathAndQuery);
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Update
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(int productVariantId, int quantity)
    {
        try
        {
            await _cartService.UpdateCartItemAsync(User.GetUserId(), productVariantId, quantity);
            TempData["Success"] = "Cart updated.";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Remove
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Remove(int productVariantId)
    {
        try
        {
            await _cartService.RemoveFromCartAsync(User.GetUserId(), productVariantId);
            TempData["Success"] = "Item removed from your cart.";
        }
        catch (ArgumentException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    // POST: /Cart/Clear
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Clear()
    {
        await _cartService.ClearCartAsync(User.GetUserId());
        TempData["Success"] = "Your cart is now empty.";
        return RedirectToAction(nameof(Index));
    }
}
