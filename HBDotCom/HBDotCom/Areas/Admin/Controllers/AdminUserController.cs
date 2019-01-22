using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using HBDotCom.Areas.Identity.Models;
using HBDotCom.Data;
using HBDotCom.Areas.Admin.Models;
using Microsoft.AspNetCore.Identity;

namespace HBDotCom.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class AdminUserController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole<string>> _roleManager;

        public AdminUserController(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole<string>> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/AdminUser
        public async Task<IActionResult> Index()
        {
            var users = await _context.ApplicationUser.Include(x => x.UserRoles).ToListAsync();
            List<UserViewModel> vm = new List<UserViewModel>();

            foreach (var user in users)
            {
                var userVm = new UserViewModel
                {
                    UserId = user.Id,
                    UserName = user.UserName,
                    FullName = user.FullName,
                    Email = user.Email,
                    EmailConfirmed = user.EmailConfirmed,
                    PhoneNumber = user.PhoneNumber,
                    PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                    Roles = await _userManager.GetRolesAsync(user),
                };

                vm.Add(userVm);
            }

            return View(vm);
        }

        // GET: Admin/AdminUser/Details/5
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel vm = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = await _userManager.GetRolesAsync(user),
            };

            return View(vm);
        }

        // GET: Admin/AdminUser/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/AdminUser/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel userVm)
        {
            if (ModelState.IsValid)
            {
                ApplicationUser user = new ApplicationUser
                {
                    FullName = userVm.FullName,
                    UserName = userVm.UserName,
                    Email = userVm.Email,
                    EmailConfirmed = userVm.EmailConfirmed,
                    PhoneNumber = userVm.PhoneNumber,
                    PhoneNumberConfirmed = userVm.PhoneNumberConfirmed,
                };

                foreach (var role in userVm.Roles)
                {
                    
                }

                IdentityResult result = await _userManager.CreateAsync(user, userVm.Password);

                if (result.Succeeded)
                {
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(userVm);
        }

        // GET: Admin/AdminUser/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            if (user == null)
            {
                return NotFound();
            }

            UserViewModel vm = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed,
                Roles = await _userManager.GetRolesAsync(user),
            };

            return View(vm);
        }

        // POST: Admin/AdminUser/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, UserViewModel user)
        {
            if (id != user.UserId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var userToUpdate = await _userManager.FindByIdAsync(id);
                try
                {
                    userToUpdate.FullName = user.FullName;
                    userToUpdate.UserName = user.UserName;
                    userToUpdate.Email = user.Email;
                    userToUpdate.EmailConfirmed = user.EmailConfirmed;
                    userToUpdate.PhoneNumber = user.PhoneNumber;
                    userToUpdate.PhoneNumberConfirmed = user.PhoneNumberConfirmed;
                    
                    await _userManager.AddToRolesAsync(userToUpdate, user.Roles);
                    
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ApplicationUserExists(id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(user);
        }

        // GET: Admin/AdminUser/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            
            if (user == null)
            {
                return NotFound();
            }

            UserViewModel vm = new UserViewModel
            {
                UserId = user.Id,
                UserName = user.UserName,
                FullName = user.FullName,
                Email = user.Email,
                EmailConfirmed = user.EmailConfirmed,
                PhoneNumber = user.PhoneNumber,
                PhoneNumberConfirmed = user.PhoneNumberConfirmed
            };
            return View(vm);
        }

        // POST: Admin/AdminUser/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            ApplicationUser user = await _userManager.FindByIdAsync(id);
            await _userManager.DeleteAsync(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ApplicationUserExists(string id)
        {
            return _context.ApplicationUser.Any(e => e.Id == id);
        }
    }
}
