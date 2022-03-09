using System;

namespace Blog.Exceptions
{
    public sealed class PostNotFoundException : Exception
    {
        public PostNotFoundException(string postId) : base($"Post {postId} not found.")
        {
        }
    }
}
