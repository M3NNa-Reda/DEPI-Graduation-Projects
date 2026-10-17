using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Cart
{
    public class CartDto
    {
        public int Id { get; set; }
        public List<CartItemDto> Items { get; set; } = new();
        public decimal Subtotal { get; set; }
    }
}
