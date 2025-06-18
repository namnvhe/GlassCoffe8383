using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IOrderItemToppingRepository
    {
        Task<List<OrderItemTopping>> GetAllAsync();
        Task<OrderItemTopping> FindOrderItemToppingByIdAsync(int orderItemToppingId);
        Task SaveOrderItemToppingAsync(OrderItemTopping orderItemTopping);
        Task UpdateOrderItemToppingAsync(OrderItemTopping orderItemTopping);
        Task DeleteOrderItemToppingAsync(OrderItemTopping orderItemTopping);

        // tìm kiếm theo quan hệ
        Task<List<OrderItemTopping>> GetOrderItemToppingsByOrderItemIdAsync(int orderItemId);
        Task<List<OrderItemTopping>> GetOrderItemToppingsByToppingIdAsync(int toppingId);
        Task<OrderItemTopping> FindOrderItemToppingByCompositeKeyAsync(int orderItemId, int toppingId);

        // xử lý hàng loạt
        Task SaveMultipleOrderItemToppingsAsync(List<OrderItemTopping> orderItemToppings);
        Task DeleteOrderItemToppingsByOrderItemIdAsync(int orderItemId);

        // tính toán
        Task<decimal> CalculateTotalToppingPriceByOrderItemAsync(int orderItemId);
    }
}
