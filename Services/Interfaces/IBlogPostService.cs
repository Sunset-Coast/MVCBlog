using TechnicalBlog.Models;

namespace TechnicalBlog.Services.Interfaces
{
    public interface IBlogPostService
    {
        public Task AddTagsToBlogPostAsync(IEnumerable<int> tagIds, int blogPostId);
        public Task AddTagsToBlogPostAsync(string tagNames, int blogPostId);

        public Task<List<BlogPost>> GetAllBlogPostsAsync();

        public Task<List<BlogPost>> GetPopularBlogPostsAsync();

        public Task<List<BlogPost>> GetRecentBlogPostAsync(int count);

        public Task<List<Category>> GetCategoriesAsync();

        public Task<List<Tag>> GetTagsAsync();

   
        public Task RemoveAllBlogPostTagsAysnc(int blogPostId);
        public IEnumerable<BlogPost> SearchBlogPosts(string searchString);
        public Task<bool> ValidateSlugAsync(string title, int blogPostId);
    }
}
