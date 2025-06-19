using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MovieWebsite.Models
{
    public class ChatMessage
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
    }
}