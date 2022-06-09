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

namespace Task3.Services
{
    public interface IAdminService
    {
        Task<List<AccountViewModel>> GetIndexViewModelAsync();
        Task<AccountEditViewModel> GetEditViewModelAsync(string username);
        Task EditAsync(AccountEditViewModel model);
        Task AddModerator(string username);
        Task DeleteModerator(string username);


    }

    public class AdminService : IAdminService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private SignInManager<IdentityUser> SignInManager { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        public AdminService(ApplicationDbContext context,
            IMapper mapper,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            Mapper = mapper;
            UserManager = userManager;
            SignInManager = signInManager;
            AppEnvironment = appEnvironment;
        }

        public async Task<List<AccountViewModel>> GetIndexViewModelAsync()
        {
            var users = await Context.Users.ToListAsync();
            var model = Mapper.Map<List<AccountViewModel>>(users);
            
            return model;
        }
        public async Task AddModerator(string username)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            await UserManager.AddToRoleAsync(user, "Moderator");
        }

        public async Task DeleteModerator(string username)
        {
            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == username);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var userSection = await Context.ModeratedSections
                .Include(x => x.User)
                .Where(x => x.User.UserName == username)
                .ToListAsync();

            Context.ModeratedSections.RemoveRange(userSection);

            await UserManager.RemoveFromRoleAsync(user, "Moderator");
        }

        public async Task<AccountEditViewModel> GetEditViewModelAsync(string username)
        {
            var sections = await Context.Sections
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .ToListAsync();

            if (sections == null)
            {
                throw new ArgumentNullException(nameof(sections));
            }

            var sectionView = Mapper.Map<List<SectionViewModel>>(sections);

            var result = new AccountEditViewModel
            {
                UserName = username,
                Sections = sectionView
            };

            return result;
        }

        public async Task EditAsync(AccountEditViewModel model)
        {
            var userSection = await Context.ModeratedSections
                .Include(x => x.User)
                .Where(x => x.User.UserName == model.UserName)
                .ToListAsync();

            Context.ModeratedSections.RemoveRange(userSection);

            if (model.SelectedSections == null)
            {
                await Context.SaveChangesAsync();
                return;
            };

            var sections = await Context.Sections
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .Where(x => model.SelectedSections.Contains(x.Id))
                .ToListAsync();

            if (!sections.Any())
            {
                throw new ArgumentNullException(nameof(sections));
            }

            var user = await UserManager.Users.FirstOrDefaultAsync(x => x.UserName == model.UserName);
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            foreach (var item in sections) 
            {
                Context.ModeratedSections.Add(new ModeratedSections { Section = item, User = user });
            }
            await Context.SaveChangesAsync();
        }
    }
}