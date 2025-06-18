using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Movie
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(200)]
        public string EnglishTitle { get; set; }

        [Required]
        public string Description { get; set; }

        public int ReleaseYear { get; set; }

        [Required]
        public int CountryId { get; set; }
        public Country Country { get; set; }

        [Required]
        public int GenreId { get; set; }
        public Genre Genre { get; set; }

        [MaxLength(500)]
        public string Director { get; set; }

        [MaxLength(1000)]
        public string Cast { get; set; }

        public string PosterPath { get; set; }
        
        public string TrailerPath { get; set; }

        public int TotalEpisodes { get; set; } = 1;
        public bool IsCompleted { get; set; } = false;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;

        public int Views { get; set; } = 0;
        public double AverageRating { get; set; } = 0.0;
        public int RatingCount { get; set; } = 0;

        public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
        public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
    }
}