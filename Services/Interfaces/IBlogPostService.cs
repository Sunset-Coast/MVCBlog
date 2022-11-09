using TechnicalBlog.Models;

namespace TechnicalBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task<bool> ValidateSlugAsync(string title, int blogPostId);

        public Task<List<BlogPost>> GetAllBlogPostsAsync();

        public Task<List<BlogPost>> GetPopularBlogPostsAsync();

        public Task<List<BlogPost>> GetRecentBlogPostAsync(int count);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<Tag>> GetTagsAsync();

        public Task<List<Tag>> GetBlogPostTags(int blogPostId);
    }
}
