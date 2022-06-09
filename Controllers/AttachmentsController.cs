using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Task3.ViewModels;
using Task3.Store;
using Task3.Store.Models;

namespace Task3.Controllers
{
    public class AttachmentsController : Controller
    {
        private ApplicationDbContext Context;

        public AttachmentsController(ApplicationDbContext context)
        {
            Context = context;
        }
        public IActionResult Index(int? id = null)
        {
            var attachment = Context.Attachments.FirstOrDefault(x => x.Id == id);
            if (attachment == null)
            {
                return NotFound();
            }

            var indexViewModel = new AttachmentViewModel
            {
                Id = attachment.Id,
                MessageId = attachment.MessageId,
                FileName = attachment.FileName,
                FilePath = attachment.FilePath,
                Created = attachment.Created
            };

            return View(indexViewModel);
        }
    }
}
