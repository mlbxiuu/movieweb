// using Microsoft.EntityFrameworkCore;
// using System;
// using System.Collections.Generic;
// using System.ComponentModel.DataAnnotations;

// namespace MovieWebsite.Models
// {
//     public class Movie
//     {
//         public int Id { get; set; }

//         [Required]
//         [MaxLength(200)]
//         public string Title { get; set; } // Tên phim tiếng Việt

//         [MaxLength(200)]
//         public string EnglishTitle { get; set; } // Tên phim tiếng Anh

//         [Required]
//         public string Description { get; set; } // Mô tả phim

//         public int ReleaseYear { get; set; } // Năm phát hành

//         [Required]
//         public int CountryId { get; set; } // Quốc gia sản xuất
//         public Country Country { get; set; }

//         [Required]
//         public int GenreId { get; set; } // Thể loại chính
//         public Genre Genre { get; set; }

//         [MaxLength(500)]
//         public string Director { get; set; } // Đạo diễn

//         [MaxLength(1000)]
//         public string Cast { get; set; } // Diễn viên

//         public string PosterPath { get; set; } // Đường dẫn poster
//         public string TrailerPath { get; set; } // Đường dẫn trailer

//         public int TotalEpisodes { get; set; } = 1; // Tổng số tập (1 nếu là phim lẻ)
//         public bool IsCompleted { get; set; } = false; // Đã hoàn thành chưa

//         public DateTime CreatedAt { get; set; } = DateTime.Now;
//         public DateTime UpdatedAt { get; set; } = DateTime.Now;

//         public int Views { get; set; } = 0; // Lượt xem
//         public double AverageRating { get; set; } = 0.0; // Điểm đánh giá trung bình
//         public int RatingCount { get; set; } = 0; // Số lượt đánh giá

//         // Navigation properties
//         public ICollection<Episode> Episodes { get; set; } = new List<Episode>();
//         public ICollection<Rating> Ratings { get; set; } = new List<Rating>();
//         public ICollection<Comment> Comments { get; set; } = new List<Comment>();
//         public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>(); // Many-to-many với Genre
//     }

//     public class Episode
//     {
//         public int Id { get; set; }

//         [Required]
//         public int MovieId { get; set; }
//         public Movie Movie { get; set; }

//         public int EpisodeNumber { get; set; } // Số tập

//         [Required]
//         [MaxLength(200)]
//         public string Title { get; set; } // Tên tập phim

//         public string Description { get; set; } // Mô tả tập phim

//         [Required]
//         public string VideoPath { get; set; } // Đường dẫn video

//         public int Duration { get; set; } // Thời lượng (phút)
//         public DateTime ReleaseDate { get; set; } // Ngày phát hành tập
//         public int Views { get; set; } = 0; // Lượt xem tập này

//         public DateTime CreatedAt { get; set; } = DateTime.Now;
//         public DateTime UpdatedAt { get; set; } = DateTime.Now;
//     }

//     public class Country
//     {
//         public int Id { get; set; }

//         [Required]
//         [MaxLength(100)]
//         public string Name { get; set; } // Tên quốc gia

//         [MaxLength(10)]
//         public string Code { get; set; } // Mã quốc gia (VN, US, KR, etc.)

//         public string? FlagPath { get; set; } // Đường dẫn icon quốc gia

//         // Navigation properties
//         public ICollection<Movie> Movies { get; set; } = new List<Movie>();
//     }

//     public class Genre
//     {
//         public int Id { get; set; }

//         [Required]
//         [MaxLength(100)]
//         public string Name { get; set; } // Tên thể loại

//         public string? Description { get; set; } // Mô tả thể loại
//         public string Color { get; set; } // Màu đại diện cho thể loại

//         // Navigation properties
//         public ICollection<Movie> Movies { get; set; } = new List<Movie>();
//         public ICollection<MovieGenre> MovieGenres { get; set; } = new List<MovieGenre>();
//     }

//     // Bảng trung gian cho many-to-many relationship giữa Movie và Genre
//     public class MovieGenre
//     {
//         public int MovieId { get; set; }
//         public Movie Movie { get; set; }

//         public int GenreId { get; set; }
//         public Genre Genre { get; set; }
//     }

//     public class Rating
//     {
//         public int Id { get; set; }

//         [Required]
//         public int MovieId { get; set; }
//         public Movie Movie { get; set; }

//         [Required]
//         [MaxLength(100)]
//         public string UserName { get; set; } // Tên người đánh giá

//         public string UserEmail { get; set; } // Email người đánh giá

//         [Range(1, 10)]
//         public int Score { get; set; } // Điểm đánh giá (1-10)

//         [MaxLength(500)]
//         public string Review { get; set; } // Nhận xét

//         public DateTime CreatedAt { get; set; } = DateTime.Now;
//     }

//     public class Comment
//     {
//         public int Id { get; set; }

//         [Required]
//         public int MovieId { get; set; }
//         public Movie Movie { get; set; }

//         public int? EpisodeId { get; set; } // Null nếu comment cho cả phim
//         public Episode Episode { get; set; }

//         [Required]
//         [MaxLength(100)]
//         public string UserName { get; set; } // Tên người bình luận

//         public string UserEmail { get; set; } // Email người bình luận

//         [Required]
//         public string Content { get; set; } // Nội dung bình luận

//         public int? ParentCommentId { get; set; } // Để reply comment
//         public Comment ParentComment { get; set; }

//         public int Likes { get; set; } = 0; // Số lượt thích
//         public int Dislikes { get; set; } = 0; // Số lượt không thích

//         public DateTime CreatedAt { get; set; } = DateTime.Now;

//         // Navigation properties
//         public ICollection<Comment> Replies { get; set; } = new List<Comment>();
//     }

//     public class AppDbContext : DbContext
//     {
//         public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

//         public DbSet<Movie> Movies { get; set; }
//         public DbSet<Episode> Episodes { get; set; }
//         public DbSet<Country> Countries { get; set; }
//         public DbSet<Genre> Genres { get; set; }
//         public DbSet<MovieGenre> MovieGenres { get; set; }
//         public DbSet<Rating> Ratings { get; set; }
//         public DbSet<Comment> Comments { get; set; }

//         protected override void OnModelCreating(ModelBuilder modelBuilder)
//         {
//             base.OnModelCreating(modelBuilder);

//             // Cấu hình many-to-many relationship cho Movie và Genre
//             modelBuilder.Entity<MovieGenre>()
//                 .HasKey(mg => new { mg.MovieId, mg.GenreId });

//             modelBuilder.Entity<MovieGenre>()
//                 .HasOne(mg => mg.Movie)
//                 .WithMany(m => m.MovieGenres)
//                 .HasForeignKey(mg => mg.MovieId);

//             modelBuilder.Entity<MovieGenre>()
//                 .HasOne(mg => mg.Genre)
//                 .WithMany(g => g.MovieGenres)
//                 .HasForeignKey(mg => mg.GenreId);

//             // Cấu hình self-referencing cho Comment (reply)
//             modelBuilder.Entity<Comment>()
//                 .HasOne(c => c.ParentComment)
//                 .WithMany(c => c.Replies)
//                 .HasForeignKey(c => c.ParentCommentId)
//                 .OnDelete(DeleteBehavior.Restrict);

//             // Cấu hình indexes để tối ưu hiệu suất
//             modelBuilder.Entity<Movie>()
//                 .HasIndex(m => m.Title);

//             modelBuilder.Entity<Movie>()
//                 .HasIndex(m => m.ReleaseYear);

//             modelBuilder.Entity<Episode>()
//                 .HasIndex(e => new { e.MovieId, e.EpisodeNumber })
//                 .IsUnique();

//             // Seed data mẫu cho Countries
//             modelBuilder.Entity<Country>().HasData(
//                 new Country { Id = 1, Name = "Việt Nam", Code = "VN" },
//                 new Country { Id = 2, Name = "Hàn Quốc", Code = "KR" },
//                 new Country { Id = 3, Name = "Trung Quốc", Code = "CN" },
//                 new Country { Id = 4, Name = "Nhật Bản", Code = "JP" },
//                 new Country { Id = 5, Name = "Mỹ", Code = "US" },
//                 new Country { Id = 6, Name = "Thái Lan", Code = "TH" }
//             );

//             // Seed data mẫu cho Genres
//             modelBuilder.Entity<Genre>().HasData(
//                 new Genre { Id = 1, Name = "Hành Động", Color = "#FF5722" },
//                 new Genre { Id = 2, Name = "Tình Cảm", Color = "#E91E63" },
//                 new Genre { Id = 3, Name = "Hài Hước", Color = "#FFC107" },
//                 new Genre { Id = 4, Name = "Kinh Dị", Color = "#424242" },
//                 new Genre { Id = 5, Name = "Khoa Học Viễn Tưởng", Color = "#2196F3" },
//                 new Genre { Id = 6, Name = "Phiêu Lưu", Color = "#4CAF50" },
//                 new Genre { Id = 7, Name = "Chính Kịch", Color = "#9C27B0" },
//                 new Genre { Id = 8, Name = "Hoạt Hình", Color = "#FF9800" }
//             );


//             // Seed data mẫu cho Movies
//             modelBuilder.Entity<Movie>().HasData(
//                 new Movie
//                 {
//                     Id = 1,
//                     Title = "Bố Già",
//                     EnglishTitle = "Old Father",
//                     Description = "Bộ phim kể về cuộc sống của một gia đình lao động nghèo ở Sài Gòn.",
//                     ReleaseYear = 2021,
//                     CountryId = 1, // Việt Nam
//                     GenreId = 7, // Chính Kịch
//                     Director = "Trấn Thành",
//                     Cast = "Trấn Thành, Tuấn Trần, Ngô Kiến Huy",
//                     PosterPath = "/images/1.png",
//                     TrailerPath = "/videos/1.mp4",
//                     TotalEpisodes = 1,
//                     IsCompleted = true,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now,
//                     Views = 150000,
//                     AverageRating = 8.5,
//                     RatingCount = 1200
//                 },
//                 new Movie
//                 {
//                     Id = 2,
//                     Title = "Hạ Cánh Nơi Anh",
//                     EnglishTitle = "Crash Landing on You",
//                     Description = "Câu chuyện tình yêu giữa một nữ thừa kế Hàn Quốc và một sĩ quan Bắc Triều Tiên.",
//                     ReleaseYear = 2019,
//                     CountryId = 2, // Hàn Quốc
//                     GenreId = 2, // Tình Cảm
//                     Director = "Lee Jeong-hyo",
//                     Cast = "Hyun Bin, Son Ye-jin",
//                     PosterPath = "/images/2.png",
//                     TrailerPath = "/videos/2.mp4",
//                     TotalEpisodes = 16,
//                     IsCompleted = true,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now,
//                     Views = 250000,
//                     AverageRating = 9.0,
//                     RatingCount = 2000
//                 },
//                 new Movie
//                 {
//                     Id = 3,
//                     Title = "Ký Sinh Trùng",
//                     EnglishTitle = "Parasite",
//                     Description = "Một gia đình nghèo tìm cách thâm nhập vào cuộc sống của một gia đình giàu có.",
//                     ReleaseYear = 2019,
//                     CountryId = 2, // Hàn Quốc
//                     GenreId = 7, // Chính Kịch
//                     Director = "Bong Joon-ho",
//                     Cast = "Song Kang-ho, Lee Sun-kyun, Cho Yeo-jeong",
//                     PosterPath = "/images/3.png",
//                     TrailerPath = "/videos/3.mp4",
//                     TotalEpisodes = 1,
//                     IsCompleted = true,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now,
//                     Views = 300000,
//                     AverageRating = 9.2,
//                     RatingCount = 2500
//                 }
//             );

//             // Seed data mẫu cho MovieGenres (many-to-many)
//             modelBuilder.Entity<MovieGenre>().HasData(
//                 new MovieGenre { MovieId = 1, GenreId = 7 }, // Bố Già - Chính Kịch
//                 new MovieGenre { MovieId = 1, GenreId = 3 }, // Bố Già - Hài Hước
//                 new MovieGenre { MovieId = 2, GenreId = 2 }, // Hạ Cánh Nơi Anh - Tình Cảm
//                 new MovieGenre { MovieId = 2, GenreId = 3 }, // Hạ Cánh Nơi Anh - Hài Hước
//                 new MovieGenre { MovieId = 3, GenreId = 7 }, // Ký Sinh Trùng - Chính Kịch
//                 new MovieGenre { MovieId = 3, GenreId = 4 }  // Ký Sinh Trùng - Kinh Dị
//             );

//             // Seed data mẫu cho Episodes
//             modelBuilder.Entity<Episode>().HasData(
//                 // Tập phim cho Hạ Cánh Nơi Anh (MovieId = 2, 16 tập)
//                 new Episode
//                 {
//                     Id = 1,
//                     MovieId = 2,
//                     EpisodeNumber = 1,
//                     Title = "Tập 1: Cuộc gặp gỡ định mệnh",
//                     Description = "Yoon Se-ri vô tình hạ cánh xuống Bắc Triều Tiên sau một tai nạn.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 70,
//                     ReleaseDate = new DateTime(2019, 12, 14),
//                     Views = 10000,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 },
//                 new Episode
//                 {
//                     Id = 2,
//                     MovieId = 2,
//                     EpisodeNumber = 2,
//                     Title = "Tập 2: Kế hoạch trốn thoát",
//                     Description = "Ri Jeong-hyeok giúp Se-ri tìm cách trở về Hàn Quốc.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 65,
//                     ReleaseDate = new DateTime(2019, 12, 15),
//                     Views = 9500,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 },
//                 new Episode
//                 {
//                     Id = 3,
//                     MovieId = 2,
//                     EpisodeNumber = 3,
//                     Title = "Tập 3: Bí mật bị hé lộ",
//                     Description = "Se-ri dần thích nghi với cuộc sống ở Bắc Triều Tiên.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 68,
//                     ReleaseDate = new DateTime(2019, 12, 21),
//                     Views = 9000,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 },
//                 new Episode
//                 {
//                     Id = 4,
//                     MovieId = 2,
//                     EpisodeNumber = 4,
//                     Title = "Tập 4: Tình cảm nảy nở",
//                     Description = "Tình cảm giữa Se-ri và Jeong-hyeok bắt đầu phát triển.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 72,
//                     ReleaseDate = new DateTime(2019, 12, 22),
//                     Views = 8800,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 },
//                 // Thêm các tập khác (tạm thời chỉ thêm 4 tập để minh họa, bạn có thể mở rộng đến 16 tập)
//                 new Episode
//                 {
//                     Id = 5,
//                     MovieId = 2,
//                     EpisodeNumber = 5,
//                     Title = "Tập 5: Thách thức mới",
//                     Description = "Se-ri đối mặt với những nguy hiểm mới ở Bắc Triều Tiên.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 70,
//                     ReleaseDate = new DateTime(2019, 12, 28),
//                     Views = 8500,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 },
//                 new Episode
//                 {
//                     Id = 6,
//                     MovieId = 2,
//                     EpisodeNumber = 6,
//                     Title = "Tập 6: Hành trình nguy hiểm",
//                     Description = "Jeong-hyeok lên kế hoạch bảo vệ Se-ri.",
//                     VideoPath = "/videos/1.mp4",
//                     Duration = 67,
//                     ReleaseDate = new DateTime(2019, 12, 29),
//                     Views = 8200,
//                     CreatedAt = DateTime.Now,
//                     UpdatedAt = DateTime.Now
//                 }
//             // Có thể thêm các tập từ 7 đến 16 tương tự nếu cần
//             );

//             // Seed data mẫu cho Ratings
//             modelBuilder.Entity<Rating>().HasData(
//                 new Rating
//                 {
//                     Id = 1,
//                     MovieId = 1, // Bố Già
//                     UserName = "NguyenVanA",
//                     UserEmail = "nguyenvana@example.com",
//                     Score = 8,
//                     Review = "Phim rất cảm động, diễn xuất của Trấn Thành tuyệt vời!",
//                     CreatedAt = DateTime.Now
//                 },
//                 new Rating
//                 {
//                     Id = 2,
//                     MovieId = 1, // Bố Già
//                     UserName = "TranThiB",
//                     UserEmail = "tranthib@example.com",
//                     Score = 9,
//                     Review = "Cốt truyện gần gũi, phản ánh đúng cuộc sống.",
//                     CreatedAt = DateTime.Now
//                 },
//                 new Rating
//                 {
//                     Id = 3,
//                     MovieId = 3, // Ký Sinh Trùng
//                     UserName = "LeVanC",
//                     UserEmail = "levanc@example.com",
//                     Score = 10,
//                     Review = "Một kiệt tác của điện ảnh Hàn Quốc!",
//                     CreatedAt = DateTime.Now
//                 }
//             );

//             // Seed data mẫu cho Comments
//             modelBuilder.Entity<Comment>().HasData(
//                 new Comment
//                 {
//                     Id = 1,
//                     MovieId = 1, // Bố Già
//                     EpisodeId = null,
//                     UserName = "NguyenVanA",
//                     UserEmail = "nguyenvana@example.com",
//                     Content = "Phim này thực sự làm mình khóc rất nhiều!",
//                     Likes = 50,
//                     Dislikes = 2,
//                     CreatedAt = DateTime.Now
//                 },
//                 new Comment
//                 {
//                     Id = 2,
//                     MovieId = 1, // Bố Già
//                     EpisodeId = null,
//                     UserName = "TranThiB",
//                     UserEmail = "tranthib@example.com",
//                     Content = "Cảm ơn Trấn Thành đã mang đến một bộ phim ý nghĩa!",
//                     ParentCommentId = 1, // Reply cho comment 1
//                     Likes = 30,
//                     Dislikes = 1,
//                     CreatedAt = DateTime.Now
//                 },
//                 new Comment
//                 {
//                     Id = 3,
//                     MovieId = 2, // Hạ Cánh Nơi Anh
//                     EpisodeId = 1, // Tập 1
//                     UserName = "LeVanC",
//                     UserEmail = "levanc@example.com",
//                     Content = "Tập 1 rất hấp dẫn, chemistry giữa hai diễn viên chính đỉnh cao!",
//                     Likes = 40,
//                     Dislikes = 0,
//                     CreatedAt = DateTime.Now
//                 },
//                 new Comment
//                 {
//                     Id = 4,
//                     MovieId = 2, // Hạ Cánh Nơi Anh
//                     EpisodeId = 2, // Tập 2
//                     UserName = "PhamThiD",
//                     UserEmail = "phamthid@example.com",
//                     Content = "Tập 2 càng cuốn, không thể rời mắt!",
//                     Likes = 35,
//                     Dislikes = 1,
//                     CreatedAt = DateTime.Now
//                 }
//             );
//         }
//     }
// }