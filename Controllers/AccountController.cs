using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Task3.ViewModels;
using Task3.Services;

namespace Task3.Controllers
{
    public class AccountController : Controller
    {
        private IAccountService AccountService { get; }
        public AccountController(IAccountService accountService)
        {
            AccountService = accountService;
        }

        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await AccountService.RegisterAsync(model);
                return RedirectToAction("Index", "Sections");
            }
            catch (ArgumentException ae)
            {
                ModelState.AddModelError(nameof(model.Email), ae.Message);
                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                await AccountService.LoginAsync(model);
                return RedirectToAction("Index", "Sections");
            }
            catch (Exception e)
            {
                ModelState.AddModelError(string.Empty, e.Message);
                return View(model);
            }
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await AccountService.LogoutAsync();
            return RedirectToAction("Index", "Sections");
        }
    }
}
