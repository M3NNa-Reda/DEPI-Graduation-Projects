using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Application.DTOs.Review
{
    public class AddReviewDto
    {
        public int OrderItemId { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
    }
}
