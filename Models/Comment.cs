using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Comment
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int? EpisodeId { get; set; }
        public Episode Episode { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        [Required]
        public string Content { get; set; }

        public int? ParentCommentId { get; set; }
        public Comment ParentComment { get; set; }

        public int Likes { get; set; } = 0;
        public int Dislikes { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<Comment> Replies { get; set; } = new List<Comment>();
    }
}