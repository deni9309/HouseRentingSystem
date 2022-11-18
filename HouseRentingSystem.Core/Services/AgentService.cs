using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Infrastructure.Data;
using HouseRentingSystem.Infrastructure.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace HouseRentingSystem.Core.Services
{
    public class AgentService : IAgentService
    {
        private readonly IRepository _repo;

        public AgentService(IRepository repo)
        {
            _repo = repo;
        }

        public async Task CreateAsync(string userId, string phoneNumber)
        {
            var agent = new Agent()
            {
                UserId = userId,
                PhoneNumber = phoneNumber
            };

            await _repo.AddAsync(agent);
            await _repo.SaveChangesAsync();
        }

        public async Task<bool> ExistsByIdAsync(string userId)
        {
            return await _repo.All<Agent>()
                .AnyAsync(a => a.UserId == userId);
        }

        public async Task<int> GetAgentIdAsync(string userId)
        {
            return (await _repo.AllReadonly<Agent>()
                .FirstOrDefaultAsync(a => a.UserId == userId))?.Id ?? 0; ;
        }

        public async Task<bool> UserHasRentsAsync(string userId)
        {
            return await _repo.All<House>()
               .AnyAsync(h => h.RenterId == userId);
        }

        public async Task<bool> UserWithPhoneNumberExistsAsync(string phoneNumber)
        {
            return await _repo.All<Agent>()
               .AnyAsync(a => a.PhoneNumber == phoneNumber);
        }
    }
}
