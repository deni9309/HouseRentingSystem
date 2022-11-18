using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.House;
using HouseRentingSystem.Extensions;
using HouseRentingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystem.Controllers
{
    [Authorize]
    public class HouseController : Controller
    {
        private readonly IHouseService _houseService;
        private readonly IAgentService _agentService;

        public HouseController(
            IHouseService houseService, 
            IAgentService agentService)
        {
            _houseService = houseService;
            _agentService = agentService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> All([FromQuery]AllHousesQueryModel queryModel)
        {
            var result = await _houseService.All(
                queryModel.Category,
                queryModel.SearchTerm,
                queryModel.Sorting,
                queryModel.CurrentPage,
                AllHousesQueryModel.HousesPerPage);

            queryModel.TotalHousesCount = result.TotalHousesCount;
            queryModel.Categories = await _houseService.AllCategoryNames();
            queryModel.Houses = result.Houses;

            return View(queryModel);
        }

        [HttpGet]
        public async Task<IActionResult> Mine()
        {
            IEnumerable<HouseServiceModel> myHouses;

            var userId = User.Id();

            if(await _agentService.ExistsByIdAsync(userId))
            {
                int agentId = await _agentService.GetAgentIdAsync(userId);
                myHouses = await _houseService.AllHousesByAgentId(agentId);
            }
            else
            {
                myHouses = await _houseService.AllHousesByUserId(userId);
            }

            return View(myHouses);
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            if((await _houseService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            var model = await _houseService.HouseDetailsById(id);

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Add()
        {
            if (await _agentService.ExistsByIdAsync(User.Id()) == false)
            {
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            var model = new HouseModel()
            {
                HouseCategories = await _houseService.AllCategories()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(HouseModel model)
        {
            if (await _agentService.ExistsByIdAsync(User.Id()) == false)
            {
                return RedirectToAction(nameof(AgentController.Become), "Agent");
            }

            if ((await _houseService.CategoryExists(model.CategoryId)) == false)
            {
                ModelState.AddModelError(nameof(model.CategoryId), "Category does not exists!");
            }

            if (!ModelState.IsValid)
            {
                model.HouseCategories = await _houseService.AllCategories();    

                return View(model); 
            }

            int agentId = await _agentService.GetAgentIdAsync(User.Id());

            int id = await _houseService.Create(model, agentId);

            return RedirectToAction(nameof(Details), new { id });
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if((await _houseService.Exists(id))==false)
            {
                return RedirectToAction(nameof(All));
            }

            if((await _houseService.HasAgentWithId(id, User.Id()))== false)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
			}

            var house = await _houseService.HouseDetailsById(id);
            var categoryId = await _houseService.GetHouseCategoryId(id);

			var model = new HouseModel()
            {
                Id = house.Id,
                Address = house.Address,
                PricePerMonth = house.PricePerMonth,
                Description = house.Description,
                Title = house.Title,
                HouseCategories = await _houseService.AllCategories(),
                ImageUrl = house.ImageUrl,
                CategoryId = categoryId
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, HouseModel model)
        {
            if(id != model.Id)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });

            }
            if ((await _houseService.Exists(model.Id)) == false)
			{
				model.HouseCategories = await _houseService.AllCategories();
				ModelState.AddModelError("", "House does not exist!");
				return View(model);
			}

			if ((await _houseService.HasAgentWithId(model.Id, User.Id())) == false)
			{
				return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
			}

            if ((await _houseService.CategoryExists(model.CategoryId)) == false)
            {
				model.HouseCategories = await _houseService.AllCategories();
				ModelState.AddModelError(nameof(model.CategoryId), "Category does not exist!");
				return View(model);
			}

			if (!ModelState.IsValid)
			{
				model.HouseCategories = await _houseService.AllCategories();
				return View(model);
			}
			await _houseService.Edit(model.Id, model);
			return RedirectToAction(nameof(Details), new { model.Id});
        }

        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if ((await _houseService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }
            if ((await _houseService.HasAgentWithId(id, User.Id())) == false)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            var house = await _houseService.HouseDetailsById(id);

            var model = new HouseDetailsViewModel()
            {
                Address = house.Address,
                Title = house.Title,
                ImageUrl = house.ImageUrl
            };

            return View(model);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(int id, HouseDetailsViewModel model)
        {
            if ((await _houseService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            if ((await _houseService.HasAgentWithId(id, User.Id())) == false)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            await _houseService.Delete(id);

            return RedirectToAction(nameof(All));
        }

        [HttpPost]
        public async Task<IActionResult> Rent(int id)
        {
            if ((await _houseService.Exists(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            if (await _agentService.ExistsByIdAsync(User.Id()))
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }
            if(await _houseService.IsRented(id))
            {
                return RedirectToAction(nameof(All));
            }
            await _houseService.Rent(id, User.Id());

            return RedirectToAction(nameof(Mine));
        }

        [HttpPost]
        public async Task<IActionResult> Leave(int id)
        {
            if ((await _houseService.Exists(id)) == false || 
                (await _houseService.IsRented(id)) == false)
            {
                return RedirectToAction(nameof(All));
            }

            if (await _houseService.IsRentedByUserWithId(id, User.Id()) == false)
            {
                return RedirectToPage("/Account/AccessDenied", new { area = "Identity" });
            }

            await _houseService.Leave(id);

            return RedirectToAction(nameof(Mine));
        }
    }
}
