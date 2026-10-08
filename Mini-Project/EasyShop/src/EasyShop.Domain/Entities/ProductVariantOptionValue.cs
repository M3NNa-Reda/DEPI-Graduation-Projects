using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ProductVariantOptionValue
    {
        // pk composite from (ProductVariantId , ProductOptionValueId)
        public int ProductVariantId { get; set; } //fk 
        public int ProductOptionValueId { get; set; } // fk 
        public ProductVariant  ProductVariant { get; set; }

        public ProductOptionValue  ProductOptionValue { get; set; }
    }
}
