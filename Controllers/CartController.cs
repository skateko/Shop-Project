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
    [Authorize(Policy = "DenyAdmin")]
    public class CartController : Controller
    {
        private readonly ShopDbContext _context;
        public CartController(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            double price = 0;
            List<(Article, Int32)> list = new List<(Article, Int32)>();
            foreach (var key in Request.Cookies.Keys)
            {
                bool success = Int32.TryParse(key, out var cookieValue);
                if (success)
                {
                    Article article = _context.Articles
                        .Include(a => a.Category)
                        .FirstOrDefault(m => m.ArticleId == cookieValue);
                    if (article != null)
                    {
                        int amount = Int32.Parse(Request.Cookies[key]);
                        list.Add((article, amount));
                        price += amount * article.Price;
                    }
                }
            }
            if (price != 0)
            {
                ViewData["BasketType"] = "not-empty-basket";
            }
            else
            {
                ViewData["BasketType"] = "empty-basket";

            }
            ViewData["OverallPrice"] = price;

            return View(list);
        }

        public async Task<IActionResult> AddToCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int ArticleId, int amount)
        {
            SetCookie(ArticleId.ToString(), amount.ToString(), 7);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> DeleteFromCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteFromCart(int ArticleId, int amount)
        {
            SetCookie(ArticleId.ToString(), amount.ToString(), -1);
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EditCart(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        [HttpPost]
        public async Task<IActionResult> EditCart(int ArticleId, int amount)
        {
            SetCookie(ArticleId.ToString(), amount.ToString(), 7);
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        public async Task<IActionResult> Order()
        {
            double price = 0;
            List<(Article, Int32)> list = new List<(Article, Int32)>();
            foreach (var key in Request.Cookies.Keys)
            {
                bool success = Int32.TryParse(key, out var cookieValue);
                if (success)
                {
                    Article article = _context.Articles
                        .Include(a => a.Category)
                        .FirstOrDefault(m => m.ArticleId == cookieValue);
                    if (article != null)
                    {
                        int amount = Int32.Parse(Request.Cookies[key]);
                        list.Add((article, amount));
                        price += amount * article.Price;
                    }
                }
            }
            if (price != 0)
            {
                ViewData["BasketType"] = "not-empty-basket";
            }
            else
            {
                ViewData["BasketType"] = "empty-basket";

            }
            ViewData["OverallPrice"] = price;

            return View(list);
        }


        [HttpPost]
        [Authorize]
        public async Task<IActionResult> OrderConfirm()
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(-1);
            foreach (var cookieKey in Request.Cookies.Keys)
            {
                bool success = Int32.TryParse(cookieKey, out var cookieValue);
                if (success && _context.Articles.Include(a => a.Category)
                                                .FirstOrDefault(m => m.ArticleId == cookieValue) != null)
                {
                    Response.Cookies.Append(cookieKey, Request.Cookies[cookieKey], option);
                }
            }

            TempData["name"] = Request.Form["name"].ToString();
            TempData["surname"] = Request.Form["surname"].ToString();
            TempData["email"] = Request.Form["email"].ToString();
            TempData["phone"] = Request.Form["phone"].ToString();
            TempData["address"] = Request.Form["address"].ToString();
            TempData["date"] = Request.Form["date"].ToString();
            TempData["payment"] = Request.Form["payment"].ToString();

            return RedirectToAction(nameof(OrderSumUp));
        }

        [Authorize]
        public async Task<IActionResult> OrderSumUp()
        {
            return View();
        }

        public void SetCookie(string key, string value, int expireTimeDays)
        {
            CookieOptions option = new CookieOptions();
            option.Expires = DateTime.Now.AddDays(expireTimeDays);
            Response.Cookies.Append(key, value, option);
            foreach (var cookieKey in Request.Cookies.Keys)
            {
                bool success = Int32.TryParse(cookieKey, out var cookieValue);
                if (success && _context.Articles.Include(a => a.Category)
                                                .FirstOrDefault(m => m.ArticleId == cookieValue) != null && cookieKey != key)
                {
                    option.Expires = DateTime.Now.AddDays(7);
                    Response.Cookies.Append(cookieKey, Request.Cookies[cookieKey], option);
                }
            }
        }
    }
}
