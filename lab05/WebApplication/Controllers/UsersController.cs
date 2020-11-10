using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebApplication.Models;
using WebApplication.ViewModels;

namespace WebApplication.Controllers
{
    [Authorize(Roles = "admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<User> manager;
        private readonly SignInManager<User> inManager;

        public UsersController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            manager = userManager;
            inManager = signInManager;
        }

        public IActionResult Index()
        {
            IEnumerable<User> users = manager.Users.ToList();

            UsersViewModel model = new UsersViewModel
            {
                Users = users
            };

            return View(model);
        }

        public IActionResult Create()
        {
            UsersViewModel model = new UsersViewModel
            {
                User = new User()
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UsersViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.User))
            {
                model.User.UserName = model.User.Email;

                var result = await manager.CreateAsync(model.User, model.Password);
                if (result.Succeeded)
                {
                    User user = await manager.FindByNameAsync(model.User.Email);

                    if (user != null)
                    {
                        await manager.AddToRoleAsync(user, "user");
                    }

                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(string id)
        {
            User user = await manager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            UsersViewModel model = new UsersViewModel
            {
                User = user
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UsersViewModel model)
        {
            if (ModelState.IsValid & CheckUniqueValues(model.User))
            {
                User user = await manager.FindByIdAsync(model.User.Id);

                if (user != null)
                {
                    user.Email = model.User.Email;
                    user.UserName = model.User.Email;

                    var result = await manager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        foreach (var error in result.Errors)
                        {
                            ModelState.AddModelError(string.Empty, error.Description);
                        }
                    }
                }
            }

            return View(model);
        }

        public async Task<IActionResult> Delete(string id)
        {
            User user = await manager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            UsersViewModel model = new UsersViewModel
            {
                User = user,
                DeleteViewModel = new DeleteViewModel
                {
                    Message = "Do you want to delete this user?",
                    IsForDelete = true
                }
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(UsersViewModel model)
        {
            User user = await manager.FindByIdAsync(model.User.Id);

            if (user == null)
                return NotFound();

            var result = await manager.DeleteAsync(user);
            
            if (result.Succeeded)
            {
                model.DeleteViewModel.IsForDelete = false;

                if (User.Identity.IsAuthenticated & User.Identity.Name == user.UserName)
                {
                    await inManager.SignOutAsync();
                    return RedirectToAction("Index", "Home");
                }
                
                return View(model);
            }
            else
            {
                foreach (var error in result.Errors)
                    ModelState.AddModelError(string.Empty, error.Description);
            }

            model.DeleteViewModel.IsForDelete = true;
            return View(model);
        }

        public async Task<IActionResult> ChangePassword(string id)
        {
            User user = await manager.FindByIdAsync(id);

            if (user == null)
                return NotFound();

            UsersViewModel model = new UsersViewModel
            {
                User = user
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(UsersViewModel model)
        {
            if (ModelState.IsValid)
            {
                User user = await manager.FindByIdAsync(model.User.Id);

                if (user == null)
                    return NotFound();

                var result = await manager.ChangePasswordAsync(user, model.Password, model.NewPassword);

                if (result.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    foreach (var error in result.Errors)
                        ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            return View(model);
        }

        private bool CheckUniqueValues(User user)
        {
            bool firstFlag = true;

            IEnumerable<User> users = manager.Users.ToList();

            User tempUser = users.FirstOrDefault(u => u.Email == user.Email);
            if (tempUser != null)
            {
                if (tempUser.Id != user.Id)
                {
                    ModelState.AddModelError(string.Empty, "Another entity have this email. Please replace this to another.");
                    firstFlag = false;
                }
            }

            if (firstFlag)
                return true;
            else
                return false;
        }
    }
}
