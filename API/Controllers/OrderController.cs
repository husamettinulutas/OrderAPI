using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using API.Dtos;
using Core.Entities;
using Core.Interfaces;
using Core.Responses;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers
{
    public class OrderController : BaseApiController
    {
        private readonly IOrderRepository _orderRepository;
        public OrderController(IOrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }

        [HttpGet]
        public async Task<ActionResult<CustomerOrder>> GetOrder()
        {
            var authorizedUser = await _orderRepository.GetUserByName(User.Identity.Name);
            var order = await _orderRepository.GetOrderAsync(authorizedUser.Id);
            if (order == null) return NotFound();

            return Ok(new ControllerResponse
            {
                ResponseCode = 200,
                Result = order,
            });
        }

        [HttpPost]
        public async Task<ActionResult<CustomerOrder>> AddOrder(int productId, int quantity)
        {
            var authorizedUser = await _orderRepository.GetUserByName(User.Identity.Name);
            var order = await _orderRepository.AddOrderAsync(authorizedUser.Id, productId, quantity);
            if (order == null) return NotFound();

            return Ok(new ControllerResponse
            {
                ResponseCode = 200,
                Result = order,
            });
        }

        [HttpPut]
        public async Task<ActionResult<CustomerOrder>> UpdateOrder(CustomerOrderDto req)
        {
            var authorizedUser = await _orderRepository.GetUserByName(User.Identity.Name);

            authorizedUser.UserAddress = new UserAddress
            {
                City = req.Adress.City,
                Country = req.Adress.District,
                District = req.Adress.District,
                Name = req.Adress.Name,
                Phone = req.Adress.Phone,
                PostalCode = req.Adress.PostalCode,
                Street = req.Adress.Street,
                UserId = authorizedUser.Id,
                Id = authorizedUser.UserAddress != null ? authorizedUser.UserAddress.Id : 0,
            };

            var order = new CustomerOrder
            {
                User = authorizedUser,
                UserId = authorizedUser.Id,
                Items = req.Items != null && req.Items.Any() ? req.Items.Select(i => new OrderItem()
                {
                    Id = i.Id,
                    Quantity = i.Quantity,
                }).ToList() : new List<OrderItem>(),
            };

            var updatedOrder = await _orderRepository.UpdateOrderAsync(order);

            if (updatedOrder == null)
            {
                return Ok(new ControllerResponse
                {
                    ResponseCode = -300,
                    Errors = new List<string> { "Order wasn't able to updated" },
                });
            }
            return Ok(new ControllerResponse
            {
                ResponseCode = 200,
                Result = updatedOrder,
            });
        }

        [HttpDelete]
        public async Task<bool> DeleteOrderAsync()
        {
            var authorizedUser = await _orderRepository.GetUserByName(User.Identity.Name);
            if (authorizedUser == null) return false;

            return await _orderRepository.DeleteOrderAsync(authorizedUser.Id);
        }
    }
}