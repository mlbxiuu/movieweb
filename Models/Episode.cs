using System;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Episode
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        public int EpisodeNumber { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        public string Description { get; set; }

        [Required]
        public string VideoPath { get; set; }

        public int Duration { get; set; }
        public DateTime ReleaseDate { get; set; }
        public int Views { get; set; } = 0;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}