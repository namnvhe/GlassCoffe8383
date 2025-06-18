using Cafe.BusinessObjects.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cafe.Repositories.IRepository
{
    public interface IToppingRepository
    {
        Task<List<Topping>> GetAllAsync();
        Task<List<Topping>> GetAvailableToppingsAsync();
        Task<Topping> FindToppingByIdAsync(int toppingId);
        Task SaveToppingAsync(Topping topping);
        Task UpdateToppingAsync(Topping topping);
        Task DeleteToppingAsync(Topping topping);

        // tìm kiếm nâng cao
        Task<List<Topping>> FindToppingsByNameAsync(string name);
        Task<List<Topping>> GetToppingsByPriceRangeAsync(decimal minPrice, decimal maxPrice);

        // quản lý trạng thái
        Task UpdateToppingAvailabilityAsync(int toppingId, bool isAvailable);
    }
}
