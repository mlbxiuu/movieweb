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
using System.Diagnostics;

namespace MovieWebsite.Controllers
{
    public class GenreController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<GenreController> _logger;

        public GenreController(ILogger<GenreController> logger, AppDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // --- HÀNH ĐỘNG INDEX ---
        public async Task<IActionResult> Index()
        {
            // Lấy danh sách thể loại từ DB
            var genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            
            // Tạo ViewModel và gán dữ liệu
            var viewModel = new HomeViewModel1
            {
                Genres = genres
            };

            // Truyền ViewModel ra View
            return View(viewModel);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}