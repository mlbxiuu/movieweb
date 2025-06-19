
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System;
using System.Threading.Tasks;

namespace MovieWebsite.Controllers
{
    public class WatchPartyController : Controller
    {
        private readonly AppDbContext _context;

        public WatchPartyController(AppDbContext context)
        {
            _context = context;
        }

        // Action được gọi từ nút "Xem Cùng Nhau" trên trang chi tiết phim
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int movieId)
        {
            //Dùng FindAsync để tìm và lấy toàn bộ đối tượng Movie
            var movie = await _context.Movies.FindAsync(movieId);

            // Kiểm tra xem phim có được tìm thấy không. Nếu không, movie sẽ là null.
            if (movie == null)
            {
                // Nếu không tìm thấy, trả về lỗi NotFound
                return NotFound("Không tìm thấy phim.");
            }

            // Nếu code chạy đến đây, có nghĩa là đã tìm thấy phim và biến 'movie' đã có dữ liệu.
            var newRoom = new WatchPartyRoom
            {

                Name = $"Phòng xem chung: {movie.Title}",
                MovieId = movieId,
                InviteCode = Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper()
            };

            _context.WatchPartyRooms.Add(newRoom);
            await _context.SaveChangesAsync();

            return RedirectToAction("Room", new { inviteCode = newRoom.InviteCode });
        }

        // Action hiển thị phòng xem phim
        // URL sẽ có dạng: /watch/ABC123XYZ
        [HttpGet("watch/{inviteCode}")]
        public async Task<IActionResult> Room(string inviteCode)
        {
            var room = await _context.WatchPartyRooms
                                     .Include(r => r.Movie)
                                      .ThenInclude(m => m.Episodes)  // Lấy kèm thông tin phim
                                     .FirstOrDefaultAsync(r => r.InviteCode == inviteCode && r.IsActive);

            if (room == null)
            {
                // Có thể tạo một View riêng để thông báo lỗi này
                TempData["ErrorMessage"] = "Phòng xem phim không tồn tại hoặc đã kết thúc.";
                return RedirectToAction("Index", "Home");
            }

            // === GÁN ID PHIM VÀO VIEWBAG ===
            ViewBag.CurrentMovieId = room.MovieId;
            // Truyền cả đối tượng room (đã có movie bên trong) cho View
            return View(room);
        }
        // Xử lý khi người dùng submit form tham gia phòng
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Join(string inviteCode)
        {
            // Kiểm tra xem inviteCode có được nhập không
            if (string.IsNullOrWhiteSpace(inviteCode))
            {
                TempData["ErrorMessage"] = "Vui lòng nhập mã mời.";
                return RedirectToAction("Index", "Home");
            }

            // Chuyển mã mời người dùng nhập về chữ IN HOA để so sánh
            var normalizedInviteCode = inviteCode.Trim().ToUpper();

            // Tìm phòng có mã mời tương ứng và còn hoạt động
            //    Chúng ta cũng tìm bằng mã đã được chuẩn hóa
            var room = await _context.WatchPartyRooms
                                     .FirstOrDefaultAsync(r => r.InviteCode == normalizedInviteCode && r.IsActive);

            if (room != null)
            {
                //  Nếu phòng tồn tại, chuyển hướng người dùng đến phòng đó
                //    Sử dụng mã mời đã được chuẩn hóa để đảm bảo URL nhất quán
                return RedirectToAction("Room", new { inviteCode = room.InviteCode });
            }
            else
            {
                // Nếu phòng không tồn tại, báo lỗi và quay về trang chủ
                TempData["ErrorMessage"] = "Mã mời không hợp lệ hoặc phòng đã kết thúc.";
                return RedirectToAction("Index", "Home");
            }
        }
    }
}