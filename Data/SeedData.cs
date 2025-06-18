using Microsoft.EntityFrameworkCore;
using MovieWebsite.Models;

namespace MovieWebsite.Data
{
    public static class SeedData
    {
        public static void Seed(ModelBuilder modelBuilder)
        {
            // Seed Countries
            modelBuilder.Entity<Country>().HasData(
                new Country { Id = 1, Name = "Việt Nam", Code = "VN" },
                new Country { Id = 2, Name = "Hàn Quốc", Code = "KR" },
                new Country { Id = 3, Name = "Trung Quốc", Code = "CN" },
                new Country { Id = 4, Name = "Nhật Bản", Code = "JP" },
                new Country { Id = 5, Name = "Mỹ", Code = "US" },
                new Country { Id = 6, Name = "Thái Lan", Code = "TH" }
            );

            // Seed Genres
            modelBuilder.Entity<Genre>().HasData(
                new Genre { Id = 1, Name = "Hành Động", Color = "#FF5722" },
                new Genre { Id = 2, Name = "Tình Cảm", Color = "#E91E63" },
                new Genre { Id = 3, Name = "Hài Hước", Color = "#FFC107" },
                new Genre { Id = 4, Name = "Kinh Dị", Color = "#424242" },
                new Genre { Id = 5, Name = "Khoa Học Viễn Tưởng", Color = "#2196F3" },
                new Genre { Id = 6, Name = "Phiêu Lưu", Color = "#4CAF50" },
                new Genre { Id = 7, Name = "Chính Kịch", Color = "#9C27B0" },
                new Genre { Id = 8, Name = "Hoạt Hình", Color = "#FF9800" }
            );

            // Seed Movies
            modelBuilder.Entity<Movie>().HasData(
                new Movie
                {
                    Id = 1,
                    Title = "Bố Già",
                    EnglishTitle = "Old Father",
                    Description = "Bộ phim kể về cuộc sống của một gia đình lao động nghèo ở Sài Gòn.",
                    ReleaseYear = 2021,
                    CountryId = 1,
                    GenreId = 7,
                    Director = "Trấn Thành",
                    Cast = "Trấn Thành, Tuấn Trần, Ngô Kiến Huy",
                    PosterPath = "/images/1.png",
                    TrailerPath = "/videos/1.mp4",
                    TotalEpisodes = 1,
                    IsCompleted = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Views = 150000,
                    AverageRating = 8.5,
                    RatingCount = 1200
                },
                new Movie
                {
                    Id = 2,
                    Title = "Hạ Cánh Nơi Anh",
                    EnglishTitle = "Crash Landing on You",
                    Description = "Câu chuyện tình yêu giữa một nữ thừa kế Hàn Quốc và một sĩ quan Bắc Triều Tiên.",
                    ReleaseYear = 2019,
                    CountryId = 2,
                    GenreId = 2,
                    Director = "Lee Jeong-hyo",
                    Cast = "Hyun Bin, Son Ye-jin",
                    PosterPath = "/images/2.png",
                    TrailerPath = "/videos/2.mp4",
                    TotalEpisodes = 16,
                    IsCompleted = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Views = 250000,
                    AverageRating = 9.0,
                    RatingCount = 2000
                },
                new Movie
                {
                    Id = 3,
                    Title = "Ký Sinh Trùng",
                    EnglishTitle = "Parasite",
                    Description = "Một gia đình nghèo tìm cách thâm nhập vào cuộc sống của một gia đình giàu có.",
                    ReleaseYear = 2019,
                    CountryId = 2,
                    GenreId = 7,
                    Director = "Bong Joon-ho",
                    Cast = "Song Kang-ho, Lee Sun-kyun, Cho Yeo-jeong",
                    PosterPath = "/images/3.png",
                    TrailerPath = "/videos/3.mp4",
                    TotalEpisodes = 1,
                    IsCompleted = true,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now,
                    Views = 300000,
                    AverageRating = 9.2,
                    RatingCount = 2500
                }
            );

            // Seed MovieGenres
            modelBuilder.Entity<MovieGenre>().HasData(
                new MovieGenre { MovieId = 1, GenreId = 7 },
                new MovieGenre { MovieId = 1, GenreId = 3 },
                new MovieGenre { MovieId = 2, GenreId = 2 },
                new MovieGenre { MovieId = 2, GenreId = 3 },
                new MovieGenre { MovieId = 3, GenreId = 7 },
                new MovieGenre { MovieId = 3, GenreId = 4 }
            );

            // Seed Episodes
            modelBuilder.Entity<Episode>().HasData(
                new Episode
                {
                    Id = 1,
                    MovieId = 2,
                    EpisodeNumber = 1,
                    Title = "Tập 1: Cuộc gặp gỡ định mệnh",
                    Description = "Yoon Se-ri vô tình hạ cánh xuống Bắc Triều Tiên sau một tai nạn.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 70,
                    ReleaseDate = new DateTime(2019, 12, 14),
                    Views = 10000,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Episode
                {
                    Id = 2,
                    MovieId = 2,
                    EpisodeNumber = 2,
                    Title = "Tập 2: Kế hoạch trốn thoát",
                    Description = "Ri Jeong-hyeok giúp Se-ri tìm cách trở về Hàn Quốc.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 65,
                    ReleaseDate = new DateTime(2019, 12, 15),
                    Views = 9500,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Episode
                {
                    Id = 3,
                    MovieId = 2,
                    EpisodeNumber = 3,
                    Title = "Tập 3: Bí mật bị hé lộ",
                    Description = "Se-ri dần thích nghi với cuộc sống ở Bắc Triều Tiên.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 68,
                    ReleaseDate = new DateTime(2019, 12, 21),
                    Views = 9000,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Episode
                {
                    Id = 4,
                    MovieId = 2,
                    EpisodeNumber = 4,
                    Title = "Tập 4: Tình cảm nảy nở",
                    Description = "Tình cảm giữa Se-ri và Jeong-hyeok bắt đầu phát triển.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 72,
                    ReleaseDate = new DateTime(2019, 12, 22),
                    Views = 8800,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Episode
                {
                    Id = 5,
                    MovieId = 2,
                    EpisodeNumber = 5,
                    Title = "Tập 5: Thách thức mới",
                    Description = "Se-ri đối mặt với những nguy hiểm mới ở Bắc Triều Tiên.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 70,
                    ReleaseDate = new DateTime(2019, 12, 28),
                    Views = 8500,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                },
                new Episode
                {
                    Id = 6,
                    MovieId = 2,
                    EpisodeNumber = 6,
                    Title = "Tập 6: Hành trình nguy hiểm",
                    Description = "Jeong-hyeok lên kế hoạch bảo vệ Se-ri.",
                    VideoPath = "/videos/1.mp4",
                    Duration = 67,
                    ReleaseDate = new DateTime(2019, 12, 29),
                    Views = 8200,
                    CreatedAt = DateTime.Now,
                    UpdatedAt = DateTime.Now
                }
            );

            // Seed Ratings
            modelBuilder.Entity<Rating>().HasData(
                new Rating
                {
                    Id = 1,
                    MovieId = 1,
                    UserName = "NguyenVanA",
                    UserEmail = "nguyenvana@example.com",
                    Score = 8,
                    Review = "Phim rất cảm động, diễn xuất của Trấn Thành tuyệt vời!",
                    CreatedAt = DateTime.Now
                },
                new Rating
                {
                    Id = 2,
                    MovieId = 1,
                    UserName = "TranThiB",
                    UserEmail = "tranthib@example.com",
                    Score = 9,
                    Review = "Cốt truyện gần gũi, phản ánh đúng cuộc sống.",
                    CreatedAt = DateTime.Now
                },
                new Rating
                {
                    Id = 3,
                    MovieId = 3,
                    UserName = "LeVanC",
                    UserEmail = "levanc@example.com",
                    Score = 10,
                    Review = "Một kiệt tác của điện ảnh Hàn Quốc!",
                    CreatedAt = DateTime.Now
                }
            );

            // Seed Comments
            modelBuilder.Entity<Comment>().HasData(
                new Comment
                {
                    Id = 1,
                    MovieId = 1,
                    EpisodeId = null,
                    UserName = "NguyenVanA",
                    UserEmail = "nguyenvana@example.com",
                    Content = "Phim này thực sự làm mình khóc rất nhiều!",
                    Likes = 50,
                    Dislikes = 2,
                    CreatedAt = DateTime.Now
                },
                new Comment
                {
                    Id = 2,
                    MovieId = 1,
                    EpisodeId = null,
                    UserName = "TranThiB",
                    UserEmail = "tranthib@example.com",
                    Content = "Cảm ơn Trấn Thành đã mang đến một bộ phim ý nghĩa!",
                    ParentCommentId = 1,
                    Likes = 30,
                    Dislikes = 1,
                    CreatedAt = DateTime.Now
                },
                new Comment
                {
                    Id = 3,
                    MovieId = 2,
                    EpisodeId = 1,
                    UserName = "LeVanC",
                    UserEmail = "levanc@example.com",
                    Content = "Tập 1 rất hấp dẫn, chemistry giữa hai diễn viên chính đỉnh cao!",
                    Likes = 40,
                    Dislikes = 0,
                    CreatedAt = DateTime.Now
                },
                new Comment
                {
                    Id = 4,
                    MovieId = 2,
                    EpisodeId = 2,
                    UserName = "PhamThiD",
                    UserEmail = "phamthid@example.com",
                    Content = "Tập 2 càng cuốn, không thể rời mắt!",
                    Likes = 35,
                    Dislikes = 1,
                    CreatedAt = DateTime.Now
                }
            );
        }
    }
}