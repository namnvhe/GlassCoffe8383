using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IOrderItemRepository
    {
        Task<List<OrderItem>> GetAllAsync();
        Task<OrderItem> FindOrderItemByIdAsync(int orderItemId);
        Task SaveOrderItemAsync(OrderItem orderItem);
        Task UpdateOrderItemAsync(OrderItem orderItem);
        Task DeleteOrderItemAsync(OrderItem orderItem);

        // tìm kiếm theo quan hệ
        Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId);
        Task<List<OrderItem>> GetOrderItemsByMenuItemIdAsync(int menuItemId);

        // xử lý hàng loạt
        Task SaveMultipleOrderItemsAsync(List<OrderItem> orderItems);
        Task DeleteOrderItemsByOrderIdAsync(int orderId);

        // tính toán và thống kê
        Task<decimal> CalculateOrderItemTotalPriceAsync(int orderItemId);
        Task<decimal> CalculateTotalPriceByOrderIdAsync(int orderId);
        Task<List<OrderItem>> GetPopularMenuItemsAsync(int topCount = 10);

        // cập nhật đặc biệt
        Task UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity);
    }
}
