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
    public class ToppingRepository : IToppingRepository
    {
        public async Task<List<Topping>> GetAllAsync() => await ToppingDAO.GetToppingsAsync();

        public async Task<List<Topping>> GetAvailableToppingsAsync() => await ToppingDAO.GetAvailableToppingsAsync();

        public async Task<Topping> FindToppingByIdAsync(int toppingId) => await ToppingDAO.FindToppingByIdAsync(toppingId);

        public async Task<List<Topping>> FindToppingsByNameAsync(string name) => await ToppingDAO.FindToppingsByNameAsync(name);

        public async Task<List<Topping>> GetToppingsByPriceRangeAsync(decimal minPrice, decimal maxPrice) => await ToppingDAO.GetToppingsByPriceRangeAsync(minPrice, maxPrice);

        public async Task SaveToppingAsync(Topping topping) => await ToppingDAO.SaveToppingAsync(topping);

        public async Task UpdateToppingAsync(Topping topping) => await ToppingDAO.UpdateToppingAsync(topping);

        public async Task DeleteToppingAsync(Topping topping) => await ToppingDAO.DeleteToppingAsync(topping);

        public async Task UpdateToppingAvailabilityAsync(int toppingId, bool isAvailable) => await ToppingDAO.UpdateToppingAvailabilityAsync(toppingId, isAvailable);
    }
}
