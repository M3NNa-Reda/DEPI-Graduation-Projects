using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ProductOption
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string Name { get; set; }
        public Product Product { get; set; }

        public ICollection<ProductOptionValue> productOptionValues { get; set; } = new List<ProductOptionValue>();

    }
}
