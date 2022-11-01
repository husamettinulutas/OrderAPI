using Core.Entities;
using System.Collections.Generic;

namespace API.Dtos
{
    public class CustomerOrderDto
    {
        public int Id { get; set; }

        public UserAddressDto Adress { get; set; }

        public List<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
