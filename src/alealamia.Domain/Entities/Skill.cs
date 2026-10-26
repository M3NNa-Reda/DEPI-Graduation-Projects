using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Domain.Entities
{
    public class Skill
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
