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
    public class OrderItemToppingRepository : IOrderItemToppingRepository
    {
        public async Task<List<OrderItemTopping>> GetAllAsync() =>
            await OrderItemToppingDAO.GetOrderItemToppingsAsync();

        public async Task<OrderItemTopping> FindOrderItemToppingByIdAsync(int orderItemToppingId) =>
            await OrderItemToppingDAO.FindOrderItemToppingByIdAsync(orderItemToppingId);

        public async Task<List<OrderItemTopping>> GetOrderItemToppingsByOrderItemIdAsync(int orderItemId) =>
            await OrderItemToppingDAO.GetOrderItemToppingsByOrderItemIdAsync(orderItemId);

        public async Task<List<OrderItemTopping>> GetOrderItemToppingsByToppingIdAsync(int toppingId) =>
            await OrderItemToppingDAO.GetOrderItemToppingsByToppingIdAsync(toppingId);

        public async Task<OrderItemTopping> FindOrderItemToppingByCompositeKeyAsync(int orderItemId, int toppingId) =>
            await OrderItemToppingDAO.FindOrderItemToppingByCompositeKeyAsync(orderItemId, toppingId);

        public async Task<decimal> CalculateTotalToppingPriceByOrderItemAsync(int orderItemId) =>
            await OrderItemToppingDAO.CalculateTotalToppingPriceByOrderItemAsync(orderItemId);

        public async Task SaveOrderItemToppingAsync(OrderItemTopping orderItemTopping) =>
            await OrderItemToppingDAO.SaveOrderItemToppingAsync(orderItemTopping);

        public async Task SaveMultipleOrderItemToppingsAsync(List<OrderItemTopping> orderItemToppings) =>
            await OrderItemToppingDAO.SaveMultipleOrderItemToppingsAsync(orderItemToppings);

        public async Task UpdateOrderItemToppingAsync(OrderItemTopping orderItemTopping) =>
            await OrderItemToppingDAO.UpdateOrderItemToppingAsync(orderItemTopping);

        public async Task DeleteOrderItemToppingAsync(OrderItemTopping orderItemTopping) =>
            await OrderItemToppingDAO.DeleteOrderItemToppingAsync(orderItemTopping);

        public async Task DeleteOrderItemToppingsByOrderItemIdAsync(int orderItemId) =>
            await OrderItemToppingDAO.DeleteOrderItemToppingsByOrderItemIdAsync(orderItemId);
    }
}
