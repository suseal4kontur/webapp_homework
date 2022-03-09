using System;
using Blog.Exceptions;
using Blog.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Blog.UnitTests
{
    [TestFixture]
    internal sealed class DeletePostTests
    {
        private IBlogRepository blogRepository;

        [SetUp]
        public void SetUp()
        {
            this.blogRepository = new BlogRepository();
        }

        [Test]
        public void ThrowPostNotFoundException_WhenGetDeletedPost()
        {
            var post = this.blogRepository.CreatePostAsync(new PostCreateInfo(), default).Result;

            this.blogRepository.DeletePostAsync(post.Id, default).Wait();

            Action action = () => this.blogRepository.GetPostAsync(post.Id, default);
            action.Should().Throw<PostNotFoundException>();
        }

        [Test]
        public void ThrowPostNotFoundException_WhenPostNotFound()
        {
            Action action = () => this.blogRepository.DeletePostAsync(Guid.NewGuid().ToString(), default);

            action.Should().Throw<PostNotFoundException>();
        }
    }
}
