using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3.ViewModels;
using Task3.Services;
using Microsoft.AspNetCore.Authorization;

namespace Task3.Controllers
{
    public class SectionsController : Controller
    {
        private ISectionService SectionService { get; }
        public SectionsController(ISectionService sectionService)
        {
            SectionService = sectionService;
        }
        public async Task<IActionResult> Index()
        {
            var model = await SectionService.GetIndexViewModelAsync();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var model = SectionService.GetCreateViewModel();
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SectionCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await SectionService.CreateAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentException ae)
            {
                ModelState.AddModelError(nameof(model.Name), ae.Message);
                return View(model);
            }
        }

        [Authorize(Roles = "Admin, Moderator")]
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var model = await SectionService.GetEditViewModelAsync(id, User);
                return View(model);
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin, Moderator")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(SectionEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var editModel = await SectionService.GetEditViewModelAsync(model.Id, User);
                return View(editModel);
            }
            try
            {
                await SectionService.EditAsync(model, User);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (ArgumentException ae)
            {
                ModelState.AddModelError(nameof(model.Name), ae.Message);
                var editViewModel = await SectionService.GetEditViewModelAsync(model.Id, User);
                return View(editViewModel);
            }
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var model = await SectionService.GetDeleteViewModelAsync(id);
                return View(model);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(SectionDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await SectionService.DeleteAsync(model);
                return RedirectToAction("Index");
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }
    }
}
