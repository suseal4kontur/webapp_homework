﻿using System;
using System.Threading.Tasks;
using Blog.Exceptions;
using Blog.Models;
using FluentAssertions;
using NUnit.Framework;

namespace Blog.UnitTests
{
    [TestFixture]
    internal sealed class UpdatePostTests
    {
        private IBlogRepository blogRepository;

        [SetUp]
        public void SetUp()
        {
            this.blogRepository = new BlogRepository();
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void OneFieldUpdate_WhenOneFieldNotNull()
        {
            var createInfo = new PostCreateInfo
            {
                Title = "Спортивное питание",
                Text = "текст",
                Tags = new[] { "food", "sport" },
            };
            var post = this.blogRepository.CreatePostAsync(createInfo, default).Result;
            var updateInfo = new PostUpdateInfo { Text = "другой текст" };

            this.blogRepository.UpdatePostAsync(post.Id, updateInfo, default).Wait();

            var updatedPost = this.blogRepository.GetPostAsync(post.Id, default).Result;
            updatedPost.Title.Should().Be(post.Title);
            updatedPost.Text.Should().Be(updateInfo.Text);
            updatedPost.Tags.Should().BeEquivalentTo(post.Tags);
            updatedPost.CreatedAt.Should().BeWithin(TimeSpan.FromMilliseconds(100)).Before(post.CreatedAt);
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void AllFieldsUpdate_WhenAllFieldsNotNull()
        {
            var createInfo = new PostCreateInfo
            {
                Title = "Спортивное питание",
                Text = "текст",
                Tags = new[] { "food", "sport" },
            };
            var post = this.blogRepository.CreatePostAsync(createInfo, default).Result;
            var updateInfo = new PostUpdateInfo
            {
                Title = "другой заголовок",
                Text = "другой текст",
                Tags = new[] { "otherTag" },
            };

            this.blogRepository.UpdatePostAsync(post.Id, updateInfo, default).Wait();

            var updatedPost = this.blogRepository.GetPostAsync(post.Id, default).Result;
            updatedPost.Title.Should().Be(updateInfo.Title);
            updatedPost.Text.Should().Be(updateInfo.Text);
            updatedPost.Tags.Should().BeEquivalentTo(updateInfo.Tags);
            updatedPost.CreatedAt.Should().BeWithin(TimeSpan.FromMilliseconds(100)).Before(post.CreatedAt);
            ((BlogRepository)blogRepository).DeleteAllPostsAsync(default).GetAwaiter().GetResult();
        }

        [Test]
        public void ThrowPostNotFoundException_WhenPostNotFound()
        {
            var updateInfo = new PostUpdateInfo { Title = "другой заголовок" };
            Func<Task> action = async () =>
                await this.blogRepository.UpdatePostAsync(Guid.NewGuid().ToString(), updateInfo, default);

            action.Should().ThrowAsync<PostNotFoundException>();
        }
    }
}
