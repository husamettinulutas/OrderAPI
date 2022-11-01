using System.Threading.Tasks;
using Core.Entities;

namespace Core.Interfaces
{
    public interface IOrderRepository
    {
        Task<CustomerOrder> GetOrderAsync(string userId);

        Task<CustomerOrder> AddOrderAsync(string userId, int productId, int quantity);

        Task<CustomerOrder> UpdateOrderAsync(CustomerOrder order);

        Task<bool> DeleteOrderAsync(string userId);

        Task<User> GetUserByName(string userName);
    }
}