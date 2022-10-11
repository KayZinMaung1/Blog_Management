using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogsController(BlogDbContext context,IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        // GET: api/Blogs
        [HttpGet]
        public async Task<ActionResult<IEnumerable<BlogModel>>> GetBlogs()
        {
            return await _context.Blogs.ToListAsync();
        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        public async Task<ActionResult<BlogModel>> GetBlogModel(int id)
        {
            var blogModel = await _context.Blogs.FindAsync(id);

            if (blogModel == null)
            {
                return NotFound();
            }

            return blogModel;
        }

        // PUT: api/Blogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutBlogModel(int id, BlogModel blogModel)
        {
            if (id != blogModel.Id)
            {
                return BadRequest();
            }

            _context.Entry(blogModel).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BlogModelExists(id))
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

        // POST: api/Blogs
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<BlogModel>> PostBlogModel([FromForm] BlogModel blogModel)
        {
            blogModel.ImageName = await SaveImage(blogModel.ImageFile);
            _context.Blogs.Add(blogModel);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetBlogModel", new { id = blogModel.Id }, blogModel);
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBlogModel(int id)
        {
            var blogModel = await _context.Blogs.FindAsync(id);
            if (blogModel == null)
            {
                return NotFound();
            }

            _context.Blogs.Remove(blogModel);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool BlogModelExists(int id)
        {
            return _context.Blogs.Any(e => e.Id == id);
        }

        [NonAction]
        public async Task<string> SaveImage(IFormFile imageFile)
        {
            string imageName = new string(Path.GetFileNameWithoutExtension(imageFile.FileName).Take(10).ToArray()).Replace(' ', '-');
            imageName = imageName + DateTime.Now.ToString("yymmssfff") + Path.GetExtension(imageFile.FileName);
            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            using (var fileStream = new FileStream(imagePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }
            return imageName;
        }
    }
}
