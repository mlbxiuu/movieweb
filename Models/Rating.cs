using System;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class Rating
    {
        public int Id { get; set; }

        [Required]
        public int MovieId { get; set; }
        public Movie Movie { get; set; }

        [Required]
        [MaxLength(100)]
        public string UserName { get; set; }

        public string UserEmail { get; set; }

        [Range(1, 10)]
        public int Score { get; set; }

        [MaxLength(500)]
        public string Review { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}