using System;
using System.Threading;
using System.Threading.Tasks;
using Blog.Models;
using MongoDB.Driver;
using Blog.Exceptions;
using System.Linq;
using System.Collections.Generic;

namespace Blog
{
    public sealed class BlogRepository : IBlogRepository
    {
        private readonly IMongoCollection<Post> posts;

        public BlogRepository()
        {
            string connectionString = "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase("blog");
            posts = database.GetCollection<Post>("posts");
        }

        public async Task<Post> GetPostAsync(string id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var cursor = await posts.FindAsync(p => p.Id == id, cancellationToken: token);
            var post = cursor.FirstOrDefault(token);
            return post ?? throw new PostNotFoundException(id);
        }

        public async Task<PostsList> SearchPostsAsync(PostSearchInfo searchInfo, CancellationToken token)
        {
            var builder = Builders<Post>.Filter;
            var filter = builder.Empty;
            if (searchInfo.Tag != null)
                filter &= builder.Where(p => p.Tags.Contains(searchInfo.Tag));
            if (searchInfo.FromCreatedAt != null)
                filter &= builder.Gte("CreatedAt", searchInfo.FromCreatedAt.Value);
            if (searchInfo.ToCreatedAt != null)
                filter &= builder.Lt("CreatedAt", searchInfo.ToCreatedAt.Value);

            var limit = searchInfo.Limit ?? 10;
            var offset = searchInfo.Offset ?? 0;

            var result = posts.Find(filter);
            var postsList = new PostsList
            {
                Total = (int)await result.CountDocumentsAsync(token),
                Posts = result.Skip(offset).Limit(limit).ToEnumerable(token).ToArray()
            };
            return postsList;
        }

        public async Task<Post> CreatePostAsync(PostCreateInfo createInfo, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            var post = new Post()
            {
                Id = Guid.NewGuid().ToString(),
                Title = createInfo.Title,
                Text = createInfo.Text,
                Tags = createInfo.Tags,
                Comments = Array.Empty<Comment>(),
                CreatedAt = createInfo.CreatedAt ?? DateTime.UtcNow
            };

            await posts.InsertOneAsync(post, cancellationToken: token);
            return post;
        }

        public async Task UpdatePostAsync(string id, PostUpdateInfo updateInfo, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await GetPostAsync(id, token);

            var updates = new List<UpdateDefinition<Post>>();

            if (updateInfo.Text != null)
                updates.Add(Builders<Post>.Update.Set(p => p.Text, updateInfo.Text));
            if (updateInfo.Title != null)
                updates.Add(Builders<Post>.Update.Set(p => p.Title, updateInfo.Title));
            if (updateInfo.Tags != null)
                updates.Add(Builders<Post>.Update.Set(p => p.Tags, updateInfo.Tags));

            var update = Builders<Post>.Update.Combine(updates);

            await posts.UpdateOneAsync(p => p.Id == id, update, cancellationToken: token);
        }

        public async Task DeletePostAsync(string id, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await GetPostAsync(id, token);

            await posts.DeleteOneAsync(p => p.Id == id, token);
        }

        public async Task DeleteAllPostsAsync(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await posts.DeleteManyAsync(p => true, token);
        }

        public async Task CreateCommentAsync(string postId, CommentCreateInfo createInfo, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            await GetPostAsync(postId, token);

            var comment = new Comment()
            {
                Username = createInfo.Username,
                Text = createInfo.Text,
                CreatedAt = DateTime.UtcNow
            };

            var update = Builders<Post>.Update.AddToSet(p => p.Comments, comment);

            await posts.UpdateOneAsync(p => p.Id == postId, update, cancellationToken: token);
        }
    }
}
