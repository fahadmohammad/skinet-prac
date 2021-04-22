using System;
using System.Linq.Expressions;
using Core.Entities;

namespace Core.Specifications
{
    public class ProductWithTypeAndBrandSpecification : BaseSpecification<Product>
    {
        public ProductWithTypeAndBrandSpecification(ProductSpecParam productSpecParam)
            :base(x => 
                (string.IsNullOrEmpty(productSpecParam.Search)|| x.Name.ToLower().Contains
                (productSpecParam.Search))&&
                // (!productSpecParam.TypeId.HasValue || x.ProductTypeId == productSpecParam.TypeId) && 
                (!productSpecParam.BrandId.HasValue || x.ProductBrandId == productSpecParam.BrandId)                
            )
        {
            AddInclude(x => x.ProductBrand);
            //AddInclude(x => x.ProductType);
            AddOrderBy(x => x.Name);
            ApplyPaging(productSpecParam.PageSize * (productSpecParam.PageIndex-1),
                productSpecParam.PageSize);
            
            if(!string.IsNullOrEmpty(productSpecParam.Sort))
            {
                switch(productSpecParam.Sort)
                {
                    case "priceAsc":
                        AddOrderBy(p => p.Price);
                        break;
                    case "priceDesc":
                        AddOrderByDescending(p => p.Price);
                        break;
                    default:
                        AddOrderBy(p => p.Name);
                        break;                    
                }

            }
        }

        public ProductWithTypeAndBrandSpecification(int id) 
            : base(x => x.Id == id)
        {
            AddInclude(x => x.ProductBrand);
            //AddInclude(x => x.ProductType);
        }
    }
}