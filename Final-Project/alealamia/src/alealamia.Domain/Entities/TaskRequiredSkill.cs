using System;
using System.Collections.Generic;
using System.Text;

namespace alealamia.Domain.Entities
{
    public class TaskRequiredSkill
    {
        public int TaskId { get; set; } //pk , fk
        public int SkillId { get; set; } //pk , fk
        public Task Task { get; set; } = null!;
        public Skill Skill { get; set; } = null!;
    }
}
