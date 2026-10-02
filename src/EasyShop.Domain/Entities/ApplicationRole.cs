using System;
using System.Collections.Generic;
using System.Text;

namespace EasyShop.Domain.Entities
{
    public class ApplicationRole
    {
        public string Id { get; set; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public bool IsActive { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}
