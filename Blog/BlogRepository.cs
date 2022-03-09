using System;
using System.Threading;
using System.Threading.Tasks;
using Blog.Models;

namespace Blog
{
    public sealed class BlogRepository : IBlogRepository
    {
        public Task<Post> GetPostAsync(string id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<PostsList> SearchPostsAsync(PostSearchInfo searchInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task<Post> CreatePostAsync(PostCreateInfo createInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task UpdatePostAsync(string id, PostUpdateInfo updateInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task DeletePostAsync(string id, CancellationToken token)
        {
            throw new NotImplementedException();
        }

        public Task CreateCommentAsync(string postId, CommentCreateInfo createInfo, CancellationToken token)
        {
            throw new NotImplementedException();
        }
    }
}
