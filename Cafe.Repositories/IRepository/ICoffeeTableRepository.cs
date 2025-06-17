using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface ICoffeeTableRepository
    {
        Task<List<CoffeeTable>> GetAllAsync();
        Task<CoffeeTable> FindCoffeeTableByIdAsync(int tableId);
        Task<CoffeeTable> FindCoffeeTableByNumberAsync(int tableNumber);
        Task<CoffeeTable> FindCoffeeTableByQRCodeAsync(string qrCode);
        Task SaveCoffeeTableAsync(CoffeeTable table);
        Task UpdateCoffeeTableAsync(CoffeeTable table);
        Task DeleteCoffeeTableAsync(CoffeeTable table);

        // quản lý trạng thái bàn
        Task<List<CoffeeTable>> GetAvailableTablesAsync();
        Task<List<CoffeeTable>> GetOccupiedTablesAsync();
        Task<string> GetTableStatusAsync(int tableId);

        // quản lý đơn hàng theo bàn
        Task<Order> GetCurrentOrderByTableIdAsync(int tableId);
        Task<List<Order>> GetOrderHistoryByTableIdAsync(int tableId, int days = 30);

        // cập nhật đặc biệt
        Task UpdateQRCodeAsync(int tableId, string newQRCode);

        // thống kê và validation
        Task<int> GetTotalTablesCountAsync();
        Task<bool> CanDeleteTableAsync(int tableId);
    }
}
