using System;
using System.Collections.Generic;
using System.Linq;
#nullable disable
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.IO;
using L11.Data;
using L11.Models;
using L11.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace L11.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ArticlesController : Controller
    {
        private readonly ShopDbContext _context;
        private readonly Microsoft.AspNetCore.Hosting.IHostingEnvironment _hostingEnvironment;

        public ArticlesController(Microsoft.AspNetCore.Hosting.IHostingEnvironment HostingEnvironment, ShopDbContext context)
        {
            _context = context;
            _hostingEnvironment = HostingEnvironment;
        }

        // GET: Articles
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var shopDbContext = _context.Articles.Include(a => a.Category);
            return View(await shopDbContext.Take(4).ToListAsync());
        }

        // GET: Articles/Details/5
        [AllowAnonymous]
        public async Task<IActionResult> Details(int? id)
        {
            Console.WriteLine(id);
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

        // GET: Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            return View();
        }

        // POST: Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,ArticleName,Price,FormFile,CategoryId")] ArticleCreateViewModel articleView)
        {
            if (ModelState.IsValid)
            {
                string fileName;
                if (articleView.FormFile != null)
                {
                    string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "upload");
                    string defaultName = articleView.FormFile.FileName;
                    fileName = defaultName.Substring(0, defaultName.Length - 4) + Guid.NewGuid().ToString() + ".jpg";
                    string filePath = Path.Combine(uploads, fileName);
                    var thumbPath = filePath.Substring(0, filePath.Length - 4) + "Thumb.jpg";
                    using (Stream fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                    {
                        articleView.FormFile.CopyTo(fileStream);
                    }
                    using (System.Drawing.Image thumb = System.Drawing.Image.FromFile(filePath))
                    {
                        using (var thumbImage = thumb.GetThumbnailImage(100, 100, new System.Drawing.Image.GetThumbnailImageAbort(() => false), IntPtr.Zero))
                        {
                            thumbImage.Save(thumbPath);
                        }
                    }
                }
                else
                {
                    fileName = "NoImage.jpg";
                }
                Article article = new Article()
                {
                    ArticleId = articleView.ArticleId,
                    ArticleName = articleView.ArticleName,
                    Price = articleView.Price,
                    ImageName = fileName,
                    CategoryId = articleView.CategoryId
                };
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", articleView.CategoryId);
            foreach (var modelState in ViewData.ModelState.Values)
            {
                foreach (var error in modelState.Errors)
                {
                    Console.WriteLine(error);
                }
            }
            var errors2 = ModelState.Values.SelectMany(v => v.Errors);
            Console.WriteLine(errors2);
            return View(articleView);
        }

        // GET: Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", article.CategoryId);
            return View(article);
        }

        // POST: Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,ArticleName,Price,ImageName,CategoryId")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", article.CategoryId);
            return View(article);
        }

        // GET: Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();
            if (article.ImageName != "NoImage.jpg")
            {
                string uploads = Path.Combine(_hostingEnvironment.WebRootPath, "upload");
                string filePath = Path.Combine(uploads, article.ImageName);
                string thumbPath = filePath.Substring(0, filePath.Length - 4) + "Thumb.jpg";
                System.IO.File.Delete(filePath);
                System.IO.File.Delete(thumbPath);
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
