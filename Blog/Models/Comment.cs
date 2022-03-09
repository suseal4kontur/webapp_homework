using System;

namespace Blog.Models
{
    public sealed class Comment
    {
        public string Username { get; set; }

        public string Text { get; set; }

        /// <summary>
        /// Дата создания комментария. Проставляется автоматически при создании.
        /// </summary>
        public DateTime CreatedAt { get; set; }
    }
}
