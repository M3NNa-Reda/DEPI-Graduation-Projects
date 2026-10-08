using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ProductOptionValue
    {
        public int Id { get; set; }
        public int ProductOptionId { get; set; }
        public string Value { get; set; }
        public ProductOption  ProductOption { get; set; }
        public ICollection<ProductVariantOptionValue> ProductVariantOptionValues { get; set; } = new List<ProductVariantOptionValue>();

    }
}
