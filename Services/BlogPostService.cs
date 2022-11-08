using Microsoft.EntityFrameworkCore;
using TechnicalBlog.Data;
using TechnicalBlog.Extensions;
using TechnicalBlog.Models;
using TechnicalBlog.Services.Interfaces;

namespace TechnicalBlog.Services
{
    public class BlogPostService : IBlogPostService
    {
		private readonly ApplicationDbContext _context;
		//Injects the database
		public BlogPostService(ApplicationDbContext context)
		{
			//Defines it
			_context = context;
		}

		public async Task<List<BlogPost>> GetAllBlogPostsAsync()
		{
			try
			{
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).Include(b => b.Category).Include(b=>b.Tags).OrderByDescending(b => b.DateCreated).ToListAsync();

				return blogPosts;
            }
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<bool> ValidateSlugAsync(string title, int blogPostId)
        {
			try
			{
				//title by default becomes the default string parameter for Slugify
				string newSlug = title.Slugify();

				if (blogPostId == 0)
				{
					return !(await _context.BlogPosts.AnyAsync(b => b.Slug == newSlug));
				}
				else
				{
					BlogPost? blogPost = await _context.BlogPosts.AsNoTracking().FirstOrDefaultAsync(b => b.Id == blogPostId);

					string? oldSlug = blogPost?.Slug;
					// Check to see if the new Title/newSlug is the same as the old Title/oldSlug

					if (!string.Equals(newSlug,oldSlug))
					{
						// Now check to see if the Title/newSlug already exists in the database
						return !(await _context.BlogPosts.AnyAsync(b => b.Id != blogPostId && b.Slug == newSlug));
					}
				}
				return true;

			}
			catch (Exception)
			{

				throw;
			}
        }
    }
}
