using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MovieWebsite.Data;
using MovieWebsite.Models;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace MovieWebsite.Areas.Admin.Controllers
{
    // [Area("Admin")]
    public class AdMoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<AdMoviesController> _logger;

        // Tái sử dụng các phương thức xử lý file upload

        private async Task<string> ProcessFileUpload(IFormFile file, string fileType)
        {
            if (file == null || file.Length == 0) return null;

            string uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }

            string uniqueFileName = $"{fileType}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }

            return $"/uploads/{uniqueFileName}"; // Trả về đường dẫn tương đối
        }

        private void DeleteFile(string filePath)
        {
            if (string.IsNullOrEmpty(filePath)) return;

            // Chuyển đổi đường dẫn tương đối thành đường dẫn vật lý
            var fullPath = Path.Combine(_environment.WebRootPath, filePath.TrimStart('/'));
            if (System.IO.File.Exists(fullPath))
            {
                System.IO.File.Delete(fullPath);
            }
        }


        public AdMoviesController(AppDbContext context, IWebHostEnvironment environment, ILogger<AdMoviesController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Admin/Movies
        public async Task<IActionResult> ListMovies()
        {
            var movies = await _context.Movies
                .Include(m => m.Country)
                //.Include(m => m.Genre) 
                .Include(m => m.Episodes)
                .AsSplitQuery()
                .Include(m => m.MovieGenres).ThenInclude(mg => mg.Genre)
                .OrderByDescending(m => m.CreatedAt)
                .ToListAsync();
            return View(movies);
        }

        // GET: Admin/Movies/Create
        public async Task<IActionResult> Create()
        {
            var viewModel = new MovieViewModel
            {
                Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync(),
                Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync()
            };
            return View(viewModel);
        }

        // POST: Admin/Movies/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)] // 500MB
        public async Task<IActionResult> Create(MovieViewModel movieVM)
        {
            // Xóa lỗi ModelState không cần thiết (kế thừa từ controller cũ của em)
            foreach (var key in ModelState.Keys.Where(k => k.StartsWith("Episodes[") && k.EndsWith("].MovieTitle")))
            {
                ModelState.Remove(key);
            }
            // Thêm validate cho GenreId nếu vẫn còn dùng
            // if (movieVM.GenreId == 0)
            // {
            //     ModelState.AddModelError("GenreId", "Vui lòng chọn thể loại chính.");
            // }

            if (ModelState.IsValid)
            {
                var movie = new Movie
                {
                    Title = movieVM.Title,
                    EnglishTitle = movieVM.EnglishTitle,
                    Description = movieVM.Description,
                    ReleaseYear = movieVM.ReleaseYear,
                    CountryId = movieVM.CountryId,
                    GenreId = movieVM.GenreId, // Bỏ đi nếu không dùng
                    Director = movieVM.Director,
                    Cast = movieVM.Cast,
                    TotalEpisodes = movieVM.TotalEpisodes,
                    IsCompleted = movieVM.IsCompleted,
                    PosterPath = await ProcessFileUpload(movieVM.PosterFile, "poster"),
                    TrailerPath = await ProcessFileUpload(movieVM.TrailerFile, "trailer"),

                };
                if (movieVM.SelectedGenreIds != null)
                {
                    foreach (var genreId in movieVM.SelectedGenreIds)
                    {
                        movie.MovieGenres.Add(new MovieGenre { GenreId = genreId });
                    }
                }
                _context.Add(movie);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã thêm phim mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Nếu thất bại, tải lại Dropdowns và trả về View
            _logger.LogWarning("Tạo phim thất bại do lỗi ModelState.");
            movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
            movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return View(movieVM);
        }

        // GET: Admin/Movies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.MovieGenres)
                 .Include(m => m.Episodes) //lấy ds tập  phim
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            var viewModel = new MovieViewModel
            {
                Id = movie.Id,
                Title = movie.Title,
                EnglishTitle = movie.EnglishTitle,
                Description = movie.Description,
                ReleaseYear = movie.ReleaseYear,
                CountryId = movie.CountryId,
                //GenreId = movie.GenreId, // Bỏ đi nếu không dùng
                Director = movie.Director,
                Cast = movie.Cast,
                TotalEpisodes = movie.TotalEpisodes,
                IsCompleted = movie.IsCompleted,
                PosterPath = movie.PosterPath,
                TrailerPath = movie.TrailerPath,
                SelectedGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList(),
                Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync(),
                Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync(),


                ExistingEpisodes = movie.Episodes.ToList()
            };

            return View(viewModel);
        }

        // POST: Admin/Movies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)]
        public async Task<IActionResult> Edit(int id, MovieViewModel movieVM)
        {
            if (id != movieVM.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    var movieToUpdate = await _context.Movies.Include(m => m.MovieGenres).FirstOrDefaultAsync(m => m.Id == id);
                    if (movieToUpdate == null) return NotFound();

                    // Cập nhật thông tin cơ bản
                    movieToUpdate.Title = movieVM.Title;
                    movieToUpdate.EnglishTitle = movieVM.EnglishTitle;
                    movieToUpdate.Description = movieVM.Description;
                    movieToUpdate.ReleaseYear = movieVM.ReleaseYear;
                    movieToUpdate.CountryId = movieVM.CountryId;
                    //movieToUpdate.GenreId = movieVM.GenreId; // Bỏ đi nếu không dùng
                    movieToUpdate.Director = movieVM.Director;
                    movieToUpdate.Cast = movieVM.Cast;
                    movieToUpdate.TotalEpisodes = movieVM.TotalEpisodes;
                    movieToUpdate.IsCompleted = movieVM.IsCompleted;
                    movieToUpdate.UpdatedAt = DateTime.Now;

                    // Xử lý upload file mới
                    if (movieVM.PosterFile != null)
                    {
                        DeleteFile(movieToUpdate.PosterPath);
                        movieToUpdate.PosterPath = await ProcessFileUpload(movieVM.PosterFile, "poster");
                    }
                    if (movieVM.TrailerFile != null)
                    {
                        DeleteFile(movieToUpdate.TrailerPath);
                        movieToUpdate.TrailerPath = await ProcessFileUpload(movieVM.TrailerFile, "trailer");
                    }

                    // Cập nhật các thể loại (quan hệ nhiều-nhiều)
                    movieToUpdate.MovieGenres.Clear();
                    if (movieVM.SelectedGenreIds != null)
                    {
                        foreach (var genreId in movieVM.SelectedGenreIds)
                        {
                            movieToUpdate.MovieGenres.Add(new MovieGenre { MovieId = id, GenreId = genreId });
                        }
                    }

                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Đã cập nhật phim thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Movies.Any(e => e.Id == movieVM.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu thất bại, tải lại Dropdowns và trả về View
            movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
            movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
            return View(movieVM);
        }

        // GET: Admin/Movies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var movie = await _context.Movies
                .Include(m => m.Country)
                //.Include(m => m.Genre) // Bỏ đi nếu không dùng
                .FirstOrDefaultAsync(m => m.Id == id);

            if (movie == null) return NotFound();

            return View(movie);
        }

        // POST: Admin/Movies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var movie = await _context.Movies.Include(m => m.Episodes).FirstOrDefaultAsync(m => m.Id == id);
            if (movie != null)
            {
                // Xóa file poster và trailer
                DeleteFile(movie.PosterPath);
                DeleteFile(movie.TrailerPath);

                // Xóa file video của các tập phim
                foreach (var episode in movie.Episodes)
                {
                    DeleteFile(episode.VideoPath);
                }

                // EF Core sẽ tự động xóa các bản ghi liên quan trong Episodes, MovieGenres, Comments, Ratings...
                // do đã cấu hình quan hệ khóa ngoại.
                _context.Movies.Remove(movie);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Đã xóa phim và tất cả dữ liệu liên quan thành công!";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/AdMovies/CreateEpisode/5
        public async Task<IActionResult> CreateEpisode(int? movieId)
        {
            if (movieId == null)
            {
                return NotFound();
            }

            // Tìm phim trong DB để lấy thông tin
            var movie = await _context.Movies.FindAsync(movieId.Value);
            if (movie == null)
            {
                return NotFound();
            }

            // Tìm số tập lớn nhất hiện có của phim này để gợi ý số tập tiếp theo
            var lastEpisodeNumber = await _context.Episodes
                                                  .Where(e => e.MovieId == movieId.Value)
                                                  .MaxAsync(e => (int?)e.EpisodeNumber) ?? 0;

            // Chuẩn bị ViewModel để gửi cho View
            var viewModel = new EpisodeViewModel
            {
                MovieId = movie.Id,
                MovieTitle = movie.Title, // Hiển thị tên phim trên form cho dễ nhận biết
                EpisodeNumber = lastEpisodeNumber + 1, // Gợi ý số tập tiếp theo
                ReleaseDate = DateTime.Today // Gợi ý ngày phát hành là hôm nay
            };

            return View(viewModel);
        }
        // Trong AdMoviesController.cs

        // POST: Admin/AdMovies/CreateEpisode
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)] // Giới hạn 500MB cho mỗi tập phim
        public async Task<IActionResult> CreateEpisode(EpisodeViewModel episodeVM)
        {
            // === VALIDATION BỔ SUNG ===
            // 1. Kiểm tra xem file video có thực sự được gửi lên không
            if (episodeVM.VideoFile == null || episodeVM.VideoFile.Length == 0)
            {
                ModelState.AddModelError("VideoFile", "Vui lòng tải lên file video cho tập phim.");
            }

            // 2. Kiểm tra xem số tập có bị trùng với tập đã có của phim này không
            var episodeExists = await _context.Episodes
                                              .AnyAsync(e => e.MovieId == episodeVM.MovieId && e.EpisodeNumber == episodeVM.EpisodeNumber);
            if (episodeExists)
            {
                ModelState.AddModelError("EpisodeNumber", $"Tập số {episodeVM.EpisodeNumber} đã tồn tại cho phim này.");
            }

            // 3. Kiểm tra xem phim có tồn tại không
            var movie = await _context.Movies.FindAsync(episodeVM.MovieId);
            if (movie == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Xử lý upload file video
                string videoPath = await ProcessFileUpload(episodeVM.VideoFile, "episode");

                if (!string.IsNullOrEmpty(videoPath))
                {
                    // Tạo đối tượng Episode (Entity) từ ViewModel
                    var episode = new Episode
                    {
                        MovieId = episodeVM.MovieId,
                        EpisodeNumber = episodeVM.EpisodeNumber,
                        Title = episodeVM.Title,
                        Description = episodeVM.Description,
                        Duration = episodeVM.Duration,
                        ReleaseDate = episodeVM.ReleaseDate,
                        VideoPath = videoPath // Lưu đường dẫn tương đối vào DB
                    };

                    _context.Episodes.Add(episode);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = $"Đã thêm thành công tập {episode.EpisodeNumber} cho phim '{movie.Title}'.";
                    // Chuyển hướng người dùng về trang Edit của phim để họ có thể xem danh sách tập hoặc thêm tập mới
                    return RedirectToAction("Edit", new { id = episodeVM.MovieId });
                }
                else
                {
                    // Nếu upload file thất bại
                    ModelState.AddModelError("", "Có lỗi xảy ra trong quá trình tải file lên. Vui lòng thử lại.");
                }
            }

            // Nếu ModelState không hợp lệ, điền lại MovieTitle và trả về View cũ với các lỗi
            _logger.LogWarning("Thêm tập phim thất bại do lỗi ModelState.");
            // Cần gán lại MovieTitle vì trường này không được submit lên cùng form (nếu không dùng input hidden)
            episodeVM.MovieTitle = movie.Title;
            return View(episodeVM);
        }
    }
}