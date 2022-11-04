using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HouseRentingSystem.Core.Services
{
    public class HouseService :IHouseService
    {
        private readonly IRepository _repo;

        public HouseService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task<IEnumerable<HouseHomeModel>> GetLastThreeHouses()
        {
            return await _repo.AllReadonly<House>()
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
    }
}
