using Core.Entities.Identity;

namespace Core.Entities
{
    public class Vendor : BaseEntity
    {
        public int AppUserId { get; set; }
        public int ProductQuality { get; set; }
        public int DeliveryResponse { get; set; }
        public int CustomerService { get; set; }
    }
}