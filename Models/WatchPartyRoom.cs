using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class WatchPartyRoom
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string InviteCode { get; set; } // Một chuỗi ngẫu nhiên duy nhất
        public int MovieId { get; set; }
        public Movie Movie { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public bool IsActive { get; set; } = true;
    }
}