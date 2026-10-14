using EasyShop.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Order
{
    public class UpdateOrderStatusDto
    {
        public OrderStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}
