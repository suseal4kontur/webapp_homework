using System.Threading;
using System.Threading.Tasks;
using Blog.Models;

namespace Blog
{
    public interface IBlogRepository
    {
        public Task<Post> GetPostAsync(string id, CancellationToken token);

        public Task<PostsList> SearchPostsAsync(PostSearchInfo searchInfo, CancellationToken token);

        public Task<Post> CreatePostAsync(PostCreateInfo createInfo, CancellationToken token);

        public Task UpdatePostAsync(string id, PostUpdateInfo updateInfo, CancellationToken token);

        public Task DeletePostAsync(string id, CancellationToken token);

        public Task CreateCommentAsync(string postId, CommentCreateInfo createInfo, CancellationToken token);
    }
}
