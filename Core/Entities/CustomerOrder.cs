using System.Collections.Generic;

namespace Core.Entities
{
    public class CustomerOrder
    {
        public int Id { get; set; }

        public string UserId { get; set; }

        public User User { get; set; }

        public List<OrderItem> Items { get; set; } = new List<OrderItem>();
    }
}