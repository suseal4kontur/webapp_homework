using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Blog.Models
{
    public sealed class Post
    {
        /// <summary>
        /// Идентификатор поста. Генерируется автоматически при создании с помощью Guid.NewGuid()
        /// </summary>
        [BsonId]
        public string Id { get; set; }

        public string Title { get; set; }

        public string Text { get; set; }

        [BsonIgnoreIfNull]
        public string[] Tags { get; set; }

        [BsonIgnoreIfNull]
        public Comment[] Comments { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
