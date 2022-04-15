#nullable disable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using L11.Data;
using L11.Models;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Authorization;

namespace L11.Controllers
{
    [Route("api/data/")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DataController : ControllerBase
    {
        private readonly ShopDbContext _context;

        public DataController(ShopDbContext context)
        {
            _context = context;
        }

        // GET: api/data/categories
        [HttpGet("categories")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            return await _context.Categories.ToListAsync();
        }

        // GET: api/data/categories/5
        [HttpGet("categories/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return category;
        }

        // PUT: api/data/categories/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("categories/{id}")]
        public async Task<IActionResult> PutCategory(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                return BadRequest();
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/data/categories
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("categories")]
        public async Task<ActionResult<Category>> PostCategory(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetCategory", new { id = category.CategoryId }, category);
        }

        // PATCH: api/data/categories/5
        [HttpPatch("categories/{id}")]
        public async Task<IActionResult> PatchCategory(int id,
                [FromBody] JsonPatchDocument<Category> patch)
        {
            if (patch != null)
            {
                Category category = _context.Categories.Find(id);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (category != null)
                {
                    patch.ApplyTo(category);
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                    return new ObjectResult(category);
                }
                return NotFound();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/data/categories/5
        [HttpDelete("categories/{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            var articles = await _context.Articles.Where(a => a.CategoryId == category.CategoryId).ToListAsync();

            if (articles.Count > 0)
            {
                return BadRequest($"Category {category.CategoryName} has articles - cannot be deleted!");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CategoryId == id);
        }

        // GET: api/data/articles
        [HttpGet("articles")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Article>>> GetArticles()
        {
            return await _context.Articles.ToListAsync();
        }

        // GET: api/data/articles/5
        [HttpGet("articles/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<Article>> GetArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);

            if (article == null)
            {
                return NotFound();
            }

            return article;
        }

        // PUT: api/data/articles/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("articles/{id}")]
        public async Task<IActionResult> PutArticle(int id, Article article)
        {
            if (id != article.ArticleId)
            {
                return BadRequest();
            }

            _context.Entry(article).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ArticleExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/data/articles
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost("articles")]
        public async Task<ActionResult<Article>> PostArticle(Article article)
        {
            _context.Articles.Add(article);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetArticle", new { id = article.ArticleId }, article);
        }

        // PATCH: api/data/articles/5
        [HttpPatch("articles/{id}")]
        public async Task<IActionResult> PatchArticle(int id,
                [FromBody] JsonPatchDocument<Article> patch)
        {
            if (patch != null)
            {
                Article article = _context.Articles.Find(id);

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (article != null)
                {
                    patch.ApplyTo(article);
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                    return new ObjectResult(article);
                }
                return NotFound();
            }
            else
            {
                return BadRequest(ModelState);
            }
        }

        // DELETE: api/data/articles/5
        [HttpDelete("articles/{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }

            _context.Articles.Remove(article);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }

        [HttpGet("articles/next/{skip}/{take}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<Article>>> GetNext(int skip, int take)
        {
            
            var list = _context.Articles.Skip(skip).Take(take).ToList();
            foreach (var elem in list)
            {
               Console.WriteLine(_context.Categories.First(c => c.CategoryId == elem.CategoryId).CategoryName);
            }
            return list;
        }
    }
}
