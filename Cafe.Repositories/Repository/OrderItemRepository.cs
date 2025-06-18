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
    public class OrderItemRepository : IOrderItemRepository
    {
        public async Task<List<OrderItem>> GetAllAsync() =>
            await OrderItemDAO.GetOrderItemsAsync();

        public async Task<OrderItem> FindOrderItemByIdAsync(int orderItemId) =>
            await OrderItemDAO.FindOrderItemByIdAsync(orderItemId);

        public async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId) =>
            await OrderItemDAO.GetOrderItemsByOrderIdAsync(orderId);

        public async Task<List<OrderItem>> GetOrderItemsByMenuItemIdAsync(int menuItemId) =>
            await OrderItemDAO.GetOrderItemsByMenuItemIdAsync(menuItemId);

        public async Task<decimal> CalculateOrderItemTotalPriceAsync(int orderItemId) =>
            await OrderItemDAO.CalculateOrderItemTotalPriceAsync(orderItemId);

        public async Task<decimal> CalculateTotalPriceByOrderIdAsync(int orderId) =>
            await OrderItemDAO.CalculateTotalPriceByOrderIdAsync(orderId);

        public async Task<List<OrderItem>> GetPopularMenuItemsAsync(int topCount = 10) =>
            await OrderItemDAO.GetPopularMenuItemsAsync(topCount);

        public async Task SaveOrderItemAsync(OrderItem orderItem) =>
            await OrderItemDAO.SaveOrderItemAsync(orderItem);

        public async Task SaveMultipleOrderItemsAsync(List<OrderItem> orderItems) =>
            await OrderItemDAO.SaveMultipleOrderItemsAsync(orderItems);

        public async Task UpdateOrderItemAsync(OrderItem orderItem) =>
            await OrderItemDAO.UpdateOrderItemAsync(orderItem);

        public async Task UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity) =>
            await OrderItemDAO.UpdateOrderItemQuantityAsync(orderItemId, newQuantity);

        public async Task DeleteOrderItemAsync(OrderItem orderItem) =>
            await OrderItemDAO.DeleteOrderItemAsync(orderItem);

        public async Task DeleteOrderItemsByOrderIdAsync(int orderId) =>
            await OrderItemDAO.DeleteOrderItemsByOrderIdAsync(orderId);
    }
}
