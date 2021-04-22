using System.Collections.Generic;

namespace Core.Entities
{
    public class Review : BaseEntity
    {
        public int Star { get; set; }
        public List<Comment> Comments { get; set; }
        public int AppUserId { get; set; }

    }
}