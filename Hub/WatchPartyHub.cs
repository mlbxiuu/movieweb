using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace MovieWebsite.Hubs
{
    public class WatchPartyHub : Hub
    {

        private readonly AppDbContext _context;


        public WatchPartyHub(AppDbContext context)
        {

            _context = context;
        }
        // Phương thức này được gọi từ JavaScript của client khi họ vào một phòng
        public async Task JoinRoom(string roomId, string userName)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, roomId);
            await Clients.Group(roomId).SendAsync("UserJoined", $"{userName} đã tham gia phòng.");
        }

        // Được gọi khi client gửi tin nhắn
        public async Task SendMessage(string roomId, string userName, string message)
        {
            // Tìm roomId thực sự từ inviteCode
            var room = await _context.WatchPartyRooms.FirstOrDefaultAsync(r => r.InviteCode == roomId);
            if (room != null)
            {
                var chatMessage = new ChatMessage
                {
                    RoomId = room.Id,
                    UserName = userName,
                    Message = message,
                    Timestamp = DateTime.Now
                };
                _context.ChatMessages.Add(chatMessage);
                await _context.SaveChangesAsync();
            }

            // Gửi tin nhắn đến các client
            await Clients.Group(roomId).SendAsync("ReceiveMessage", userName, message);
        }

        // Được gọi khi người điều khiển (host) thay đổi trạng thái video
        public async Task SyncVideoState(string roomId, string state, double currentTime)
        {
            // Gửi lệnh đến các client khác trong phòng
            await Clients.OthersInGroup(roomId).SendAsync("ReceiveVideoState", state, currentTime);
        }
    }
}

