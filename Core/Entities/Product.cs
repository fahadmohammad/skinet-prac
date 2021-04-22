using System.Collections.Generic;

namespace Core.Entities
{
    public class Product : BaseEntity
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        // public ProductType ProductType { get; set; }
        // public int ProductTypeId { get; set; }
        public ProductBrand ProductBrand { get; set; }
        public int ProductBrandId { get; set; }
        public int VendorId { get; set; }
        public Vendor Vendor { get; set; }
        public string Sku { get; set; }
        public string ShortDescription { get; set; }
        public List<Media> Medias { get; set; }
        public string RootProductId { get; set; }
        public decimal DiscountedPrice { get; set; }
        public List<Review> Reviews { get; set; }
        public List<string> ReturnPolicies { get; set; }
        public string PictureUrl { get; set; }
        public int ProductCategoryId { get; set; }
        public ProductCategory ProductCategory { get; set; }

    }
}