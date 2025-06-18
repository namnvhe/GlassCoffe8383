using Cafe.BusinessObjects.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Cafe.DataAccess.DAO
{
    public class CoffeeTableDAO
    {
        public static async Task<List<CoffeeTable>> GetCoffeeTablesAsync()
        {
            var listTables = new List<CoffeeTable>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    listTables = await context.CoffeeTables
                        .Include(t => t.Orders)
                            .ThenInclude(o => o.OrderItems)
                        .OrderBy(t => t.TableNumber)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return listTables;
        }

        public static async Task<List<CoffeeTable>> GetAvailableTablesAsync()
        {
            var availableTables = new List<CoffeeTable>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    availableTables = await context.CoffeeTables
                        .Where(t => !t.Orders.Any(o => o.OrderTime.Date == today &&
                                                      (o.Status == "Pending" || o.Status == "In Progress")))
                        .OrderBy(t => t.TableNumber)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return availableTables;
        }

        public static async Task<List<CoffeeTable>> GetOccupiedTablesAsync()
        {
            var occupiedTables = new List<CoffeeTable>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    occupiedTables = await context.CoffeeTables
                        .Include(t => t.Orders.Where(o => o.OrderTime.Date == today &&
                                                         (o.Status == "Pending" || o.Status == "In Progress")))
                            .ThenInclude(o => o.OrderItems)
                                .ThenInclude(oi => oi.MenuItem)
                        .Where(t => t.Orders.Any(o => o.OrderTime.Date == today &&
                                                     (o.Status == "Pending" || o.Status == "In Progress")))
                        .OrderBy(t => t.TableNumber)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return occupiedTables;
        }

        public static async Task<CoffeeTable> FindCoffeeTableByIdAsync(int tableId)
        {
            CoffeeTable table = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    table = await context.CoffeeTables
                        .Include(t => t.Orders)
                            .ThenInclude(o => o.OrderItems)
                                .ThenInclude(oi => oi.MenuItem)
                        .Include(t => t.Orders)
                            .ThenInclude(o => o.User)
                        .SingleOrDefaultAsync(x => x.TableId == tableId);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        public static async Task<CoffeeTable> FindCoffeeTableByNumberAsync(int tableNumber)
        {
            CoffeeTable table = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    table = await context.CoffeeTables
                        .Include(t => t.Orders)
                        .SingleOrDefaultAsync(x => x.TableNumber == tableNumber);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        public static async Task<CoffeeTable> FindCoffeeTableByQRCodeAsync(string qrCode)
        {
            CoffeeTable table = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    table = await context.CoffeeTables
                        .Include(t => t.Orders.Where(o => o.Status == "Pending" || o.Status == "In Progress"))
                            .ThenInclude(o => o.OrderItems)
                                .ThenInclude(oi => oi.MenuItem)
                        .SingleOrDefaultAsync(x => x.Qrcode == qrCode);
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return table;
        }

        public static async Task<string> GetTableStatusAsync(int tableId)
        {
            string status = "Available";
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    var hasActiveOrder = await context.Orders
                        .AnyAsync(o => o.TableId == tableId &&
                                      o.OrderTime.Date == today &&
                                      (o.Status == "Pending" || o.Status == "In Progress"));

                    status = hasActiveOrder ? "Occupied" : "Available";
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return status;
        }

        public static async Task<Order> GetCurrentOrderByTableIdAsync(int tableId)
        {
            Order currentOrder = null;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var today = DateTime.Today;
                    currentOrder = await context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Include(o => o.User)
                        .Where(o => o.TableId == tableId &&
                                   o.OrderTime.Date == today &&
                                   (o.Status == "Pending" || o.Status == "In Progress"))
                        .OrderByDescending(o => o.OrderTime)
                        .FirstOrDefaultAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return currentOrder;
        }

        public static async Task<List<Order>> GetOrderHistoryByTableIdAsync(int tableId, int days = 30)
        {
            var orderHistory = new List<Order>();
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var startDate = DateTime.Today.AddDays(-days);
                    orderHistory = await context.Orders
                        .Include(o => o.OrderItems)
                            .ThenInclude(oi => oi.MenuItem)
                        .Include(o => o.User)
                        .Where(o => o.TableId == tableId && o.OrderTime >= startDate)
                        .OrderByDescending(o => o.OrderTime)
                        .ToListAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return orderHistory;
        }

        public static async Task<int> GetTotalTablesCountAsync()
        {
            int count = 0;
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    count = await context.CoffeeTables.CountAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
            return count;
        }

        public static async Task SaveCoffeeTableAsync(CoffeeTable table)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    // Tự động generate QR code nếu chưa có
                    if (string.IsNullOrEmpty(table.Qrcode))
                    {
                        table.Qrcode = $"TABLE_{table.TableNumber}_{Guid.NewGuid().ToString("N")[..8].ToUpper()}";
                    }

                    context.CoffeeTables.Add(table);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateCoffeeTableAsync(CoffeeTable table)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    context.Entry<CoffeeTable>(table).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task UpdateQRCodeAsync(int tableId, string newQRCode)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var table = await context.CoffeeTables
                        .SingleOrDefaultAsync(t => t.TableId == tableId);

                    if (table != null)
                    {
                        table.Qrcode = newQRCode;
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task DeleteCoffeeTableAsync(CoffeeTable table)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var existingTable = await context.CoffeeTables
                        .SingleOrDefaultAsync(c => c.TableId == table.TableId);

                    if (existingTable != null)
                    {
                        context.CoffeeTables.Remove(existingTable);
                        await context.SaveChangesAsync();
                    }
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }

        public static async Task<bool> CanDeleteTableAsync(int tableId)
        {
            try
            {
                using (var context = new CoffeManagerContext())
                {
                    var hasOrders = await context.Orders
                        .AnyAsync(o => o.TableId == tableId);

                    return !hasOrders;
                }
            }
            catch (Exception e)
            {
                throw new Exception(e.Message);
            }
        }
    }
}
