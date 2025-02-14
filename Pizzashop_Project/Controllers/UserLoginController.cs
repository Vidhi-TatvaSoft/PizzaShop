using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DAL.Models;

namespace Pizzashop_Project.Controllers
{
    public class UserLoginController : Controller
    {
        private readonly PizzashopDbContext _context;

        public UserLoginController(PizzashopDbContext context)
        {
            _context = context;
        }

        // GET: UserLogin
        public async Task<IActionResult> Index()
        {
            var pizzashopDbContext = _context.Userlogins.Include(u => u.Role);
            return View(await pizzashopDbContext.ToListAsync());
        }

        // GET: UserLogin/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null || _context.Userlogins == null)
            {
                return NotFound();
            }

            var userlogin = await _context.Userlogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserloginId == id);
            if (userlogin == null)
            {
                return NotFound();
            }

            return View(userlogin);
        }

        // GET: UserLogin/Create
        public IActionResult Create()
        {
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId");
            return View();
        }

        // POST: UserLogin/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Email,Password")] Userlogin userlogin)
        {
            if(_context.Userlogins.FirstOrDefault(e=> e.Email == userlogin.Email && e.Password==userlogin.Password) != null){
                return RedirectToAction("Index", "UserLogin");
            }
            ViewBag.message ="Enter valid Credentials";
          return View();
        }

        // GET: UserLogin/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null || _context.Userlogins == null)
            {
                return NotFound();
            }

            var userlogin = await _context.Userlogins.FindAsync(id);
            if (userlogin == null)
            {
                return NotFound();
            }
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userlogin.RoleId);
            return View(userlogin);
        }

        // POST: UserLogin/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UserloginId,RoleId,Email,Password")] Userlogin userlogin)
        {
            if (id != userlogin.UserloginId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userlogin);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserloginExists(userlogin.UserloginId))
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
            ViewData["RoleId"] = new SelectList(_context.Roles, "RoleId", "RoleId", userlogin.RoleId);
            return View(userlogin);
        }

        // GET: UserLogin/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null || _context.Userlogins == null)
            {
                return NotFound();
            }

            var userlogin = await _context.Userlogins
                .Include(u => u.Role)
                .FirstOrDefaultAsync(m => m.UserloginId == id);
            if (userlogin == null)
            {
                return NotFound();
            }

            return View(userlogin);
        }

        // POST: UserLogin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            if (_context.Userlogins == null)
            {
                return Problem("Entity set 'PizzashopDbContext.Userlogins'  is null.");
            }
            var userlogin = await _context.Userlogins.FindAsync(id);
            if (userlogin != null)
            {
                _context.Userlogins.Remove(userlogin);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserloginExists(long id)
        {
          return (_context.Userlogins?.Any(e => e.UserloginId == id)).GetValueOrDefault();
        }
    }
}
