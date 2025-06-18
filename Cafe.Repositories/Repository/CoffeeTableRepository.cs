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
    public class CoffeeTableRepository : ICoffeeTableRepository
    {
        public async Task<List<CoffeeTable>> GetAllAsync() =>
            await CoffeeTableDAO.GetCoffeeTablesAsync();

        public async Task<CoffeeTable> FindCoffeeTableByIdAsync(int tableId) =>
            await CoffeeTableDAO.FindCoffeeTableByIdAsync(tableId);

        public async Task<CoffeeTable> FindCoffeeTableByNumberAsync(int tableNumber) =>
            await CoffeeTableDAO.FindCoffeeTableByNumberAsync(tableNumber);

        public async Task<CoffeeTable> FindCoffeeTableByQRCodeAsync(string qrCode) =>
            await CoffeeTableDAO.FindCoffeeTableByQRCodeAsync(qrCode);

        public async Task<List<CoffeeTable>> GetAvailableTablesAsync() =>
            await CoffeeTableDAO.GetAvailableTablesAsync();

        public async Task<List<CoffeeTable>> GetOccupiedTablesAsync() =>
            await CoffeeTableDAO.GetOccupiedTablesAsync();

        public async Task<string> GetTableStatusAsync(int tableId) =>
            await CoffeeTableDAO.GetTableStatusAsync(tableId);

        public async Task<Order> GetCurrentOrderByTableIdAsync(int tableId) =>
            await CoffeeTableDAO.GetCurrentOrderByTableIdAsync(tableId);

        public async Task<List<Order>> GetOrderHistoryByTableIdAsync(int tableId, int days = 30) =>
            await CoffeeTableDAO.GetOrderHistoryByTableIdAsync(tableId, days);

        public async Task<int> GetTotalTablesCountAsync() =>
            await CoffeeTableDAO.GetTotalTablesCountAsync();

        public async Task<bool> CanDeleteTableAsync(int tableId) =>
            await CoffeeTableDAO.CanDeleteTableAsync(tableId);

        public async Task SaveCoffeeTableAsync(CoffeeTable table) =>
            await CoffeeTableDAO.SaveCoffeeTableAsync(table);

        public async Task UpdateCoffeeTableAsync(CoffeeTable table) =>
            await CoffeeTableDAO.UpdateCoffeeTableAsync(table);

        public async Task UpdateQRCodeAsync(int tableId, string newQRCode) =>
            await CoffeeTableDAO.UpdateQRCodeAsync(tableId, newQRCode);

        public async Task DeleteCoffeeTableAsync(CoffeeTable table) =>
            await CoffeeTableDAO.DeleteCoffeeTableAsync(table);
    }
}
