using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class OrderDAO
    {
        public static async Task<List<Order>> GetOrdersAsync()
        {
            var listOrders = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listOrders = await context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Table)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.OrderItemToppings)
                                .ThenInclude(ot => ot.Topping)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listOrders;
        }

        public static async Task<List<Order>> GetOrdersByUserIdAsync(int userId)
        {
            var orders = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orders = await context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Table)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Where(o => o.UserId == userId)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orders;
        }

        public static async Task<List<Order>> GetOrdersByTableIdAsync(int tableId)
        {
            var orders = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orders = await context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Table)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Where(o => o.TableId == tableId)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orders;
        }

        public static async Task<List<Order>> GetOrdersByStatusAsync(string status)
        {
            var orders = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orders = await context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Table)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Where(o => o.Status == status)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orders;
        }

        public static async Task<List<Order>> GetOrdersByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            var orders = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    orders = await context.Orders
                        .Include(o => o.User)
                        .Include(o => o.Table)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Where(o => o.OrderTime >= startDate && o.OrderTime <= endDate)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orders;
        }

        public static async Task<Order> FindOrderByIdAsync(int orderId)
        {
            Order o = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    o = await context.Orders
                        .Include(ord => ord.User)
                        .Include(ord => ord.Table)
                        .Include(ord => ord.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Include(ord => ord.OrderItems)
                            .ThenInclude(od => od.OrderItemToppings)
                                .ThenInclude(ot => ot.Topping)
                        .SingleOrDefaultAsync(x => x.OrderId == orderId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return o;
        }

        public static async Task<decimal> CalculateOrderTotalAsync(int orderId)
        {
            decimal total = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var order = await context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.OrderItemToppings)
                                .ThenInclude(ot => ot.Topping)
                        .SingleOrDefaultAsync(o => o.OrderId == orderId);

                    if (order != null)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            var basePrice = item.MenuItem.Price * item.Quantity;
                            var toppingsPrice = item.OrderItemToppings
                                .Sum(ot => ot.Topping.Price * ot.Quantity);
                            total += basePrice + toppingsPrice;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return total;
        }

        public static async Task<decimal> GetTotalRevenueByDateRangeAsync(DateTime startDate, DateTime endDate)
        {
            decimal totalRevenue = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var orders = await context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.MenuItem)
                        .Include(o => o.OrderItems)
                            .ThenInclude(od => od.OrderItemToppings)
                                .ThenInclude(ot => ot.Topping)
                        .Where(o => o.OrderTime >= startDate && o.OrderTime <= endDate
                                && o.Status == "Completed")
                        .ToListAsync();

                    foreach (var order in orders)
                    {
                        foreach (var item in order.OrderItems)
                        {
                            var basePrice = item.MenuItem.Price * item.Quantity;
                            var toppingsPrice = item.OrderItemToppings
                                .Sum(ot => ot.Topping.Price * ot.Quantity);
                            totalRevenue += basePrice + toppingsPrice;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return totalRevenue;
        }

        public static async Task SaveOrderAsync(Order o)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Orders.Add(o);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateOrderAsync(Order o)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<Order>(o).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateOrderStatusAsync(int orderId, string status)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var order = await context.Orders.SingleOrDefaultAsync(o => o.OrderId == orderId);
                    if (order != null)
                    {
                        order.Status = status;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteOrderAsync(Order o)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingOrder = await context.Orders.SingleOrDefaultAsync(c => c.OrderId == o.OrderId);
                    if (existingOrder != null)
                    {
                        context.Orders.Remove(existingOrder);
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
