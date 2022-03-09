using System;
using Blog.Exceptions;
using Blog.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Blog.UnitTests
{
    [TestFixture]
    internal sealed class GetPostTests
    {
        private IBlogRepository firstBlogRepository;
        private IBlogRepository secondBlogRepository;

        [SetUp]
        public void SetUp()
        {
            this.firstBlogRepository = new BlogRepository();
            this.secondBlogRepository = new BlogRepository();
        }

        [Test]
        public void GotFromSecondRepository_WhenCreateInFirst()
        {
            var createInfo = new PostCreateInfo
            {
                Title = "Спортивное питание",
                Text = "текст",
                Tags = new[] { "food", "sport" },
            };
            var expected = this.firstBlogRepository.CreatePostAsync(createInfo, default).Result;

            var actual = this.secondBlogRepository.GetPostAsync(expected.Id, default).Result;

            actual.Should().BeEquivalentTo(expected);
        }

        [Test]
        public void ThrowPostNotFoundException_WhenPostNotFound()
        {
            Action action = () => this.firstBlogRepository.GetPostAsync(Guid.NewGuid().ToString(), default);

            action.Should().Throw<PostNotFoundException>();
        }
    }
}
