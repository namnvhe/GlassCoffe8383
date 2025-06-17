using Cafe.BusinessObjects.Models;
using Cafe.DataAccess.DAO;
using Cafe.Repositories.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.Repository
{
    public class OrderRepository : IOrderRepository
    {
        public async Task<List<Order>> GetAllAsync() =>
            await OrderDAO.GetOrdersAsync();

        public async Task<Order> FindOrderByIdAsync(int orderId) =>
            await OrderDAO.FindOrderByIdAsync(orderId);

        public async Task<List<Order>> GetOrdersByUserIdAsync(int userId) =>
            await OrderDAO.GetOrdersByUserIdAsync(userId);

        public async Task<List<Order>> GetOrdersByTableIdAsync(int tableId) =>
            await OrderDAO.GetOrdersByTableIdAsync(tableId);

        public async Task<List<Order>> GetOrdersByStatusAsync(string status) =>
            await OrderDAO.GetOrdersByStatusAsync(status);

        public async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await OrderDAO.GetOrdersByDateRangeAsync(startDate, endDate);

        public async Task<decimal> CalculateOrderTotalAsync(int orderId) =>
            await OrderDAO.CalculateOrderTotalAsync(orderId);

        public async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate) =>
            await OrderDAO.GetTotalRevenueByDateRangeAsync(startDate, endDate);

        public async Task SaveOrderAsync(Order order) =>
            await OrderDAO.SaveOrderAsync(order);

        public async Task UpdateOrderAsync(Order order) =>
            await OrderDAO.UpdateOrderAsync(order);

        public async Task UpdateOrderStatusAsync(int orderId, string status) =>
            await OrderDAO.UpdateOrderStatusAsync(orderId, status);

        public async Task DeleteOrderAsync(Order order) =>
            await OrderDAO.DeleteOrderAsync(order);
    }
}
