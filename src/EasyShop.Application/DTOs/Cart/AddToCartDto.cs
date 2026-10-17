using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Cart
{
    public class AddToCartDto
    {
        public int ProductVariantId { get; set; }
        public int Quantity { get; set; }
    }
}
