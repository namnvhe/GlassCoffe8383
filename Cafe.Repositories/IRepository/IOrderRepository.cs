using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IOrderRepository
    {
        Task<List<Order>> GetAllAsync();
        Task<Order> FindOrderByIdAsync(int orderId);
        Task SaveOrderAsync(Order order);
        Task UpdateOrderAsync(Order order);
        Task DeleteOrderAsync(Order order);

        // tìm kiếm theo quan hệ
        Task<List<Order>> GetOrdersByUserIdAsync(int userId);
        Task<List<Order>> GetOrdersByTableIdAsync(int tableId);
        Task<List<Order>> GetOrdersByStatusAsync(string status);
        Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate);

        // tính toán và thống kê
        Task<decimal> CalculateOrderTotalAsync(int orderId);
        Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate);

        // cập nhật đặc biệt
        Task UpdateOrderStatusAsync(int orderId, string status);
    }
}
