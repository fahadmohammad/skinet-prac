namespace Core.Entities
{
    public class ProductAttribute : BaseEntity
    {
        public int ProductId { get; set; }
        public string Attribute { get; set; }
        public string Value { get; set; }

    }
}