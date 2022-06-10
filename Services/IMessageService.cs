using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Task3.Store;
using Task3.Store.Models;
using Task3.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Task3.Store.Roles;
using Task3.DtoModels;

namespace Task3.Services
{
    public interface IMessageService
    {
/*        MessageViewModel GetViewModel(int id);*/
        Task<MessageEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal User);
        Task<MessageDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal User);
        Task<MessageCreateViewModel> GetCreateViewModelAsync(int id);
        Task CreateAsync(MessageCreateViewModel model, ClaimsPrincipal User);
        Task EditAsync(MessageEditViewModel model, ClaimsPrincipal User);
        Task DeleteAsync(MessageDeleteViewModel model, ClaimsPrincipal User);

        // api methods
        Task EditMessage(MessageAddEditDto model, int id);
        Task DeleteMessage(int id);
    }

    public class MessageService : IMessageService
    {
        private ApplicationDbContext Context { get; }
        private ITopicService TopicService { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        private string[] AllowedExtensions { get; } = { "jpg", "jpeg", "png" };

        public MessageService(ApplicationDbContext context,
            ITopicService topicService,
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            AppEnvironment = appEnvironment;
            TopicService = topicService;
        }

/*        public MessageViewModel GetViewModel(int id)
        {
            return;
        }*/
        public async Task<MessageCreateViewModel> GetCreateViewModelAsync(int id)
        {
            var topic = await Context.Topics.FirstOrDefaultAsync(x => x.Id == id);
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }
            var createViewModel = Mapper.Map<MessageCreateViewModel>(topic);
            return createViewModel;
        }

        public async Task<MessageEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user)
        {
            var message = await Context.Messages
                .Include(x => x.Topic)
                .ThenInclude(y => y.Section)
                .ThenInclude(z => z.Moderators)
                .ThenInclude(a => a.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!user.IsInRole(Roles.Admin) && !message.Topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (message.Creator == null)
                {
                    throw new ArgumentNullException("This message has no creator");
                }
                if (!(user.Identity.Name == message.Creator.UserName))
                {
                    throw new ArgumentNullException("User did not create this message");
                }
            }

            var editViewModel = Mapper.Map<MessageEditViewModel>(message);
            return editViewModel;
        }
        public async Task<MessageDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user)
        {
            var message = await Context.Messages
                .Include(x => x.Topic)
                .ThenInclude(y => y.Section)
                .ThenInclude(z => z.Moderators)
                .ThenInclude(a => a.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!user.IsInRole(Roles.Admin) && !message.Topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (message.Creator == null)
                {
                    throw new ArgumentNullException("This message has no creator");
                }
                if (!(user.Identity.Name == message.Creator.UserName))
                {
                    throw new ArgumentNullException("User did not create this message");
                }
            }

            var deleteViewModel = Mapper.Map<MessageDeleteViewModel>(message);
            return deleteViewModel;
        }

        public async Task CreateAsync(MessageCreateViewModel model, ClaimsPrincipal User)
        {
            var topic = await Context.Topics.FirstOrDefaultAsync(x => x.Id == model.TopicId);

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            var user = await UserManager.FindByNameAsync(User.Identity.Name);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            List<Attachment> attachments = new List<Attachment> { };
            if (!(model.Attachments is null))
            {
                foreach (var file in model.Attachments)
                {
                    var extension = Path.GetExtension(file.FileName)?.Replace(".", "");
                    if (!AllowedExtensions.Contains(extension))
                    {
                        throw new InvalidDataException("Invalid file extension(s). Please only upload " + string.Join(", ", AllowedExtensions.ToArray()) + " files");
                    }

                    var fileFolder = "Files";
                    var wwwroot = "wwwroot";
                    var wwwrootFolder = $"{wwwroot}/{fileFolder}";
                    var fileId = Guid.NewGuid();
                    var path = $"{fileFolder}/{fileId}.{extension}";
                    var fullPath = $"{wwwroot}/{path}";

                    bool exists = System.IO.Directory.Exists(wwwrootFolder);

                    if (!exists)
                        System.IO.Directory.CreateDirectory(wwwrootFolder);

                    using (var fileStream = new FileStream(fullPath,
                        FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    var attachment = new Attachment
                    {
                        FileName = fileId.ToString(),
                        FilePath = path,
                        Created = DateTime.Now
                    };
                    attachments.Add(attachment);
                }
                Context.Attachments.AddRange(attachments);
            };

            var newMessage = Mapper.Map<Message>(model);
            newMessage.Topic = topic;
            newMessage.Created = DateTime.Now;
            newMessage.Creator = user;
            newMessage.Attachments = attachments;

            await Context.Messages.AddAsync(newMessage);
            await Context.SaveChangesAsync();
        }

        public async Task EditAsync(MessageEditViewModel model, ClaimsPrincipal user)
        {
            var message = await Context.Messages
                .Include(x => x.Topic)
                .ThenInclude(y => y.Section)
                .ThenInclude(z => z.Moderators)
                .ThenInclude(a => a.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!user.IsInRole(Roles.Admin) && !message.Topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (message.Creator == null)
                {
                    throw new ArgumentNullException("This message has no creator");
                }
                if (!(user.Identity.Name == message.Creator.UserName))
                {
                    throw new ArgumentNullException("User did not create this message");
                }
            }

            message.Text = model.Text;
            message.Modified = DateTime.Now;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(MessageDeleteViewModel model, ClaimsPrincipal user)
        {
            var message = await Context.Messages
                .Include(x => x.Topic)
                .ThenInclude(y => y.Section)
                .ThenInclude(z => z.Moderators)
                .ThenInclude(a => a.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (message == null)
            {
                throw new ArgumentNullException(nameof(message));
            }

            if (!user.IsInRole(Roles.Admin) && !message.Topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (message.Creator == null)
                {
                    throw new ArgumentNullException("This message has no creator");
                }
                if (!(user.Identity.Name == message.Creator.UserName))
                {
                    throw new ArgumentNullException("User did not create this message");
                }
            }

            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
        }

        // api methods
        public async Task EditMessage(MessageAddEditDto model, int id)
        {
            var message = await Context.Messages.FirstOrDefaultAsync(x => x.Id == id);
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            if (model.Text == null)
            {
                throw new ArgumentNullException(nameof(model.Text));
            }

            message.Text = model.Text;
            message.Modified = DateTime.Now;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteMessage(int id)
        {
            var message = await Context.Messages.FirstOrDefaultAsync(x => x.Id == id);
            if (message == null)
            {
                throw new KeyNotFoundException("Message not found.");
            }

            Context.Messages.Remove(message);
            await Context.SaveChangesAsync();
        }
    }
}