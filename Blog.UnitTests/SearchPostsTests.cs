using System;
using System.Threading;
using Blog.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Blog.UnitTests
{
    [TestFixture]
    internal sealed class SearchPostsTests
    {
        private IBlogRepository blogRepository;

        [SetUp]
        public void SetUp()
        {
            this.blogRepository = new BlogRepository();
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void GotPostsCreatedAfterDate_WhenFromCreatedAtNotEmpty()
        {
            var post9 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 9) }, default).Result;
            post9 = this.blogRepository.GetPostAsync(post9.Id, default).Result;
            var post10 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 10) }, default).Result;
            post10 = this.blogRepository.GetPostAsync(post10.Id, default).Result;
            var post20 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 20) }, default).Result;
            post20 = this.blogRepository.GetPostAsync(post20.Id, default).Result;

            var postsList = this.blogRepository
                .SearchPostsAsync(new PostSearchInfo { FromCreatedAt = new DateTime(2022, 3, 10) }, default).Result;

            postsList.Posts.Should().BeEquivalentTo(new[] { post10, post20 });
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void GotPostsCreatedBeforeDate_WhenToCreatedAtNotEmpty()
        {
            var post10 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 10) }, default).Result;
            post10 = this.blogRepository.GetPostAsync(post10.Id, default).Result;
            var post19 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 19) }, default).Result;
            post19 = this.blogRepository.GetPostAsync(post19.Id, default).Result;
            var post20 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { CreatedAt = new DateTime(2022, 3, 20) }, default).Result;
            post20 = this.blogRepository.GetPostAsync(post20.Id, default).Result;

            var postsList = this.blogRepository
                .SearchPostsAsync(new PostSearchInfo { ToCreatedAt = new DateTime(2022, 3, 20) }, default).Result;

            postsList.Posts.Should().BeEquivalentTo(new[] { post10, post19 });
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void GotPostsWithTag_WhenTagNotEmpty()
        {
            var postWithoutTags = this.blogRepository
                .CreatePostAsync(new PostCreateInfo(), default).Result;
            postWithoutTags = this.blogRepository.GetPostAsync(postWithoutTags.Id, default).Result;
            var post1 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag1" } }, default).Result;
            post1 = this.blogRepository.GetPostAsync(post1.Id, default).Result;
            var post2 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag2" } }, default).Result;
            post2 = this.blogRepository.GetPostAsync(post2.Id, default).Result;
            var post12 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag1", "tag2" } }, default).Result;
            post12 = this.blogRepository.GetPostAsync(post12.Id, default).Result;

            var postsList = this.blogRepository
                .SearchPostsAsync(new PostSearchInfo { Tag = "tag1" }, default).Result;

            postsList.Posts.Should().BeEquivalentTo(new[] { post1, post12 });
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void GotCorrectTotal_WhenTagWithLimitAndOffsetNotEmpty()
        {
            var post1 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag1" } }, default).Result;
            post1 = this.blogRepository.GetPostAsync(post1.Id, default).Result;
            var postWithOtherTag = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag2" } }, default).Result;
            postWithOtherTag = this.blogRepository.GetPostAsync(postWithOtherTag.Id, default).Result;
            var post2 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag1" } }, default).Result;
            post2 = this.blogRepository.GetPostAsync(post2.Id, default).Result;
            var post3 = this.blogRepository
                .CreatePostAsync(new PostCreateInfo { Tags = new[] { "tag1" } }, default).Result;
            post3 = this.blogRepository.GetPostAsync(post3.Id, default).Result;

            var postsList = this.blogRepository
                .SearchPostsAsync(new PostSearchInfo { Tag = "tag1", Limit = 1, Offset = 1 }, default).Result;

            postsList.Posts.Should().BeEquivalentTo(new[] { post2 });
            postsList.Total.Should().Be(3);
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void GotTenPosts_WhenLimitIsEmpty()
        {
            for (var i = 0; i < 15; i++)
            {
                this.blogRepository.CreatePostAsync(new PostCreateInfo(), default).Wait();
            }

            var postsList = this.blogRepository.SearchPostsAsync(new PostSearchInfo(), CancellationToken.None).Result;

            postsList.Posts.Should().HaveCount(10);
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }
    }
}
