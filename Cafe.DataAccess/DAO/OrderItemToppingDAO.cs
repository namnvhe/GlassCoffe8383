using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class OrderItemToppingDAO
    {
        public static async Task<List<OrderItemTopping>> GetOrderItemToppingsAsync()
        {
            var listOrderItemToppings = new List<OrderItemTopping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listOrderItemToppings = await context.OrderItemToppings
                        .Include(ot => ot.OrderItem)
                        .Include(ot => ot.Topping)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listOrderItemToppings;
        }

        public static async Task<List<OrderItemTopping>> GetOrderItemToppingsByOrderItemIdAsync(int orderItemId)
        {
            var orderItemToppings = new List<OrderItemTopping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orderItemToppings = await context.OrderItemToppings
                        .Include(ot => ot.Topping)
                        .Where(ot => ot.OrderItemId == orderItemId)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orderItemToppings;
        }

        public static async Task<List<OrderItemTopping>> GetOrderItemToppingsByToppingIdAsync(int toppingId)
        {
            var orderItemToppings = new List<OrderItemTopping>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orderItemToppings = await context.OrderItemToppings
                        .Include(ot => ot.OrderItem)
                        .Where(ot => ot.ToppingId == toppingId)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orderItemToppings;
        }

        public static async Task<OrderItemTopping> FindOrderItemToppingByIdAsync(int orderItemToppingId)
        {
            OrderItemTopping ot = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ot = await context.OrderItemToppings
                        .Include(orderItemTopping => orderItemTopping.OrderItem)
                        .Include(orderItemTopping => orderItemTopping.Topping)
                        .SingleOrDefaultAsync(x => x.OrderItemToppingId == orderItemToppingId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ot;
        }

        public static async Task<OrderItemTopping> FindOrderItemToppingByCompositeKeyAsync(int orderItemId, int toppingId)
        {
            OrderItemTopping ot = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    ot = await context.OrderItemToppings
                        .Include(orderItemTopping => orderItemTopping.OrderItem)
                        .Include(orderItemTopping => orderItemTopping.Topping)
                        .SingleOrDefaultAsync(x => x.OrderItemId == orderItemId && x.ToppingId == toppingId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return ot;
        }

        public static async Task<decimal> CalculateTotalToppingPriceByOrderItemAsync(int orderItemId)
        {
            decimal totalPrice = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    totalPrice = await context.OrderItemToppings
                        .Include(ot => ot.Topping)
                        .Where(ot => ot.OrderItemId == orderItemId)
                        .SumAsync(ot => ot.Quantity * ot.Topping.Price);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalPrice;
        }

        public static async Task SaveOrderItemToppingAsync(OrderItemTopping ot)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.OrderItemToppings.Add(ot);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task SaveMultipleOrderItemToppingsAsync(List<OrderItemTopping> orderItemToppings)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.OrderItemToppings.AddRange(orderItemToppings);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateOrderItemToppingAsync(OrderItemTopping ot)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<OrderItemTopping>(ot).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteOrderItemToppingAsync(OrderItemTopping ot)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingOrderItemTopping = await context.OrderItemToppings
                        .SingleOrDefaultAsync(c => c.OrderItemToppingId == ot.OrderItemToppingId);
                    if (existingOrderItemTopping != null)
                    {
                        context.OrderItemToppings.Remove(existingOrderItemTopping);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteOrderItemToppingsByOrderItemIdAsync(int orderItemId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orderItemToppings = await context.OrderItemToppings
                        .Where(ot => ot.OrderItemId == orderItemId)
                        .ToListAsync();

                    if (orderItemToppings.Any())
                    {
                        context.OrderItemToppings.RemoveRange(orderItemToppings);
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
