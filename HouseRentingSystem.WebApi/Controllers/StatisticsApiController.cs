using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.Statistics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystem.WebApi.Controllers
{
	[Route("api/statistics")]
	[ApiController]
	public class StatisticsApiController : ControllerBase
	{
		private readonly IStatisticService _service;
		public StatisticsApiController(IStatisticService service)
		{
			_service = service;
		}

		/// <summary>
		/// Gets statistics about number of houses and rented houses
		/// </summary>
		/// <returns>total houses and total rents</returns>
		[HttpGet]
		[Produces("application/json")]
		[ProducesResponseType(200, Type = typeof(StatisticsServiceModel))]
		[ProducesResponseType(500)]
		public async Task<IActionResult> GetStatistics()
		{
			var model = await _service.Total();

			return Ok(model);
		}
	}
}
