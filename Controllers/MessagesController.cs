using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
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
    public class MessagesController : Controller
    {
        private IMessageService MessageService { get; }
        private ITopicService TopicService { get; }
        public MessagesController(IMessageService messageService, ITopicService topicService)
        {
            MessageService = messageService;
            TopicService = topicService;
        }
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                var indexViewModel = await TopicService.GetViewModelAsync(id);
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
            var model = await MessageService.GetCreateViewModelAsync(id);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(MessageCreateViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var createViewModel = await MessageService.GetCreateViewModelAsync(model.TopicId);
                return View(createViewModel);
            }
            try
            {
                await MessageService.CreateAsync(model, User);
                return RedirectToAction("Index", new { Id = model.TopicId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
            catch (InvalidDataException e)
            {
                ModelState.AddModelError(nameof(model.Attachments), e.Message);
                var createViewModel = await MessageService.GetCreateViewModelAsync(model.TopicId);
                return View(createViewModel);
            }
        }

        public async Task<IActionResult> Edit(int id)
        {
            try
            {
                var editViewModel = await MessageService.GetEditViewModelAsync(id, User);
                return View(editViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(MessageEditViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await MessageService.EditAsync(model, User);
                return RedirectToAction("Index", new { Id = model.TopicId });
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
                var deleteViewModel = await MessageService.GetDeleteViewModelAsync(id, User);
                return View(deleteViewModel);
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(MessageDeleteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await MessageService.DeleteAsync(model, User);
                return RedirectToAction("Index", new { Id = model.TopicId });
            }
            catch (ArgumentNullException)
            {
                return NotFound();
            }
        }
    }
}
