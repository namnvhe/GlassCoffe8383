using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class OrderItemDAO
    {
        public static async Task<List<OrderItem>> GetOrderItemsAsync()
        {
            var listOrderItems = new List<OrderItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listOrderItems = await context.OrderItems
                        .Include(od => od.MenuItem)
                        .Include(od => od.Order)
                        .Include(od => od.OrderItemToppings)
                            .ThenInclude(ot => ot.Topping)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listOrderItems;
        }

        public static async Task<List<OrderItem>> GetOrderItemsByOrderIdAsync(int orderId)
        {
            var orderItems = new List<OrderItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orderItems = await context.OrderItems
                        .Include(od => od.MenuItem)
                        .Include(od => od.OrderItemToppings)
                            .ThenInclude(ot => ot.Topping)
                        .Where(od => od.OrderId == orderId)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orderItems;
        }

        public static async Task<List<OrderItem>> GetOrderItemsByMenuItemIdAsync(int menuItemId)
        {
            var orderItems = new List<OrderItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orderItems = await context.OrderItems
                        .Include(od => od.Order)
                        .Include(od => od.MenuItem)
                        .Where(od => od.MenuItemId == menuItemId)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orderItems;
        }

        public static async Task<OrderItem> FindOrderItemByIdAsync(int orderItemId)
        {
            OrderItem od = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    od = await context.OrderItems
                        .Include(detail => detail.MenuItem)
                        .Include(detail => detail.Order)
                        .Include(detail => detail.OrderItemToppings)
                            .ThenInclude(ot => ot.Topping)
                        .SingleOrDefaultAsync(x => x.OrderItemId == orderItemId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return od;
        }

        public static async Task<decimal> CalculateOrderItemTotalPriceAsync(int orderItemId)
        {
            decimal totalPrice = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orderItem = await context.OrderItems
                        .Include(od => od.MenuItem)
                        .Include(od => od.OrderItemToppings)
                            .ThenInclude(ot => ot.Topping)
                        .SingleOrDefaultAsync(od => od.OrderItemId == orderItemId);

                    if (orderItem != null)
                    {
                        var basePrice = orderItem.MenuItem.Price * orderItem.Quantity;
                        var toppingsPrice = orderItem.OrderItemToppings
                            .Sum(ot => ot.Topping.Price * ot.Quantity);
                        totalPrice = basePrice + toppingsPrice;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalPrice;
        }

        public static async Task<decimal> CalculateTotalPriceByOrderIdAsync(int orderId)
        {
            decimal totalPrice = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orderItems = await context.OrderItems
                        .Include(od => od.MenuItem)
                        .Include(od => od.OrderItemToppings)
                            .ThenInclude(ot => ot.Topping)
                        .Where(od => od.OrderId == orderId)
                        .ToListAsync();

                    foreach (var item in orderItems)
                    {
                        var basePrice = item.MenuItem.Price * item.Quantity;
                        var toppingsPrice = item.OrderItemToppings
                            .Sum(ot => ot.Topping.Price * ot.Quantity);
                        totalPrice += basePrice + toppingsPrice;
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalPrice;
        }

        public static async Task<List<OrderItem>> GetPopularMenuItemsAsync(int topCount = 10)
        {
            var popularItems = new List<OrderItem>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    popularItems = await context.OrderItems
                        .Include(od => od.MenuItem)
                        .GroupBy(od => od.MenuItemId)
                        .OrderByDescending(g => g.Sum(od => od.Quantity))
                        .Take(topCount)
                        .SelectMany(g => g.Take(1))
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return popularItems;
        }

        public static async Task SaveOrderItemAsync(OrderItem od)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.OrderItems.Add(od);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task SaveMultipleOrderItemsAsync(List<OrderItem> orderItems)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.OrderItems.AddRange(orderItems);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateOrderItemAsync(OrderItem od)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<OrderItem>(od).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateOrderItemQuantityAsync(int orderItemId, int newQuantity)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orderItem = await context.OrderItems
                        .SingleOrDefaultAsync(od => od.OrderItemId == orderItemId);

                    if (orderItem != null)
                    {
                        orderItem.Quantity = newQuantity;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteOrderItemAsync(OrderItem od)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingOrderItem = await context.OrderItems
                        .SingleOrDefaultAsync(c => c.OrderItemId == od.OrderItemId);

                    if (existingOrderItem != null)
                    {
                        context.OrderItems.Remove(existingOrderItem);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteOrderItemsByOrderIdAsync(int orderId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orderItems = await context.OrderItems
                        .Where(od => od.OrderId == orderId)
                        .ToListAsync();

                    if (orderItems.Any())
                    {
                        context.OrderItems.RemoveRange(orderItems);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
