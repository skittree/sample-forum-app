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
using Task3.DtoModels;
using Task3.ViewModels;
using Microsoft.AspNetCore.Identity;
using System.Security.Claims;
using Task3.Store.Roles;

namespace Task3.Services
{
    public interface ISectionService
    {
        Task<SectionViewModel> GetViewModelAsync(int id);
        Task<SectionEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user);
        Task<SectionDeleteViewModel> GetDeleteViewModelAsync(int id);
        Task<List<SectionViewModel>> GetIndexViewModelAsync();
        SectionCreateViewModel GetCreateViewModel();
        Task CreateAsync(SectionCreateViewModel model);
        Task EditAsync(SectionEditViewModel model, ClaimsPrincipal user);
        Task DeleteAsync(SectionDeleteViewModel model);
        //api methods
        Task<List<SectionDto>> GetAllSections();
        Task AddSection(SectionAddEditDto model);
        Task EditSection(SectionAddEditDto model, int id);
        Task DeleteSection(int id);
    }

    public class SectionService : ISectionService
    {
        private ApplicationDbContext Context { get; }
        private IMapper Mapper { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        public SectionService(ApplicationDbContext context,
            IMapper mapper,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            Mapper = mapper;
            AppEnvironment = appEnvironment;
        }

        public async Task<List<SectionViewModel>> GetIndexViewModelAsync()
        {
            var sections = await Context.Sections
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .ToListAsync();

            var model = Mapper.Map<List<SectionViewModel>>(sections);
            return model;
        }

        public async Task<SectionViewModel> GetViewModelAsync(int id)
        {
            var section = await Context.Sections
                .Include(x => x.Topics)
                .ThenInclude(y => y.Creator)
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return Mapper.Map<SectionViewModel>(section);
        }
        public SectionCreateViewModel GetCreateViewModel()
        {
            return new SectionCreateViewModel { };
        }

        public async Task<SectionEditViewModel> GetEditViewModelAsync(int id, ClaimsPrincipal user)
        {
            var section = await Context.Sections
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
                {
                    throw new ArgumentNullException("User is not moderator for this section");
                }
            }

            return Mapper.Map<SectionEditViewModel>(section);
        }
        public async Task<SectionDeleteViewModel> GetDeleteViewModelAsync(int id)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == id);
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            return Mapper.Map<SectionDeleteViewModel>(section);
        }

        public async Task CreateAsync(SectionCreateViewModel model)
        {
            if (Context.Sections.Any(x => x.Name.ToLower() == model.Name.ToLower()))
            {
                throw new ArgumentException($"Section with name {model.Name} already exists.");
            }

            var newSection = Mapper.Map<Section>(model);

            Context.Sections.Add(newSection);
            await Context.SaveChangesAsync();
        }

        public async Task EditAsync(SectionEditViewModel model, ClaimsPrincipal user)
        {
            var section = await Context.Sections
                .Include(x => x.Moderators)
                .ThenInclude(y => y.User)
                .FirstOrDefaultAsync(x => x.Id == model.Id);

            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            if (!user.IsInRole(Roles.Admin))
            {
                if (!section.Moderators.Any(x => x.User.UserName == user.Identity.Name))
                {
                    throw new ArgumentNullException("User is not moderator for this section");
                }
            }

            var withSameName = await Context.Sections.FirstOrDefaultAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (withSameName != null && section.Id != withSameName.Id)
            {
                throw new ArgumentException($"Section with name {model.Name} already exists.");
            }

            section.Name = model.Name;
            section.Description = model.Description;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteAsync(SectionDeleteViewModel model)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == model.Id);
            if (section == null)
            {
                throw new ArgumentNullException(nameof(section));
            }

            Context.Sections.Remove(section);
            await Context.SaveChangesAsync();
        }

        //api methods

        public async Task<List<SectionDto>> GetAllSections()
        {
            var sections = await Context.Sections.ToListAsync();
            var dtomodel = Mapper.Map<List<SectionDto>>(sections);
            return dtomodel;
        }

        public async Task AddSection(SectionAddEditDto model)
        {
            if (model.Name == null)
            {
                throw new ArgumentNullException(nameof(model.Name));
            }
            if (await Context.Sections.AnyAsync(x => x.Name.ToLower() == model.Name.ToLower()))
            {
                throw new ArgumentException($"Section with name {model.Name} already exists.");
            }

            var newSection = Mapper.Map<Section>(model);

            Context.Sections.Add(newSection);
            await Context.SaveChangesAsync();
        }

        public async Task EditSection(SectionAddEditDto model, int id)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == id);
            if (section == null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            if (model.Name == null)
            {
                throw new ArgumentNullException(nameof(model.Name));
            }

            var withSameName = await Context.Sections.FirstOrDefaultAsync(x => x.Name.ToLower() == model.Name.ToLower());
            if (withSameName != null && section.Id != withSameName.Id)
            {
                throw new ArgumentException($"Section with name {model.Name} already exists.");
            }

            section.Name = model.Name;
            section.Description = model.Description;

            await Context.SaveChangesAsync();
        }

        public async Task DeleteSection(int id)
        {
            var section = await Context.Sections.FirstOrDefaultAsync(x => x.Id == id);
            if (section == null)
            {
                throw new KeyNotFoundException("Section not found.");
            }

            Context.Sections.Remove(section);
            await Context.SaveChangesAsync();
        }
    }
}