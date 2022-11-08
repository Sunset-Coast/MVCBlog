using TechnicalBlog.Models;

namespace TechnicalBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogPostId);

        public Task<List<BlogPost>> GetAllBlogPostsAsync();

  
    }
}
