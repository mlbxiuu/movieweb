using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using System.Threading.Tasks;

namespace MovieWebsite.Areas.Admin.Controllers
{
    // [Area("Admin")]
    public class AdminController : Controller
    {
        private readonly AppDbContext _context;

        // Dependency Injection cho AppDbContext
        public AdminController(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Tạo ViewModel để chứa dữ liệu thống kê
            var viewModel = new DashboardViewModel
            {
                MovieCount = await _context.Movies.CountAsync(),
                GenreCount = await _context.Genres.CountAsync(),
                CountryCount = await _context.Countries.CountAsync()
                // Em có thể thêm các thống kê khác ở đây, ví dụ:
                // EpisodeCount = await _context.Episodes.CountAsync(),
                // UserCount = ...
            };

            // Truyền ViewModel ra View
            return View(viewModel);
        }
    }

    // ViewModel dành riêng cho trang Dashboard
    public class DashboardViewModel
    {
        public int MovieCount { get; set; }
        public int GenreCount { get; set; }
        public int CountryCount { get; set; }
        public int EpisodeCount { get; set; }
        public int UserCount { get; set; }
    }
}