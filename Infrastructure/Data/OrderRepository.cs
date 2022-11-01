using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace Infrastructure.Data
{
    public class OrderRepository : IOrderRepository
    {
        private readonly StoreContext _context;
        private readonly UserManager<User> _userManager;

        public OrderRepository(StoreContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<User> GetUserByName(string userName)
        {
            try
            {
                var user = await _userManager.Users.Where(u => u.UserName == userName).FirstOrDefaultAsync();

                if (user == null)
                {
                    return null;
                }

                return user;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> DeleteOrderAsync(string userId)
        {
            var relatedOrder = await _context.Orders.FirstOrDefaultAsync(x => x.UserId == userId);

            if (relatedOrder == null)
                return false;

            _context.Orders.Remove(relatedOrder);
            _context.SaveChanges();

            return true;
        }

        public async Task<CustomerOrder> GetOrderAsync(string userId)
        {
            var data = await _context.Orders.FirstOrDefaultAsync(x => x.UserId == userId);

            if (data == null)
            {
                return null;
            }

            var model = new CustomerOrder
            {
                Id = data.Id,
                User = data.User,
                UserId = data.UserId,
                Items = data.OrderItems,
            };

            return data == null ? null : model;
        }

        public async Task<CustomerOrder> AddOrderAsync(string userId, int productId, int quantity)
        {
            var data = await _context.Orders.FirstOrDefaultAsync(x => x.UserId == userId);

            if (data == null)
            {
                return null;
            }

            var relatedProduct = await _context.Products.FirstOrDefaultAsync(p => p.Id == productId);

            if (relatedProduct == null)
            {
                return null;
            }

            var orderItem = new OrderItem
            {
                OrderId = data.Id,
                Brand = relatedProduct.ProductBrand.ToString(),
                PictureUrl = relatedProduct.PictureUrl,
                Price = relatedProduct.Price,
                ProductName = relatedProduct.Name,
                Type = relatedProduct.ProductTypeId.ToString(),
                Quantity = quantity,
            };

            _context.OrderItems.Add(orderItem);

            _context.SaveChanges();

            var model = new CustomerOrder
            {
                Id = data.Id,
                User = data.User,
                UserId = data.UserId,
                Items = data.OrderItems,
            };

            return data == null ? null : model;
        }

        public async Task<CustomerOrder> UpdateOrderAsync(CustomerOrder order)
        {
            var relatedOrder = await _context.Orders.FirstOrDefaultAsync(x => x.UserId == order.UserId);

            if (relatedOrder == null) return null;

            relatedOrder.OrderItems = order.Items.Select(i =>
            {

                if (relatedOrder.OrderItems.Any(o => o.Id == i.Id))
                {
                    var relatedItem = relatedOrder.OrderItems.FirstOrDefault(o => o.Id == i.Id);
                    relatedItem.Quantity = i.Quantity;

                    return relatedItem;
                }
                return null;
            }).ToList();

            relatedOrder.User = order.User;
            relatedOrder.UserId = order.UserId;

            _context.Orders.Update(relatedOrder);
            _context.SaveChanges();

            var model = new CustomerOrder
            {
                Id = relatedOrder.Id,
                User = relatedOrder.User,
                UserId = relatedOrder.UserId,
                Items = relatedOrder.OrderItems,
            };


            return relatedOrder == null ? null : model;
        }
    }
}