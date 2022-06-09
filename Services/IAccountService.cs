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
    public interface IAccountService
    {
        Task RegisterAsync(RegisterViewModel model);
        Task LoginAsync(LoginViewModel model);
        Task LogoutAsync();
    }

    public class AccountService : IAccountService
    {
        private ApplicationDbContext Context { get; }
        private UserManager<IdentityUser> UserManager { get; }
        private SignInManager<IdentityUser> SignInManager { get; }
        private IWebHostEnvironment AppEnvironment { get; }

        public AccountService(ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IWebHostEnvironment appEnvironment)
        {
            Context = context;
            UserManager = userManager;
            SignInManager = signInManager;
            AppEnvironment = appEnvironment;
        }

        public async Task RegisterAsync(RegisterViewModel model)
        {
            var withSameEmail = await Context.Users.FirstOrDefaultAsync(x => x.NormalizedEmail == model.Email.ToLower());
            if (withSameEmail != null)
            {
                throw new ArgumentException($"User with {model.Email} is already registered.");
            }

            var identityResult = await UserManager.CreateAsync(
                new IdentityUser
                {
                    Id = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    UserName = model.UserName
                },
                model.Password);

            if (identityResult.Succeeded)
            {
                return;
            }

            throw new Exception(identityResult.Errors.First().Description);
        }

        public async Task LoginAsync(LoginViewModel model)
        {
            var signInResult = await SignInManager.PasswordSignInAsync(model.UserName, model.Password, false, false);
            if (signInResult.Succeeded)
            {
                return;
            }

            // обработать ошибки менеджера
            throw new Exception("Incorrect username or password.");
        }

        public async Task LogoutAsync()
        {
            await SignInManager.SignOutAsync();
        }
    }
}