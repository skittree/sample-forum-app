using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task3.Models;
using Microsoft.AspNetCore.Authorization;
using Task3.Services;
using Task3.ViewModels;

namespace Task3.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private IAdminService AdminService { get; }
        public AdminController(IAdminService adminService)
        {
            AdminService = adminService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await AdminService.GetIndexViewModelAsync();
            return View(model);
        }

        public async Task<IActionResult> Add(string id)
        {
            await AdminService.AddModerator(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Delete(string id)
        {
            await AdminService.DeleteModerator(id);
            return RedirectToAction("Index");
        }

        public async Task<IActionResult> Edit(string id)
        {
            try
            {
                var model = await AdminService.GetEditViewModelAsync(id);
                return View(model);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(AccountEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var editModel = await AdminService.GetEditViewModelAsync(model.UserName);
                return View(editModel);
            }
            try
            {
                await AdminService.EditAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ArgumentException ae)
            {
                ModelState.AddModelError(nameof(model.UserName), ae.Message);
                var editViewModel = await AdminService.GetEditViewModelAsync(model.UserName);
                return View(editViewModel);
            }
        }
    }
}
