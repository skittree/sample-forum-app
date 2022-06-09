using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3.ViewModels;
using Task3.Store;
using Task3.Store.Models;
using Task3.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace Task3.Controllers
{
    public class TopicsController : Controller
    {
        private ITopicService TopicService { get; }
        private ISectionService SectionService { get; }
        public TopicsController(ITopicService topicService, ISectionService sectionService)
        {
            TopicService = topicService;
            SectionService = sectionService;
        }
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var indexViewModel = await SectionService.GetViewModelAsync(id);
                return View(indexViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [Authorize]
        public async Task<IActionResult> Create(int id)
        {
            var model = await TopicService.GetCreateViewModelAsync(id);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(TopicCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var createModel = await TopicService.GetCreateViewModelAsync(model.SectionId);
                return View(createModel);
            }
            try
            {
                await TopicService.CreateAsync(model, User.Identity.Name);
                return RedirectToAction("Index", new { Id = model.SectionId });
            }
            catch(ArgumentNullException)
            {
                return NotFound();
            }

        }
        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var editViewModel = await TopicService.GetEditViewModelAsync(id, User);
                return View(editViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(TopicEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var editModel = await TopicService.GetEditViewModelAsync(model.Id, User);
                return View(editModel);
            }
            try
            {
                await TopicService.EditAsync(model, User);
                return RedirectToAction("Index", new { Id = model.SectionId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                var deleteViewModel = await TopicService.GetDeleteViewModelAsync(id, User);
                return View(deleteViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(TopicDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var deleteModel = await TopicService.GetDeleteViewModelAsync(model.Id, User);
                return View(deleteModel);
            }
            try
            {
                await TopicService.DeleteAsync(model, User);
                return RedirectToAction("Index", new { Id = model.SectionId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }
    }
}
