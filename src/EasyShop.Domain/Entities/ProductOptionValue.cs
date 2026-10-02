using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ProductOptionValue
    {
        public int Id { get; set; }
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
    }
}
