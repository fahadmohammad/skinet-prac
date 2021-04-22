using System.Collections.Generic;

namespace Core.Entities
{
    public class ProductCategory : BaseEntity
    {
        public string Name { get; set; }
        public List<ProductType> ProductTypes { get; set; }
    }
}