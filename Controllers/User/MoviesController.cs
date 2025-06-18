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

namespace MovieWebsite.Controllers
{
    public class MoviesController : Controller
    {
        private readonly AppDbContext _context;
        private readonly IWebHostEnvironment _environment;
        private readonly ILogger<MoviesController> _logger;

        public MoviesController(
            AppDbContext context,
            IWebHostEnvironment environment,
            ILogger<MoviesController> logger)
        {
            _context = context;
            _environment = environment;
            _logger = logger;
        }

        // GET: Movie/Index
        public async Task<IActionResult> Index(MovieSearchViewModel searchVM)
        {
            try
            {
                var moviesQuery = _context.Movies
                    .Include(m => m.Country)
                    .Include(m => m.Genre)
                    .Include(m => m.MovieGenres)
                    .ThenInclude(mg => mg.Genre)
                    .AsQueryable();

                // Apply search filters
                if (!string.IsNullOrEmpty(searchVM.SearchTerm))
                {
                    moviesQuery = moviesQuery.Where(m => m.Title.Contains(searchVM.SearchTerm) ||
                                                       m.EnglishTitle.Contains(searchVM.SearchTerm));
                }

                if (searchVM.CountryId.HasValue)
                {
                    moviesQuery = moviesQuery.Where(m => m.CountryId == searchVM.CountryId);
                }

                if (searchVM.GenreId.HasValue)
                {
                    moviesQuery = moviesQuery.Where(m => m.MovieGenres.Any(mg => mg.GenreId == searchVM.GenreId));
                }

                if (searchVM.Year.HasValue)
                {
                    moviesQuery = moviesQuery.Where(m => m.ReleaseYear == searchVM.Year);
                }

                // Apply sorting
                switch (searchVM.SortBy?.ToLower())
                {
                    case "oldest":
                        moviesQuery = moviesQuery.OrderBy(m => m.ReleaseYear);
                        break;
                    case "rating":
                        moviesQuery = moviesQuery.OrderByDescending(m => m.AverageRating);
                        break;
                    case "views":
                        moviesQuery = moviesQuery.OrderByDescending(m => m.Views);
                        break;
                    default: // newest
                        moviesQuery = moviesQuery.OrderByDescending(m => m.CreatedAt);
                        break;
                }

                // Pagination
                searchVM.TotalCount = await moviesQuery.CountAsync();
                searchVM.TotalPages = (int)Math.Ceiling((double)searchVM.TotalCount / searchVM.PageSize);
                searchVM.Page = Math.Max(1, Math.Min(searchVM.Page, searchVM.TotalPages));

                searchVM.Movies = await moviesQuery
                    .Skip((searchVM.Page - 1) * searchVM.PageSize)
                    .Take(searchVM.PageSize)
                    .ToListAsync();

                searchVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
                searchVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();

                return View(searchVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movie index");
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải danh sách phim";
                return RedirectToAction("Index", "Home");
            }
        }

        // GET: Movie/Create
        public IActionResult Create()
        {
            var viewModel = new MovieViewModel
            {
                Countries = _context.Countries.OrderBy(c => c.Name).ToList(),
                Genres = _context.Genres.OrderBy(g => g.Name).ToList(),
                Episodes = new List<EpisodeViewModel>()
            };
            return View(viewModel);
        }

        // POST: Movie/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [RequestSizeLimit(524_288_000)]
        public async Task<IActionResult> Create(MovieViewModel movieVM)
        {
            try
            {
                _logger.LogInformation("Nhận được MovieViewModel: Title={Title}, Episodes={EpisodeCount}, SelectedGenres={GenreCount}, PosterFile={PosterFile}, TrailerFile={TrailerFile}",
                    movieVM.Title, movieVM.Episodes?.Count ?? 0, movieVM.SelectedGenreIds?.Count ?? 0,
                    movieVM.PosterFile?.FileName ?? "null", movieVM.TrailerFile?.FileName ?? "null");

                // Xóa lỗi MovieTitle khỏi ModelState
                foreach (var key in ModelState.Keys.Where(k => k.Contains("MovieTitle")))
                {
                    ModelState.Remove(key);
                }

                if (!ModelState.IsValid)
                {
                    var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage);
                    _logger.LogWarning("Xác thực ModelState thất bại: {Errors}", string.Join(", ", errors));
                    movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
                    movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                    return View(movieVM);
                }

                if (!ValidateUploadedFiles(movieVM, out string errorMessage))
                {
                    _logger.LogWarning("Xác thực file thất bại: {ErrorMessage}", errorMessage);
                    ModelState.AddModelError("", errorMessage);
                    movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
                    movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                    return View(movieVM);
                }

                string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");
                Directory.CreateDirectory(uploadsFolder);

                var movie = new Movie
                {
                    Title = movieVM.Title,
                    EnglishTitle = movieVM.EnglishTitle,
                    Description = movieVM.Description,
                    ReleaseYear = movieVM.ReleaseYear,
                    CountryId = movieVM.CountryId,
                    GenreId = movieVM.GenreId,
                    Director = movieVM.Director,
                    Cast = movieVM.Cast,
                    TotalEpisodes = movieVM.TotalEpisodes,
                    IsCompleted = movieVM.IsCompleted,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Episodes = new List<Episode>()
                };

                try
                {
                    movie.PosterPath = await ProcessFileUpload(movieVM.PosterFile, uploadsFolder, "poster");
                    movie.TrailerPath = await ProcessFileUpload(movieVM.TrailerFile, uploadsFolder, "trailer");

                    if (movieVM.Episodes != null && movieVM.Episodes.Any())
                    {
                        foreach (var episodeVM in movieVM.Episodes)
                        {
                            _logger.LogInformation("Xử lý tập phim: EpisodeNumber={EpisodeNumber}, VideoFile={VideoFile}",
                                episodeVM.EpisodeNumber, episodeVM.VideoFile?.FileName ?? "null");
                            var videoPath = await ProcessFileUpload(episodeVM.VideoFile, uploadsFolder, $"episode_{episodeVM.EpisodeNumber}");
                            movie.Episodes.Add(new Episode
                            {
                                EpisodeNumber = episodeVM.EpisodeNumber,
                                Title = episodeVM.Title,
                                Description = episodeVM.Description,
                                Duration = episodeVM.Duration,
                                ReleaseDate = episodeVM.ReleaseDate,
                                VideoPath = videoPath,
                                CreatedAt = DateTime.Now,
                                UpdatedAt = DateTime.Now
                            });
                        }
                    }

                    if (movieVM.SelectedGenreIds != null)
                    {
                        movie.MovieGenres = movieVM.SelectedGenreIds
                            .Select(id => new MovieGenre { GenreId = id })
                            .ToList();
                    }

                    _context.Add(movie);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Đã thêm phim: {movie.Title} (ID: {movie.Id})");
                    TempData["SuccessMessage"] = $"Đã thêm phim '{movie.Title}' thành công!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi xử lý tải file hoặc lưu vào cơ sở dữ liệu");
                    CleanupUploadedFiles(movie);
                    TempData["ErrorMessage"] = $"Lỗi khi lưu phim: {ex.Message}";
                    movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
                    movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                    return View(movieVM);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thêm phim mới: {Message}, InnerException: {InnerException}", ex.Message, ex.InnerException?.Message);
                TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi thêm phim mới: {ex.Message}";
                movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
                movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
                return View(movieVM);
            }
        }

        private bool ValidateUploadedFiles(MovieViewModel model, out string errorMessage)
        {
            errorMessage = string.Empty;

            // Validate Poster
            if (model.PosterFile == null || model.PosterFile.Length == 0)
            {
                if (model.Id == 0) // New movie requires poster
                {
                    errorMessage = "Vui lòng chọn file poster";
                    return false;
                }
            }
            else
            {
                var allowedImageTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif" };
                if (!allowedImageTypes.Contains(model.PosterFile.ContentType.ToLower()))
                {
                    errorMessage = "Chỉ chấp nhận file ảnh (JPEG, JPG, PNG, GIF) cho poster";
                    return false;
                }

                if (model.PosterFile.Length > 5 * 1024 * 1024) // 5MB
                {
                    errorMessage = "Poster không được vượt quá 5MB";
                    return false;
                }
            }

            // Validate Trailer (optional)
            if (model.TrailerFile != null && model.TrailerFile.Length > 0)
            {
                var allowedVideoTypes = new[] { "video/mp4", "video/quicktime", "video/x-msvideo" };
                if (!allowedVideoTypes.Contains(model.TrailerFile.ContentType.ToLower()))
                {
                    errorMessage = "Chỉ chấp nhận file video (MP4, MOV, AVI) cho trailer";
                    return false;
                }

                if (model.TrailerFile.Length > 100 * 1024 * 1024) // 100MB
                {
                    errorMessage = "Trailer không được vượt quá 100MB";
                    return false;
                }
            }

            // Validate Episode Videos (only if episodes are provided)
            if (model.Episodes != null && model.Episodes.Any())
            {
                var allowedVideoTypes = new[] { "video/mp4", "video/quicktime", "video/x-msvideo" };
                foreach (var episode in model.Episodes)
                {
                    if (episode.VideoFile == null || episode.VideoFile.Length == 0)
                    {
                        errorMessage = $"Vui lòng chọn file video cho tập {episode.EpisodeNumber}";
                        return false;
                    }

                    if (!allowedVideoTypes.Contains(episode.VideoFile.ContentType.ToLower()))
                    {
                        errorMessage = $"Chỉ chấp nhận file video (MP4, MOV, AVI) cho tập {episode.EpisodeNumber}";
                        return false;
                    }

                    if (episode.VideoFile.Length > 100 * 1024 * 1024) // 100MB
                    {
                        errorMessage = $"Video tập {episode.EpisodeNumber} không được vượt quá 100MB";
                        return false;
                    }

                    if (string.IsNullOrEmpty(episode.Title))
                    {
                        errorMessage = $"Vui lòng nhập tên cho tập {episode.EpisodeNumber}";
                        return false;
                    }

                    if (episode.Duration < 1 || episode.Duration > 600)
                    {
                        errorMessage = $"Thời lượng tập {episode.EpisodeNumber} phải từ 1 đến 600 phút";
                        return false;
                    }

                    if (episode.ReleaseDate == default)
                    {
                        errorMessage = $"Vui lòng chọn ngày phát hành cho tập {episode.EpisodeNumber}";
                        return false;
                    }
                }

                // Check for duplicate episode numbers
                var episodeNumbers = model.Episodes.Select(e => e.EpisodeNumber).ToList();
                if (episodeNumbers.Distinct().Count() != episodeNumbers.Count)
                {
                    errorMessage = "Số tập phim không được trùng lặp";
                    return false;
                }

                if (model.Episodes.Count > model.TotalEpisodes)
                {
                    errorMessage = "Số lượng tập phim vượt quá tổng số tập đã khai báo";
                    return false;
                }
            }

            return true;
        }

        private async Task<string> ProcessFileUpload(IFormFile file, string uploadsFolder, string fileType)
        {
            if (file == null || file.Length == 0)
            {
                _logger.LogWarning("Không có file được tải lên cho {FileType}", fileType);
                return null;
            }

            string fileExt = Path.GetExtension(file.FileName).ToLower();
            string uniqueFileName = $"{fileType}_{Guid.NewGuid()}{fileExt}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            _logger.LogInformation($"Đang tải file {fileType}: {file.FileName} lên {filePath}");

            try
            {
                // Kiểm tra quyền ghi trước khi lưu
                var directory = Path.GetDirectoryName(filePath);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                using (var fileStream = new FileStream(filePath, FileMode.Create, FileAccess.Write))
                {
                    await file.CopyToAsync(fileStream);
                }

                // Xác minh file đã được lưu
                if (!System.IO.File.Exists(filePath))
                {
                    throw new IOException($"File không được lưu tại {filePath}");
                }

                return $"/Uploads/{uniqueFileName}";
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tải file {FileName} lên {FilePath}: {Message}", file.FileName, filePath, ex.Message);
                throw;
            }
        }

        private void CleanupUploadedFiles(Movie movie)
        {
            try
            {
                if (!string.IsNullOrEmpty(movie.PosterPath))
                {
                    var posterPath = Path.Combine(_environment.WebRootPath, movie.PosterPath.TrimStart('/'));
                    if (System.IO.File.Exists(posterPath))
                        System.IO.File.Delete(posterPath);
                }

                if (!string.IsNullOrEmpty(movie.TrailerPath))
                {
                    var trailerPath = Path.Combine(_environment.WebRootPath, movie.TrailerPath.TrimStart('/'));
                    if (System.IO.File.Exists(trailerPath))
                        System.IO.File.Delete(trailerPath);
                }

                if (movie.Episodes != null)
                {
                    foreach (var episode in movie.Episodes)
                    {
                        if (!string.IsNullOrEmpty(episode.VideoPath))
                        {
                            var videoPath = Path.Combine(_environment.WebRootPath, episode.VideoPath.TrimStart('/'));
                            if (System.IO.File.Exists(videoPath))
                                System.IO.File.Delete(videoPath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xóa file đã tải lên: {Message}", ex.Message);
            }
        }


        // GET: Movies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                {
                    _logger.LogWarning("Movie ID is null in Details action");
                    return NotFound();
                }

                // Fetch movie with related data
                var movie = await _context.Movies
                    .Include(m => m.Country)
                    .Include(m => m.Genre)
                    .Include(m => m.MovieGenres)
                        .ThenInclude(mg => mg.Genre)
                    .Include(m => m.Episodes)
                    .Include(m => m.Ratings)
                    .Include(m => m.Comments)
                        .ThenInclude(c => c.Replies)
                    .FirstOrDefaultAsync(m => m.Id == id);

                if (movie == null)
                {
                    _logger.LogWarning("Movie not found with ID: {Id}", id);
                    return NotFound();
                }

                // Increment views
                movie.Views++; // Fixed line
                await _context.SaveChangesAsync();

                // Get related movies (same genre, excluding current movie)
                var relatedMovies = await _context.Movies
                    .Include(m => m.Genre)
                    .Where(m => m.Id != movie.Id &&
                               (m.GenreId == movie.GenreId ||
                                m.MovieGenres.Any(mg => mg.GenreId == movie.GenreId)))
                    .Take(6)
                    .ToListAsync();

                var viewModel = new MovieDetailViewModel
                {
                    Movie = movie,
                    Episodes = movie.Episodes.OrderBy(e => e.EpisodeNumber).ToList(),
                    Ratings = movie.Ratings.OrderByDescending(r => r.CreatedAt).ToList(),
                    Comments = movie.Comments
                        .Where(c => c.ParentCommentId == null)
                        .OrderByDescending(c => c.CreatedAt)
                        .ToList(),
                    RelatedMovies = relatedMovies,
                    NewRating = new RatingViewModel { MovieId = movie.Id },
                    NewComment = new CommentViewModel { MovieId = movie.Id }
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading movie details for ID: {Id}", id);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải chi tiết phim";
                return RedirectToAction(nameof(Index));
            }
        }


        // GET: Movies/Watch/{movieId}/{episodeNumber}
        public async Task<IActionResult> Watch(int movieId, int episodeNumber)
{
    try
    {
        // Fetch movie with related data
        var movie = await _context.Movies
            .Include(m => m.Episodes)
            .Include(m => m.Country)
            .Include(m => m.Genre)
            .Include(m => m.Ratings)
            .Include(m => m.Comments)
            .FirstOrDefaultAsync(m => m.Id == movieId);

        if (movie == null)
        {
            _logger.LogWarning("Movie not found with ID: {MovieId}", movieId);
            return NotFound();
        }

        // Find the specific episode
        var episode = movie.Episodes.FirstOrDefault(e => e.EpisodeNumber == episodeNumber);
        if (episode == null)
        {
            _logger.LogWarning("Episode {EpisodeNumber} not found for movie ID: {MovieId}", episodeNumber, movieId);
            return NotFound();
        }

        // Increment episode views
        episode.Views++;
        await _context.SaveChangesAsync();

        // Check if the episode is scheduled to air in the future
        var currentDateTime = DateTime.Now; // 02:48 PM +07, June 09, 2025
        var nextEpisode = movie.Episodes
            .Where(e => e.EpisodeNumber > episodeNumber && e.ReleaseDate > currentDateTime)
            .OrderBy(e => e.ReleaseDate)
            .FirstOrDefault();

        var viewModel = new MovieDetailViewModel
        {
            Movie = movie,
            Episodes = movie.Episodes.OrderBy(e => e.EpisodeNumber).ToList(),
            Ratings = movie.Ratings.ToList(),
            Comments = movie.Comments.Where(c => c.EpisodeId == episode.Id).ToList(),
            RelatedMovies = await _context.Movies
                .Where(m => m.GenreId == movie.GenreId && m.Id != movieId)
                .Take(4)
                .ToListAsync(),
            NewRating = new RatingViewModel { MovieId = movie.Id },
            NewComment = new CommentViewModel { MovieId = movie.Id, EpisodeId = episode.Id }
        };

        // Set notification for the next episode if it exists and is airing soon
        if (nextEpisode != null)
        {
            var timeUntilAir = nextEpisode.ReleaseDate - currentDateTime;
            if (timeUntilAir.TotalHours <= 24) // Notify if within 24 hours
            {
                TempData["EpisodeNotification"] = $"Tập {nextEpisode.EpisodeNumber} sẽ phát sóng vào {nextEpisode.ReleaseDate:dd/MM/yyyy HH:mm}.";
            }
        }

        ViewData["CurrentEpisode"] = episode;
        return View(viewModel);
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error loading watch page for movie ID: {MovieId}, episode: {EpisodeNumber}", movieId, episodeNumber);
        TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải trang xem phim";
        return RedirectToAction(nameof(Details), new { id = movieId });
    }
}


[HttpPost]
[ValidateAntiForgeryToken]
public async Task<IActionResult> AddComment(CommentViewModel commentVM)
{
    try
    {
        if (ModelState.IsValid)
        {
            var comment = new Comment
            {
                MovieId = commentVM.MovieId,
                EpisodeId = commentVM.EpisodeId,
                UserName = commentVM.UserName,
                UserEmail = commentVM.UserEmail,
                Content = commentVM.Content,
                ParentCommentId = commentVM.ParentCommentId,
                CreatedAt = DateTime.Now,
                Likes = 0,
                Dislikes = 0
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đã thêm bình luận thành công!";
            if (commentVM.EpisodeId.HasValue)
            {
                // Redirect to Watch page for episode-specific comments
                var episode = await _context.Episodes
                    .FirstOrDefaultAsync(e => e.Id == commentVM.EpisodeId);
                if (episode != null)
                {
                    return RedirectToAction(nameof(Watch), new { movieId = commentVM.MovieId, episodeNumber = episode.EpisodeNumber });
                }
            }
            // Redirect to Details page for movie-wide comments
            return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
        }

        TempData["ErrorMessage"] = "Vui lòng kiểm tra lại thông tin bình luận";
        if (commentVM.EpisodeId.HasValue)
        {
            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.Id == commentVM.EpisodeId);
            if (episode != null)
            {
                return RedirectToAction(nameof(Watch), new { movieId = commentVM.MovieId, episodeNumber = episode.EpisodeNumber });
            }
        }
        return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error adding comment for movie ID: {MovieId}, episode ID: {EpisodeId}", commentVM.MovieId, commentVM.EpisodeId);
        TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm bình luận";
        if (commentVM.EpisodeId.HasValue)
        {
            var episode = await _context.Episodes
                .FirstOrDefaultAsync(e => e.Id == commentVM.EpisodeId);
            if (episode != null)
            {
                return RedirectToAction(nameof(Watch), new { movieId = commentVM.MovieId, episodeNumber = episode.EpisodeNumber });
            }
        }
        return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
    }
}
        // // GET: Movie/Details/5
        //         public async Task<IActionResult> Details(int? id)
        //         {
        //             try
        //             {
        //                 if (id == null)
        //                 {
        //                     TempData["ErrorMessage"] = "Không tìm thấy phim";
        //                     return RedirectToAction(nameof(Index));
        //                 }

        //                 // Lấy thông tin phim cùng các navigation properties
        //                 var movie = await _context.Movies
        //                     .Include(m => m.Country)
        //                     .Include(m => m.Genre)
        //                     .Include(m => m.Episodes)
        //                     .Include(m => m.Ratings)
        //                     .Include(m => m.Comments)
        //                         .ThenInclude(c => c.Replies)
        //                     .Include(m => m.MovieGenres)
        //                         .ThenInclude(mg => mg.Genre)
        //                     .FirstOrDefaultAsync(m => m.Id == id);

        //                 if (movie == null)
        //                 {
        //                     TempData["ErrorMessage"] = "Không tìm thấy phim";
        //                     return RedirectToAction(nameof(Index));
        //                 }

        //                 // Tăng lượt xem
        //                 movie.Views++;
        //                 movie.UpdatedAt = DateTime.Now;
        //                 _context.Update(movie);
        //                 await _context.SaveChangesAsync();

        //                 // Lấy thể loại chính và danh sách thể loại phụ của phim
        //                 var primaryGenreId = movie.GenreId;
        //                 var subGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList();

        //                 // Tìm phim liên quan dựa trên thể loại chính hoặc thể loại phụ
        //                 var relatedMovies = await _context.Movies
        //                     .Include(m => m.Genre)
        //                     .Include(m => m.Country)
        //                     .Where(m => m.Id != movie.Id &&
        //                                 (m.GenreId == primaryGenreId ||
        //                                  m.MovieGenres.Any(mg => subGenreIds.Contains(mg.GenreId))))
        //                     .Take(6)
        //                     .ToListAsync();

        //                 // Tạo ViewModel
        //                 var viewModel = new MovieDetailViewModel
        //                 {
        //                     Movie = movie,
        //                     Episodes = movie.Episodes.OrderBy(e => e.EpisodeNumber).ToList(),
        //                     Ratings = movie.Ratings.OrderByDescending(r => r.CreatedAt).ToList(),
        //                     Comments = movie.Comments
        //                         .Where(c => !c.ParentCommentId.HasValue)
        //                         .OrderByDescending(c => c.CreatedAt)
        //                         .ToList(),
        //                     RelatedMovies = relatedMovies,
        //                     NewRating = new RatingViewModel
        //                     {
        //                         MovieId = movie.Id,
        //                         MovieTitle = movie.Title
        //                     },
        //                     NewComment = new CommentViewModel
        //                     {
        //                         MovieId = movie.Id,
        //                         MovieTitle = movie.Title
        //                     }
        //                 };

        //                 return View(viewModel);
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, $"Lỗi khi tải chi tiết phim với ID: {id}");
        //                 TempData["ErrorMessage"] = "Đã xảy ra lỗi khi tải chi tiết phim";
        //                 return RedirectToAction(nameof(Index));
        //             }
        //         }


        //         // GET: Movie/Edit/5
        //         public async Task<IActionResult> Edit(int? id)
        //         {
        //             if (id == null)
        //             {
        //                 return NotFound();
        //             }

        //             var movie = await _context.Movies
        //                 .Include(m => m.MovieGenres)
        //                 .FirstOrDefaultAsync(m => m.Id == id);

        //             if (movie == null)
        //             {
        //                 return NotFound();
        //             }

        //             var viewModel = new MovieViewModel
        //             {
        //                 Id = movie.Id,
        //                 Title = movie.Title,
        //                 EnglishTitle = movie.EnglishTitle,
        //                 Description = movie.Description,
        //                 ReleaseYear = movie.ReleaseYear,
        //                 CountryId = movie.CountryId,
        //                 GenreId = movie.GenreId,
        //                 Director = movie.Director,
        //                 Cast = movie.Cast,
        //                 TotalEpisodes = movie.TotalEpisodes,
        //                 IsCompleted = movie.IsCompleted,
        //                 PosterPath = movie.PosterPath,
        //                 TrailerPath = movie.TrailerPath,
        //                 SelectedGenreIds = movie.MovieGenres.Select(mg => mg.GenreId).ToList(),
        //                 Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync(),
        //                 Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync()
        //             };

        //             return View(viewModel);
        //         }

        //         // POST: Movie/Edit/5
        //         [HttpPost]
        //         [ValidateAntiForgeryToken]
        //         [RequestSizeLimit(524_288_000)] // 500MB
        //         public async Task<IActionResult> Edit(int id, MovieViewModel movieVM)
        //         {
        //             if (id != movieVM.Id)
        //             {
        //                 return NotFound();
        //             }

        //             if (!ModelState.IsValid)
        //             {
        //                 movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
        //                 movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        //                 return View(movieVM);
        //             }

        //             try
        //             {
        //                 var movie = await _context.Movies
        //                     .Include(m => m.MovieGenres)
        //                     .FirstOrDefaultAsync(m => m.Id == id);

        //                 if (movie == null)
        //                 {
        //                     return NotFound();
        //                 }

        //                 movie.Title = movieVM.Title;
        //                 movie.EnglishTitle = movieVM.EnglishTitle;
        //                 movie.Description = movieVM.Description;
        //                 movie.ReleaseYear = movieVM.ReleaseYear;
        //                 movie.CountryId = movieVM.CountryId;
        //                 movie.GenreId = movieVM.GenreId;
        //                 movie.Director = movieVM.Director;
        //                 movie.Cast = movieVM.Cast;
        //                 movie.TotalEpisodes = movieVM.TotalEpisodes;
        //                 movie.IsCompleted = movieVM.IsCompleted;
        //                 movie.UpdatedAt = DateTime.Now;

        //                 string uploadsFolder = Path.Combine(_environment.WebRootPath, "Uploads");

        //                 // Upload new Poster if provided
        //                 if (movieVM.PosterFile != null && movieVM.PosterFile.Length > 0)
        //                 {
        //                     if (!string.IsNullOrEmpty(movie.PosterPath))
        //                     {
        //                         var oldFilePath = Path.Combine(_environment.WebRootPath, movie.PosterPath.TrimStart('/'));
        //                         if (System.IO.File.Exists(oldFilePath))
        //                         {
        //                             System.IO.File.Delete(oldFilePath);
        //                         }
        //                     }
        //                     movie.PosterPath = await ProcessFileUpload(movieVM.PosterFile, uploadsFolder, "poster");
        //                 }

        //                 // Upload new Trailer if provided
        //                 if (movieVM.TrailerFile != null && movieVM.TrailerFile.Length > 0)
        //                 {
        //                     if (!string.IsNullOrEmpty(movie.TrailerPath))
        //                     {
        //                         var oldFilePath = Path.Combine(_environment.WebRootPath, movie.TrailerPath.TrimStart('/'));
        //                         if (System.IO.File.Exists(oldFilePath))
        //                         {
        //                             System.IO.File.Delete(oldFilePath);
        //                         }
        //                     }
        //                     movie.TrailerPath = await ProcessFileUpload(movieVM.TrailerFile, uploadsFolder, "trailer");
        //                 }

        //                 // Update genres
        //                 movie.MovieGenres.Clear();
        //                 if (movieVM.SelectedGenreIds != null)
        //                 {
        //                     movie.MovieGenres = movieVM.SelectedGenreIds
        //                         .Select(id => new MovieGenre { MovieId = movie.Id, GenreId = id })
        //                         .ToList();
        //                 }

        //                 _context.Update(movie);
        //                 await _context.SaveChangesAsync();

        //                 _logger.LogInformation($"Updated movie: {movie.Title} (ID: {movie.Id})");
        //                 TempData["SuccessMessage"] = $"Đã cập nhật phim '{movie.Title}' thành công!";
        //                 return RedirectToAction(nameof(Index));
        //             }
        //             catch (DbUpdateConcurrencyException)
        //             {
        //                 if (!MovieExists(id))
        //                 {
        //                     return NotFound();
        //                 }
        //                 throw;
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, $"Error updating movie ID: {id}");
        //                 TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi cập nhật phim: {ex.Message}";
        //                 movieVM.Countries = await _context.Countries.OrderBy(c => c.Name).ToListAsync();
        //                 movieVM.Genres = await _context.Genres.OrderBy(g => g.Name).ToListAsync();
        //                 return View(movieVM);
        //             }
        //         }

        //         // GET: Movie/Delete/5
        //         public async Task<IActionResult> Delete(int? id)
        //         {
        //             if (id == null)
        //             {
        //                 return NotFound();
        //             }

        //             var movie = await _context.Movies
        //                 .Include(m => m.Country)
        //                 .Include(m => m.Genre)
        //                 .FirstOrDefaultAsync(m => m.Id == id);

        //             if (movie == null)
        //             {
        //                 return NotFound();
        //             }

        //             return View(movie);
        //         }

        //         // POST: Movie/Delete/5
        //         [HttpPost, ActionName("Delete")]
        //         [ValidateAntiForgeryToken]
        //         public async Task<IActionResult> DeleteConfirmed(int id)
        //         {
        //             try
        //             {
        //                 var movie = await _context.Movies
        //                     .Include(m => m.Episodes)
        //                     .Include(m => m.MovieGenres)
        //                     .FirstOrDefaultAsync(m => m.Id == id);

        //                 if (movie != null)
        //                 {
        //                     // Delete associated files
        //                     CleanupUploadedFiles(movie);

        //                     // Delete associated episodes' files
        //                     foreach (var episode in movie.Episodes)
        //                     {
        //                         if (!string.IsNullOrEmpty(episode.VideoPath))
        //                         {
        //                             var path = Path.Combine(_environment.WebRootPath, episode.VideoPath.TrimStart('/'));
        //                             if (System.IO.File.Exists(path))
        //                             {
        //                                 System.IO.File.Delete(path);
        //                             }
        //                         }
        //                     }

        //                     _context.Movies.Remove(movie);
        //                     await _context.SaveChangesAsync();

        //                     _logger.LogInformation($"Deleted movie: {movie.Title} (ID: {movie.Id})");
        //                     TempData["SuccessMessage"] = $"Đã xóa phim '{movie.Title}' thành công!";
        //                 }
        //                 return RedirectToAction(nameof(Index));
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, $"Error deleting movie ID: {id}");
        //                 TempData["ErrorMessage"] = $"Đã xảy ra lỗi khi xóa phim: {ex.Message}";
        //                 return RedirectToAction(nameof(Index));
        //             }
        //         }

        //         // POST: Movie/AddRating
        //         [HttpPost]
        //         [ValidateAntiForgeryToken]
        //         public async Task<IActionResult> AddRating(RatingViewModel ratingVM)
        //         {
        //             if (!ModelState.IsValid)
        //             {
        //                 return RedirectToAction(nameof(Details), new { id = ratingVM.MovieId });
        //             }

        //             try
        //             {
        //                 var rating = new Rating
        //                 {
        //                     MovieId = ratingVM.MovieId,
        //                     UserName = ratingVM.UserName,
        //                     UserEmail = ratingVM.UserEmail,
        //                     Score = ratingVM.Score,
        //                     Review = ratingVM.Review,
        //                     CreatedAt = DateTime.Now
        //                 };

        //                 _context.Ratings.Add(rating);
        //                 await _context.SaveChangesAsync();

        //                 // Update movie's average rating
        //                 var movie = await _context.Movies
        //                     .Include(m => m.Ratings)
        //                     .FirstOrDefaultAsync(m => m.Id == ratingVM.MovieId);

        //                 if (movie != null)
        //                 {
        //                     movie.RatingCount = movie.Ratings.Count;
        //                     movie.AverageRating = movie.Ratings.Average(r => r.Score);
        //                     await _context.SaveChangesAsync();
        //                 }

        //                 TempData["SuccessMessage"] = "Đã thêm đánh giá thành công!";
        //                 return RedirectToAction(nameof(Details), new { id = ratingVM.MovieId });
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, $"Error adding rating for movie ID: {ratingVM.MovieId}");
        //                 TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm đánh giá";
        //                 return RedirectToAction(nameof(Details), new { id = ratingVM.MovieId });
        //             }
        //         }

        //         // POST: Movie/AddComment
        //         [HttpPost]
        //         [ValidateAntiForgeryToken]
        //         public async Task<IActionResult> AddComment(CommentViewModel commentVM)
        //         {
        //             if (!ModelState.IsValid)
        //             {
        //                 return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
        //             }

        //             try
        //             {
        //                 var comment = new Comment
        //                 {
        //                     MovieId = commentVM.MovieId,
        //                     EpisodeId = commentVM.EpisodeId,
        //                     UserName = commentVM.UserName,
        //                     UserEmail = commentVM.UserEmail,
        //                     Content = commentVM.Content,
        //                     ParentCommentId = commentVM.ParentCommentId,
        //                     CreatedAt = DateTime.Now
        //                 };

        //                 _context.Comments.Add(comment);
        //                 await _context.SaveChangesAsync();

        //                 TempData["SuccessMessage"] = "Đã thêm bình luận thành công!";
        //                 return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
        //             }
        //             catch (Exception ex)
        //             {
        //                 _logger.LogError(ex, $"Error adding comment for movie ID: {commentVM.MovieId}");
        //                 TempData["ErrorMessage"] = "Đã xảy ra lỗi khi thêm bình luận";
        //                 return RedirectToAction(nameof(Details), new { id = commentVM.MovieId });
        //             }
        //         }

        //         private bool MovieExists(int id)
        //         {
        //             return _context.Movies.Any(e => e.Id == id);
        //         }

        //         #region Helper Methods
        //         #endregion
    }
}