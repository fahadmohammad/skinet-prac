namespace Core.Entities
{
    public class Comment : BaseEntity
    {
        public string AppUserId { get; set; }
        public string UserComments { get; set; }
    }
}