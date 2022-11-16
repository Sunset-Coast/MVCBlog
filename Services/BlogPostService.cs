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

        public async Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);

                foreach (int tagId in tagIds)
                {
                    Tag? tag = await _context.Tags.FindAsync(tagId);

                    if (blogPost != null && tag != null)
                    {
                        // category.Contacts.Add(contact);

                        blogPost.Tags.Add(tag);
                    }
                }

                await _context.SaveChangesAsync();

            }
            catch (Exception)
            {

                throw;
            }
        }
		public async Task AddTagsToBlogPostAsync(string tagNames, int blogPostId)
		{
			try
			{
				BlogPost? blogPost = await _context.BlogPosts.FindAsync(blogPostId);
				if (blogPost == null) return;
				
				foreach(string tagName in tagNames.Split(","))
				{
					if (string.IsNullOrEmpty(tagName.Trim())) continue;

					Tag? tag = await _context.Tags.FirstOrDefaultAsync(t => t.Name.Trim().ToLower() == tagName.Trim().ToLower());

					if (tag != null)
					{
						blogPost.Tags.Add(tag);
					}
					else
					{
						Tag newTag = new Tag() { Name = tagName.Trim() };
						

						blogPost.Tags.Add(newTag);
					}
				}
				await _context.SaveChangesAsync();
			}
			catch (Exception)
			{

				throw;
			}
		}
        public async Task<List<BlogPost>> GetAllBlogPostsAsync()
		{
			try
			{
                List<BlogPost> blogPosts = await _context.BlogPosts.Include(b => b.Comments).Include(b => b.Category).Include(b=>b.Creator).Include(b=>b.Tags).OrderByDescending(b => b.DateCreated).ToListAsync();

				return blogPosts;
            }
			catch (Exception)
			{

				throw;
			}
		}

	
		public async Task<List<Category>> GetCategoriesAsync()
		{
			try
			{
				//List<Category> categories = await _context.Categories.ToListAsync();

				return await _context.Categories.Include(c => c.BlogPosts).ToListAsync();
					
				
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<BlogPost>> GetPopularBlogPostsAsync()
		{
			try
			{
				List<BlogPost> blogPosts = await _context.BlogPosts
                                                         .Where(b => b.IsDeleted == false && b.IsPublished == true)
														 .Include(b=>b.Comments)
															.ThenInclude(c=>c.Author)
														 .Include(b=>b.Category)
														 .Include(b=>b.Tags)
                                                         .ToListAsync();

				return blogPosts.OrderByDescending(b=>b.Comments.Count).ToList();
			}
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<BlogPost>> GetRecentBlogPostAsync(int count)
		{
			try
			{
                List<BlogPost> blogPosts = await _context.BlogPosts
                                                         .Where(b => b.IsDeleted == false && b.IsPublished == true)
                                                         .Include(b => b.Comments)
                                                            .ThenInclude(c => c.Author)
                                                         .Include(b => b.Category)
                                                         .Include(b => b.Tags)
                                                         .ToListAsync();

				return blogPosts.OrderByDescending(b=>b.DateCreated).Take(count).ToList();
            }
			catch (Exception)
			{

				throw;
			}
		}

		public async Task<List<Tag>> GetTagsAsync()
		{
			try
			{
				List<Tag> Tags = await _context.Tags.Include(t => t.BlogPosts).ToListAsync();

				return Tags;
			}
			catch (Exception)
			{

				throw;
			}

		}

        public async Task RemoveAllBlogPostTagsAysnc(int blogPostId)
        {
            try
            {
                BlogPost? blogPost = await _context.BlogPosts.Include(b => b.Tags).FirstOrDefaultAsync(b => b.Id == blogPostId);

                blogPost!.Tags.Clear();
                _context.Update(blogPost);
                await _context.SaveChangesAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

		public IEnumerable<BlogPost> SearchBlogPosts(string searchString)
		{
			try
			{  
				IEnumerable<BlogPost> blogPosts = new List<BlogPost>();

				if (string.IsNullOrEmpty(searchString))
				{ 
					return blogPosts;
				}
				else
				{
					searchString = searchString.Trim().ToLower();

					blogPosts = _context.BlogPosts.Where(b => b.Title!.ToLower().Contains(searchString) ||
															  b.Abstract!.ToLower().Contains(searchString) ||
															  b.Content!.ToLower().Contains(searchString) ||
															  b.Category!.Name!.ToLower().Contains(searchString) ||
															  b.Comments.Any(c => c.Body!.ToLower().Contains(searchString) ||
																				c.Author!.FirstName!.ToLower().Contains(searchString) ||
                                                                                c.Author!.LastName!.ToLower().Contains(searchString)) ||
                                                              b.Tags!.Any(t => t.Name!.ToLower().Contains(searchString)))
													.Include(b => b.Comments)
															.ThenInclude(c => c.Author)
													.Include(b => b.Category)
													.Include(b => b.Tags)
													.Include(b => b.Creator)
													.Where(b => b.IsDeleted == false && b.IsPublished == true)
													.AsNoTracking()
													.OrderByDescending(b => b.DateCreated)
													.AsEnumerable();
					return blogPosts;
				}


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
