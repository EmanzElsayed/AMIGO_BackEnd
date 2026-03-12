

namespace Amigo.Domain.Entities
{
    public class Order :BaseEntity<Guid>
    {
        [Required]
        public string UserId { get; set; } = null!;

        [Required]
        public ApplicationUser User { get; set; } = null!;
        public Currency Currency { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime? OrderDate { get; set; }

        //Total Amount => calculated
        public decimal TotalAmount { get; set; }
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    }
}
