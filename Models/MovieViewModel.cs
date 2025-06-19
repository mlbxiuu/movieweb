
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;
using MovieWebsite.Models;

public class MovieViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
    [MaxLength(200, ErrorMessage = "Tiêu đề không được vượt quá 200 ký tự")]
    public string Title { get; set; }

    [MaxLength(200, ErrorMessage = "Tiêu đề tiếng Anh không được vượt quá 200 ký tự")]
    public string EnglishTitle { get; set; }

    [Required(ErrorMessage = "Mô tả là bắt buộc")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Năm phát hành là bắt buộc")]
    [Range(1900, 2100, ErrorMessage = "Năm phát hành phải từ 1900 đến 2100")]
    public int ReleaseYear { get; set; }

    [Required(ErrorMessage = "Quốc gia là bắt buộc")]
    public int CountryId { get; set; }

    [Required(ErrorMessage = "Thể loại chính là bắt buộc")]
    public int GenreId { get; set; }

    [MaxLength(500, ErrorMessage = "Tên đạo diễn không được vượt quá 500 ký tự")]
    public string Director { get; set; }

    [MaxLength(1000, ErrorMessage = "Danh sách diễn viên không được vượt quá 1000 ký tự")]
    public string Cast { get; set; }

    [Required(ErrorMessage = "Tổng số tập là bắt buộc")]
    [Range(1, int.MaxValue, ErrorMessage = "Tổng số tập phải lớn hơn 0")]
    public int TotalEpisodes { get; set; } = 1;

    public bool IsCompleted { get; set; } = false;

    [Required(ErrorMessage = "Vui lòng chọn file poster")]

    public IFormFile PosterFile { get; set; }

    public IFormFile TrailerFile { get; set; }

    public List<int> SelectedGenreIds { get; set; } = new List<int>();
    public List<Country> Countries { get; set; } = new List<Country>();
    public List<Genre> Genres { get; set; } = new List<Genre>();

    public string? PosterPath { get; set; } // Dùng khi chỉnh sửa

    public string? TrailerPath { get; set; } // Dùng khi chỉnh sửa

    // New property for episode uploads
    public List<EpisodeViewModel> Episodes { get; set; } = new List<EpisodeViewModel>();
    
        // Dùng để hiển thị danh sách các tập đã có trên trang Edit
    public List<Episode> ExistingEpisodes { get; set; } = new List<Episode>();

}


// ViewModel cho MovieGenre
public class MovieGenreViewModel
{
    public List<Movie> Movies { get; set; }
    public SelectList Genres { get; set; }
    public string MovieGenre { get; set; }
    public string SearchString { get; set; }
}


// ViewModel cho Episode
public class EpisodeViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn phim")]
    public int MovieId { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập số tập")]
    [Range(1, int.MaxValue, ErrorMessage = "Số tập phải lớn hơn 0")]
    public int EpisodeNumber { get; set; }

    [Required(ErrorMessage = "Vui lòng nhập tên tập")]
    [StringLength(200, ErrorMessage = "Tên tập không được vượt quá 200 ký tự")]
    public string Title { get; set; }

    [StringLength(1000, ErrorMessage = "Mô tả tập không được vượt quá 1000 ký tự")]
    public string Description { get; set; }

    [Required(ErrorMessage = "Vui lòng tải lên video")]
    public IFormFile VideoFile { get; set; }

    [Range(1, 600, ErrorMessage = "Thời lượng phải từ 1-600 phút")]
    public int Duration { get; set; }

    [Required(ErrorMessage = "Vui lòng chọn ngày phát hành")]
    public DateTime ReleaseDate { get; set; } = DateTime.Now;

    // For display purposes (not required)
    public string? VideoPath { get; set; }
    public string MovieTitle { get; set; } // Không cần [Required]
}

    // ViewModel cho Rating
    public class RatingViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn phim để đánh giá")]
        public int MovieId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn điểm đánh giá")]
        [Range(1, 10, ErrorMessage = "Điểm đánh giá phải từ 1-10")]
        public int Score { get; set; }

        [StringLength(500, ErrorMessage = "Nhận xét không được vượt quá 500 ký tự")]
        public string Review { get; set; }

        // For display purposes
        public string MovieTitle { get; set; }
    }

    // ViewModel cho Comment
    public class CommentViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn phim")]
        public int MovieId { get; set; }

        public int? EpisodeId { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập tên của bạn")]
        [StringLength(100, ErrorMessage = "Tên không được vượt quá 100 ký tự")]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập nội dung bình luận")]
        [StringLength(1000, ErrorMessage = "Bình luận không được vượt quá 1000 ký tự")]
        public string Content { get; set; }

        public int? ParentCommentId { get; set; }

        // For display purposes
        public string MovieTitle { get; set; }
        public string EpisodeTitle { get; set; }
        public DateTime CreatedAt { get; set; }
        public int Likes { get; set; }
        public int Dislikes { get; set; }
        public List<CommentViewModel> Replies { get; set; } = new List<CommentViewModel>();
    }

    // ViewModel cho tìm kiếm và lọc
    public class MovieSearchViewModel
    {
        public string SearchTerm { get; set; }
        public int? CountryId { get; set; }
        public int? GenreId { get; set; }
        public int? Year { get; set; }
        public string SortBy { get; set; } = "newest"; // newest, oldest, rating, views
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 12;

        // For display
        public List<Movie> Movies { get; set; } = new List<Movie>();
        public List<Country> Countries { get; set; } = new List<Country>();
        public List<Genre> Genres { get; set; } = new List<Genre>();
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
    }

    // ViewModel cho trang chi tiết phim
    public class MovieDetailViewModel
    {
        public Movie Movie { get; set; }
        public List<Episode> Episodes { get; set; } = new List<Episode>();
        public List<Rating> Ratings { get; set; } = new List<Rating>();
        public List<Comment> Comments { get; set; } = new List<Comment>();
        public List<Movie> RelatedMovies { get; set; } = new List<Movie>();

        // Form để thêm rating/comment mới
        public RatingViewModel NewRating { get; set; } = new RatingViewModel();
        public CommentViewModel NewComment { get; set; } = new CommentViewModel();
    }

    
public class HomeViewModel1
{
    public List<Genre> PopularGenres { get; set; }
    public List<Movie> LatestMovies { get; set; }
    public List<Movie> TopRatedMovies { get; set; }
    // Các section khác nếu cần
    public List<Genre> Genres { get; set; } = new List<Genre>();
}

