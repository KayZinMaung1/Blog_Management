using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Blog.Models;
using Microsoft.AspNetCore.Authorization;

namespace Blog.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController : ControllerBase
    {
        private readonly BlogDbContext _context;
        private readonly IWebHostEnvironment _hostEnvironment;

        public BlogsController(BlogDbContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            this._hostEnvironment = hostEnvironment;
        }

        

        // GET: api/Blogs
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<PagedListServer<BlogModel>>> GetBlogsWithPagination([FromQuery] Paginator filter)
        {
            var paginator = new Paginator(filter.PerPage, filter.CurrentPage);
            IEnumerable<BlogModel> result = await _context.Blogs
                .Select(x => new BlogModel() {
                    Id = x.Id,
                    Title = x.Title,
                    Description = x.Description,
                    ImageName = x.ImageName,
                    ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, x.ImageName),
                    CreatedDate = x.CreatedDate,
                    UpdatedDate = x.UpdatedDate
                }).ToListAsync();

            //paging
            var totalCount = result.Count();
            var totalPages = (int)Math.Ceiling((double)totalCount / (double)paginator.PerPage);
            var dataList = result.Skip(paginator.PerPage * (paginator.CurrentPage - 1)).Take(paginator.PerPage); //Page is dynamic
            PagedListServer<BlogModel> blogList = new PagedListServer<BlogModel>();
            blogList.Results = dataList.ToList();
            blogList.TotalCount = totalCount;
            blogList.TotalPages = totalPages;

            return blogList;

        }

        // GET: api/Blogs/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<BlogModel>> GetBlogModel(int id)
        {
            var blogModel = await _context.Blogs.FindAsync(id);

            if (blogModel == null)
            {
                return NotFound();
            }
            blogModel = new BlogModel()
            {
                Id = blogModel.Id,
                Title = blogModel.Title,
                Description = blogModel.Description,
                ImageName = blogModel.ImageName,
                ImageSrc = String.Format("{0}://{1}{2}/Images/{3}", Request.Scheme, Request.Host, Request.PathBase, blogModel.ImageName),
                CreatedDate = blogModel.CreatedDate,
                UpdatedDate = blogModel.UpdatedDate
            };

            return blogModel;


        }

        // PUT: api/Blogs/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> PutBlogModel(int id, [FromForm] BlogModel blogModel)
        {
            if (id != blogModel.Id)
            {
                return BadRequest();
            }

            if (blogModel.ImageFile != null) //Insert new image 
            {
                DeleteImage(blogModel.ImageName);
                blogModel.ImageName = await SaveImage(blogModel.ImageFile);
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
        [Authorize]
        public async Task<ActionResult<BlogModel>> PostBlogModel([FromForm] BlogModel blogModel)
        {
            if (blogModel.ImageFile != null) {
                blogModel.ImageName = await SaveImage(blogModel.ImageFile);
            }


            _context.Blogs.Add(blogModel);
            await _context.SaveChangesAsync();

            return StatusCode(201);
        }

        // DELETE: api/Blogs/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteBlogModel(int id)
        {
            var blogModel = await _context.Blogs.FindAsync(id);
            if (blogModel == null)
            {
                return NotFound();
            }

            if (blogModel.ImageFile != null) 
            {
                DeleteImage(blogModel.ImageName);
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

        [NonAction]
        public void DeleteImage(string imageName)
        {

            var imagePath = Path.Combine(_hostEnvironment.ContentRootPath, "Images", imageName);
            if (System.IO.File.Exists(imagePath))
                System.IO.File.Delete(imagePath);
        }

        

    }
}
