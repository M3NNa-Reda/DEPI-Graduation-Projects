using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Review
{
    public class ReviewDto
    {
        public int Id { get; set; }
        public int OrderItemId { get; set; }
        public int ProductId { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
