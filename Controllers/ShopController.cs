#nullable disable
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using L11.Models;
using L11.ViewModels;
using L11.Data;
using Microsoft.AspNetCore.Authorization;

namespace L11.Controllers
{
    public class ShopController : Controller
    {
        private readonly ShopDbContext _context;
        public ShopController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.ToListAsync());
        }

        public async Task<IActionResult> Shop(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CategoryId == id);
            ViewData["Articles"] = _context.Articles
               .Where(a => a.CategoryId == id)
               .ToList();

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }
    }
}