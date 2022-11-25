using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.Statistics;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
	public class StatisticService : IStatisticService
	{

		private readonly IRepository _repo;

		public StatisticService(IRepository repo)
		{
			_repo = repo;
		}

		public async Task<StatisticsServiceModel> Total()
		{
			int totalHouses = await _repo.AllReadonly<House>()
				.CountAsync(h => h.IsActive);

			int rentHouses = await _repo.AllReadonly<House>()
				.CountAsync(h => h.IsActive && h.RenterId != null);

			return new StatisticsServiceModel()
			{
				TotalHouses = totalHouses,
				TotalRents = rentHouses
			};
		}
	}
}
