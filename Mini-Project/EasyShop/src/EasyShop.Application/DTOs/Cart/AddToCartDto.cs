using System.ComponentModel.DataAnnotations;

namespace EasyShop.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        // Upper bound for one cart line. Keeps a single request from reserving absurd amounts of stock.
        public const int MaxQuantityPerItem = 99;

        [Range(1, int.MaxValue, ErrorMessage = "Choose a valid product.")]
        public int ProductVariantId { get; set; }

        [Range(1, MaxQuantityPerItem, ErrorMessage = "Quantity must be between 1 and 99.")]
        public int Quantity { get; set; }
    }
}
