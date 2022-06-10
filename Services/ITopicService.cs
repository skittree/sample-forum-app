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
    public interface ITopicService
    {
        Task<TopicViewModel> GetViewModelAsync(int id);
        Task<TopicEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal User);
        Task<TopicDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal User);
        Task<TopicCreateViewModel> GetCreateViewModelAsync(int id);
        Task CreateAsync(TopicCreateViewModel model, string username);
        Task EditAsync(TopicEditViewModel model, ClaimsPrincipal User);
        Task DeleteAsync(TopicDeleteViewModel model, ClaimsPrincipal User);
        //api methods
        Task EditTopic(TopicAddEditDto model, int id);
        Task DeleteTopic(int id);
    }

    public class TopicService : ITopicService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        public TopicService(ApplicationDbContext context,
            ISectionService sectionService,
            UserManager<IdentityUser> userManager,
            IMapper mapper,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            AppEnvironment = appEnvironment;
        }

        public async Task<TopicViewModel> GetViewModelAsync(int id)
        {
            var topic = await Context.Topics
                .Include(x => x.Messages)
                .ThenInclude(y => y.Attachments)
                .Include(x => x.Messages)
                .ThenInclude(y => y.Creator)
                .Include(x => x.Section)
                .ThenInclude(y => y.Moderators)
                .ThenInclude(z => z.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            var detailsViewModel = Mapper.Map<TopicViewModel>(topic);
            return detailsViewModel;
        }
        public async Task<TopicCreateViewModel> GetCreateViewModelAsync(int id)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == id);
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }
            var createViewModel = Mapper.Map<TopicCreateViewModel>(section);
            return createViewModel;
        }

        public async Task<TopicEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user)
        {
            var topic = await Context.Topics
                .Include(x => x.Section)
                .ThenInclude(y => y.Moderators)
                .ThenInclude(z => z.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (!user.IsInRole(Roles.Admin) && !topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (topic.Creator == null)
                {
                    throw new ArgumentNullException("This section has no creator");
                }
                if (!(user.Identity.Name == topic.Creator.UserName))
                {
                    throw new ArgumentNullException("User did not create this section");
                }
            }

            var editViewModel = Mapper.Map<TopicEditViewModel>(topic);
            return editViewModel;

        }
        public async Task<TopicDeleteViewModel> GetDeleteViewModelAsync(int id, ClaimsPrincipal user)
        {
            var topic = await Context.Topics
                .Include(x => x.Section)
                .ThenInclude(y => y.Moderators)
                .ThenInclude(z => z.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (!user.IsInRole(Roles.Admin) && !topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (topic.Creator == null)
                {
                    throw new ArgumentNullException("This section has no creator");
                }
                if (!(user.Identity.Name == topic.Creator.UserName))
                {
                    throw new ArgumentNullException("User is not moderator for this section");
                }
            }

            var deleteViewModel = Mapper.Map<TopicDeleteViewModel>(topic);
            return deleteViewModel;
        }

        public async Task CreateAsync(TopicCreateViewModel model, string username)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == model.SectionId);
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            var user = await UserManager.FindByNameAsync(username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var newTopic = Mapper.Map<Topic>(model);
            newTopic.Section = section;
            newTopic.Created = DateTime.Now;
            newTopic.Creator = user;

            await Context.Topics.AddAsync(newTopic);
            await Context.SaveChangesAsync();
        }

        public async Task EditAsync(TopicEditViewModel model, ClaimsPrincipal user)
        {
            var topic = await Context.Topics
                .Include(x => x.Section)
                .ThenInclude(y => y.Moderators)
                .ThenInclude(z => z.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (!user.IsInRole(Roles.Admin) && !topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (topic.Creator == null)
                {
                    throw new ArgumentNullException("This section has no creator");
                }
                if (!(user.Identity.Name == topic.Creator.UserName))
                {
                    throw new ArgumentNullException("User is not moderator for this section");
                }
            }

            topic.Name = model.Name;
            topic.Description = model.Description;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(TopicDeleteViewModel model, ClaimsPrincipal user)
        {
            var topic = await Context.Topics
                .Include(x => x.Section)
                .ThenInclude(y => y.Moderators)
                .ThenInclude(z => z.User)
                .Include(x => x.Creator)
                .FirstOrDefaultAsync(x => x.Id == model.Id);
            if (topic == null)
            {
                throw new ArgumentNullException(nameof(topic));
            }

            if (!user.IsInRole(Roles.Admin) && !topic.Section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
            {
                if (topic.Creator == null)
                {
                    throw new ArgumentNullException("This section has no creator");
                }
                if (!(user.Identity.Name == topic.Creator.UserName))
                {
                    throw new ArgumentNullException("User is not moderator for this section");
                }
            }

            Context.Topics.Remove(topic);
            await Context.SaveChangesAsync();
        }

        //api methods 
        public async Task EditTopic(TopicAddEditDto model, int id)
        {
            var topic = await Context.Topics.FirstOrDefaultAsync(x => x.Id == id);
            if (topic == null)
            {
                throw new KeyNotFoundException("Topic not found.");
            }

            if (model.Name == null)
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            topic.Name = model.Name;
            topic.Description = model.Description;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteTopic(int id)
        {
            var topic = await Context.Topics.FirstOrDefaultAsync(x => x.Id == id);
            if (topic == null)
            {
                throw new KeyNotFoundException("Topic not found.");
            }

            Context.Topics.Remove(topic);
            await Context.SaveChangesAsync();
        }

    }
}