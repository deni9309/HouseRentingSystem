using HouseRentingSystem.Core.Constants;
using HouseRentingSystem.Core.Contracts;
using HouseRentingSystem.Core.Models.Agent;
using HouseRentingSystem.Extensions;
using HouseRentingSystem.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HouseRentingSystem.Controllers
{
    [Authorize]
    public class AgentController : Controller
    {
        private readonly IAgentService _agentService;
        public AgentController(IAgentService agentService)
        {
            _agentService = agentService;
        }

        [HttpGet]
        public async Task<IActionResult> Become()
        {
            if (await _agentService.ExistsByIdAsync(User.Id()))
            {
                TempData[MessageConstant.ErrorMessage] = "You are already an agent!";

                return RedirectToAction("Index", "Home");
            }

            var model = new BecomeAgentModel();

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Become(BecomeAgentModel model)
        {
            if(!ModelState.IsValid)
            {
                return View(model);
            }

            var userId = User.Id();

            if(await _agentService.ExistsByIdAsync(userId))
            {
                TempData[MessageConstant.ErrorMessage] = "You are already an agent!";

                return RedirectToAction("Index", "Home");
            }

            if (await _agentService.UserWithPhoneNumberExistsAsync(model.PhoneNumber))
            {
                TempData[MessageConstant.ErrorMessage] = "An agent with this phone number already exists!";

                return RedirectToAction("Index", "Home");
            }

            if (await _agentService.UserHasRentsAsync(userId))
            {
                TempData[MessageConstant.ErrorMessage] = "You have active rents!\nTo become an agent leave all houses you currently rent.";

                return RedirectToAction("Index", "Home");
            }

            await _agentService.CreateAsync(userId, model.PhoneNumber);

            TempData[MessageConstant.SuccessMessage] = "Congrats!\nYou have just become an agent!";

            return RedirectToAction("All", "House");
        }
    }
}
