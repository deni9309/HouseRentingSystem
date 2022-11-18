using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Exceptions;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
    public class HouseService : IHouseService
    {
        private readonly IRepository _repo;
        private readonly IGuard _guard;

        public HouseService(IRepository repo, IGuard guard)
        {
            _repo = repo;
            _guard = guard;
        }

        public async Task<HousesQueryModel> All(string? category = null, string? searchTerm = null, HouseSorting sorting = HouseSorting.Newest, int currentPage = 1, int housesPerPage = 1)
        {
            var result = new HousesQueryModel();

            var houses = _repo.AllReadonly<House>()
                .Where(h => h.IsActive);

            if (string.IsNullOrWhiteSpace(category) == false)
            {
                houses = houses
                    .Where(h => h.Category.Name == category);
            }

            if (string.IsNullOrEmpty(searchTerm) == false)
            {
                searchTerm = $"%{searchTerm.ToLower()}%";

                houses = houses
                    .Where(h =>
                    EF.Functions.Like(h.Title.ToLower(), searchTerm) ||
                    EF.Functions.Like(h.Address.ToLower(), searchTerm) ||
                    EF.Functions.Like(h.Description.ToLower(), searchTerm));
            }

            houses = sorting switch
            {
                HouseSorting.Price => houses
                .OrderBy(h => h.PricePerMonth),
                HouseSorting.NotRentedFirst => houses
                .OrderBy(h => h.RenterId),
                _ => houses.OrderByDescending(h => h.Id)
            };

            result.Houses = await houses
                .Skip((currentPage - 1) * housesPerPage)
                .Take(housesPerPage)
                .Select(h => new HouseServiceModel()
                {
                    Address = h.Address,
                    Id = h.Id,
                    IsRented = h.RenterId != null,
                    PricePerMonth = h.PricePerMonth,
                    Title = h.Title,
                    Description = h.Description,
                    ImageUrl = h.ImageUrl
                })
                .ToListAsync();

            result.TotalHousesCount = await houses.CountAsync();

            return result;
        }

        public async Task<IEnumerable<HouseCategoryModel>> AllCategories()
        {
            return await _repo.AllReadonly<Category>()
                .OrderBy(c => c.Name)
                .Select(c => new HouseCategoryModel()
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync();
        }

        public async Task<IEnumerable<string>> AllCategoryNames()
        {
            return await _repo.AllReadonly<Category>()
                .Select(c => c.Name)
                .Distinct()
                .ToListAsync();
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByAgentId(int id)
        {
            return await _repo.AllReadonly<House>()
                .Where(h => h.IsActive)
                .Where(c => c.AgentId == id)
                .Select(c => new HouseServiceModel()
                {
                    Address = c.Address,
                    Id = c.Id,
                    ImageUrl = c.ImageUrl,
                    IsRented = c.RenterId != null,
                    PricePerMonth = c.PricePerMonth,
                    Title = c.Title
                })
                 .ToListAsync();
        }

        public async Task<IEnumerable<HouseServiceModel>> AllHousesByUserId(string userId)
        {
            return await _repo.AllReadonly<House>()
                 .Where(h => h.IsActive)
                 .Where(c => c.RenterId == userId)
                 .Select(c => new HouseServiceModel()
                 {
                     Address = c.Address,
                     Id = c.Id,
                     ImageUrl = c.ImageUrl,
                     IsRented = c.RenterId != null,
                     PricePerMonth = c.PricePerMonth,
                     Title = c.Title
                 })
                 .ToListAsync();
        }

        public async Task<bool> CategoryExists(int categoryId)
        {
            return await _repo.AllReadonly<Category>()
                .AnyAsync(c => c.Id == categoryId);
        }

        public async Task<int> Create(HouseModel model, int agentId)
        {
            var house = new House()
            {
                Title = model.Title,
                Address = model.Address,
                Description = model.Description,
                CategoryId = model.CategoryId,
                ImageUrl = model.ImageUrl,
                PricePerMonth = model.PricePerMonth,
                AgentId = agentId
            };

            await _repo.AddAsync(house);
            await _repo.SaveChangesAsync();

            return house.Id;
        }

        public async Task Delete(int houseId)
        {
            var house = await _repo.GetByIdAsync<House>(houseId);

            house.IsActive = false;

            await _repo.SaveChangesAsync();
        }

        public async Task Edit(int houseId, HouseModel model)
        {
            var house = await _repo.GetByIdAsync<House>(houseId);

            house.Description = model.Description;
            house.ImageUrl = model.ImageUrl;
            house.Address = model.Address;
            house.CategoryId = model.CategoryId;
            house.PricePerMonth = model.PricePerMonth;
            house.Title = model.Title;

            await _repo.SaveChangesAsync();
        }

        public async Task<bool> Exists(int id)
        {
            return await _repo.AllReadonly<House>()
                .AnyAsync(h => h.Id == id && h.IsActive);
        }

        public async Task<int> GetHouseCategoryId(int id)
        {
            return (await _repo.GetByIdAsync<House>(id)).CategoryId;

        }

        public async Task<IEnumerable<HouseHomeModel>> GetLastThreeHouses()
        {
            return await _repo.AllReadonly<House>()
                .Where(h => h.IsActive)
                .OrderByDescending(x => x.Id)
                .Select(x => new HouseHomeModel()
                {
                    Id = x.Id,
                    Title = x.Title,
                    ImageUrl = x.ImageUrl
                })
                .Take(3)
                .ToListAsync();
        }

        public async Task<bool> HasAgentWithId(int houseId, string currentUserId)
        {
            bool result = false;

            var house = await _repo.AllReadonly<House>()
                .Where(h => h.IsActive)
                .Where(h => h.Id == houseId)
                .Include(h => h.Agent)
                .FirstOrDefaultAsync();

            if (house?.Agent != null && house.Agent.UserId == currentUserId)
            {
                result = true;
            }

            return result;
        }

        public async Task<HouseDetailsModel> HouseDetailsById(int id)
        {
            return await _repo.AllReadonly<House>()
                 .Where(h => h.IsActive)
                .Where(h => h.Id == id)
                .Select(h => new HouseDetailsModel()
                {
                    Address = h.Address,
                    Category = h.Category.Name,
                    Description = h.Description,
                    Id = id,
                    ImageUrl = h.ImageUrl,
                    IsRented = h.RenterId != null,
                    PricePerMonth = h.PricePerMonth,
                    Title = h.Title,
                    Agent = new Models.Agent.AgentServiceModel()
                    {
                        Email = h.Agent.User.Email,
                        PhoneNumber = h.Agent.PhoneNumber
                    }

                }).FirstAsync();
        }

        public async Task<bool> IsRented(int houseId)
        {
            return (await _repo.GetByIdAsync<House>(houseId)).RenterId != null;
        }

        public async Task<bool> IsRentedByUserWithId(int houseId, string currentUserId)
        {
            bool result = false;

            var house = await _repo.AllReadonly<House>()
                .Where(h => h.Id == houseId)
                .Where(h => h.IsActive)
                .FirstOrDefaultAsync();

            if (house != null && house.RenterId == currentUserId)
            {
                result = true;
            }

            return result;
        }

        public async Task Leave(int houseId)
        {
            var house = await _repo.GetByIdAsync<House>(houseId);
            _guard.AgainstNull(house, "House can not be found!");
            
            house.RenterId = null;

            await _repo.SaveChangesAsync();
        }

        public async Task Rent(int houseId, string currentUserId)
        {
            var house = await _repo.GetByIdAsync<House>(houseId);

            if(house !=null && house.RenterId !=null)
            {
                throw new ArgumentException("House is already rented!");
            }

            _guard.AgainstNull(house, "House can not be found!");

            house.RenterId = currentUserId;

            await _repo.SaveChangesAsync();
        }
    }
}
