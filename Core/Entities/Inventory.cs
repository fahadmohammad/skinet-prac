namespace Core.Entities
{
    public class Inventory : BaseEntity
    {
        public string ProductSku { get; set; }
        public UnitType UnitType { get; set; }
        public double Quantity { get; set; }

    }
}