using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace MovieWebsite.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoomId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Message = table.Column<string>(type: "TEXT", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Countries",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Code = table.Column<string>(type: "TEXT", maxLength: 10, nullable: false),
                    FlagPath = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Countries", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Genres",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: true),
                    Color = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Genres", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Movies",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    EnglishTitle = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    ReleaseYear = table.Column<int>(type: "INTEGER", nullable: false),
                    CountryId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false),
                    Director = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Cast = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    PosterPath = table.Column<string>(type: "TEXT", nullable: false),
                    TrailerPath = table.Column<string>(type: "TEXT", nullable: false),
                    TotalEpisodes = table.Column<int>(type: "INTEGER", nullable: false),
                    IsCompleted = table.Column<bool>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Views = table.Column<int>(type: "INTEGER", nullable: false),
                    AverageRating = table.Column<double>(type: "REAL", nullable: false),
                    RatingCount = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Movies", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Movies_Countries_CountryId",
                        column: x => x.CountryId,
                        principalTable: "Countries",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Movies_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Episodes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeNumber = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    VideoPath = table.Column<string>(type: "TEXT", nullable: false),
                    Duration = table.Column<int>(type: "INTEGER", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Views = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Episodes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Episodes_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MovieGenres",
                columns: table => new
                {
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    GenreId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MovieGenres", x => new { x.MovieId, x.GenreId });
                    table.ForeignKey(
                        name: "FK_MovieGenres_Genres_GenreId",
                        column: x => x.GenreId,
                        principalTable: "Genres",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_MovieGenres_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Ratings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Score = table.Column<int>(type: "INTEGER", nullable: false),
                    Review = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ratings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ratings_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "WatchPartyRooms",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    InviteCode = table.Column<string>(type: "TEXT", nullable: false),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsActive = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_WatchPartyRooms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_WatchPartyRooms_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Comments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    MovieId = table.Column<int>(type: "INTEGER", nullable: false),
                    EpisodeId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UserEmail = table.Column<string>(type: "TEXT", nullable: false),
                    Content = table.Column<string>(type: "TEXT", nullable: false),
                    ParentCommentId = table.Column<int>(type: "INTEGER", nullable: true),
                    Likes = table.Column<int>(type: "INTEGER", nullable: false),
                    Dislikes = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Comments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comments_Comments_ParentCommentId",
                        column: x => x.ParentCommentId,
                        principalTable: "Comments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Comments_Episodes_EpisodeId",
                        column: x => x.EpisodeId,
                        principalTable: "Episodes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Comments_Movies_MovieId",
                        column: x => x.MovieId,
                        principalTable: "Movies",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Countries",
                columns: new[] { "Id", "Code", "FlagPath", "Name" },
                values: new object[,]
                {
                    { 1, "VN", null, "Việt Nam" },
                    { 2, "KR", null, "Hàn Quốc" },
                    { 3, "CN", null, "Trung Quốc" },
                    { 4, "JP", null, "Nhật Bản" },
                    { 5, "US", null, "Mỹ" },
                    { 6, "TH", null, "Thái Lan" }
                });

            migrationBuilder.InsertData(
                table: "Genres",
                columns: new[] { "Id", "Color", "Description", "Name" },
                values: new object[,]
                {
                    { 1, "#FF5722", null, "Hành Động" },
                    { 2, "#E91E63", null, "Tình Cảm" },
                    { 3, "#FFC107", null, "Hài Hước" },
                    { 4, "#424242", null, "Kinh Dị" },
                    { 5, "#2196F3", null, "Khoa Học Viễn Tưởng" },
                    { 6, "#4CAF50", null, "Phiêu Lưu" },
                    { 7, "#9C27B0", null, "Chính Kịch" },
                    { 8, "#FF9800", null, "Hoạt Hình" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "AverageRating", "Cast", "CountryId", "CreatedAt", "Description", "Director", "EnglishTitle", "GenreId", "IsCompleted", "PosterPath", "RatingCount", "ReleaseYear", "Title", "TotalEpisodes", "TrailerPath", "UpdatedAt", "Views" },
                values: new object[,]
                {
                    { 1, 8.5, "Trấn Thành, Tuấn Trần, Ngô Kiến Huy", 1, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9890), "Bộ phim kể về cuộc sống của một gia đình lao động nghèo ở Sài Gòn.", "Trấn Thành", "Old Father", 7, true, "/images/1.png", 1200, 2021, "Bố Già", 1, "/videos/1.mp4", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9890), 150000 },
                    { 2, 9.0, "Hyun Bin, Son Ye-jin", 2, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9895), "Câu chuyện tình yêu giữa một nữ thừa kế Hàn Quốc và một sĩ quan Bắc Triều Tiên.", "Lee Jeong-hyo", "Crash Landing on You", 2, true, "/images/2.png", 2000, 2019, "Hạ Cánh Nơi Anh", 16, "/videos/2.mp4", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9895), 250000 },
                    { 3, 9.1999999999999993, "Song Kang-ho, Lee Sun-kyun, Cho Yeo-jeong", 2, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9898), "Một gia đình nghèo tìm cách thâm nhập vào cuộc sống của một gia đình giàu có.", "Bong Joon-ho", "Parasite", 7, true, "/images/3.png", 2500, 2019, "Ký Sinh Trùng", 1, "/videos/3.mp4", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9899), 300000 }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "Dislikes", "EpisodeId", "Likes", "MovieId", "ParentCommentId", "UserEmail", "UserName" },
                values: new object[] { 1, "Phim này thực sự làm mình khóc rất nhiều!", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9987), 2, null, 50, 1, null, "nguyenvana@example.com", "NguyenVanA" });

            migrationBuilder.InsertData(
                table: "Episodes",
                columns: new[] { "Id", "CreatedAt", "Description", "Duration", "EpisodeNumber", "MovieId", "ReleaseDate", "Title", "UpdatedAt", "VideoPath", "Views" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9935), "Yoon Se-ri vô tình hạ cánh xuống Bắc Triều Tiên sau một tai nạn.", 70, 1, 2, new DateTime(2019, 12, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 1: Cuộc gặp gỡ định mệnh", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9936), "/videos/1.mp4", 10000 },
                    { 2, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9938), "Ri Jeong-hyeok giúp Se-ri tìm cách trở về Hàn Quốc.", 65, 2, 2, new DateTime(2019, 12, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 2: Kế hoạch trốn thoát", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9939), "/videos/1.mp4", 9500 },
                    { 3, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9941), "Se-ri dần thích nghi với cuộc sống ở Bắc Triều Tiên.", 68, 3, 2, new DateTime(2019, 12, 21, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 3: Bí mật bị hé lộ", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9941), "/videos/1.mp4", 9000 },
                    { 4, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9943), "Tình cảm giữa Se-ri và Jeong-hyeok bắt đầu phát triển.", 72, 4, 2, new DateTime(2019, 12, 22, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 4: Tình cảm nảy nở", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9944), "/videos/1.mp4", 8800 },
                    { 5, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9946), "Se-ri đối mặt với những nguy hiểm mới ở Bắc Triều Tiên.", 70, 5, 2, new DateTime(2019, 12, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 5: Thách thức mới", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9946), "/videos/1.mp4", 8500 },
                    { 6, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9948), "Jeong-hyeok lên kế hoạch bảo vệ Se-ri.", 67, 6, 2, new DateTime(2019, 12, 29, 0, 0, 0, 0, DateTimeKind.Unspecified), "Tập 6: Hành trình nguy hiểm", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9949), "/videos/1.mp4", 8200 }
                });

            migrationBuilder.InsertData(
                table: "MovieGenres",
                columns: new[] { "GenreId", "MovieId" },
                values: new object[,]
                {
                    { 3, 1 },
                    { 7, 1 },
                    { 2, 2 },
                    { 3, 2 },
                    { 4, 3 },
                    { 7, 3 }
                });

            migrationBuilder.InsertData(
                table: "Ratings",
                columns: new[] { "Id", "CreatedAt", "MovieId", "Review", "Score", "UserEmail", "UserName" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9966), 1, "Phim rất cảm động, diễn xuất của Trấn Thành tuyệt vời!", 8, "nguyenvana@example.com", "NguyenVanA" },
                    { 2, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9968), 1, "Cốt truyện gần gũi, phản ánh đúng cuộc sống.", 9, "tranthib@example.com", "TranThiB" },
                    { 3, new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9969), 3, "Một kiệt tác của điện ảnh Hàn Quốc!", 10, "levanc@example.com", "LeVanC" }
                });

            migrationBuilder.InsertData(
                table: "Comments",
                columns: new[] { "Id", "Content", "CreatedAt", "Dislikes", "EpisodeId", "Likes", "MovieId", "ParentCommentId", "UserEmail", "UserName" },
                values: new object[,]
                {
                    { 2, "Cảm ơn Trấn Thành đã mang đến một bộ phim ý nghĩa!", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9990), 1, null, 30, 1, 1, "tranthib@example.com", "TranThiB" },
                    { 3, "Tập 1 rất hấp dẫn, chemistry giữa hai diễn viên chính đỉnh cao!", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9992), 0, 1, 40, 2, null, "levanc@example.com", "LeVanC" },
                    { 4, "Tập 2 càng cuốn, không thể rời mắt!", new DateTime(2025, 6, 18, 22, 37, 41, 334, DateTimeKind.Local).AddTicks(9994), 1, 2, 35, 2, null, "phamthid@example.com", "PhamThiD" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comments_EpisodeId",
                table: "Comments",
                column: "EpisodeId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_MovieId",
                table: "Comments",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_Comments_ParentCommentId",
                table: "Comments",
                column: "ParentCommentId");

            migrationBuilder.CreateIndex(
                name: "IX_Episodes_MovieId_EpisodeNumber",
                table: "Episodes",
                columns: new[] { "MovieId", "EpisodeNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MovieGenres_GenreId",
                table: "MovieGenres",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_CountryId",
                table: "Movies",
                column: "CountryId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_GenreId",
                table: "Movies",
                column: "GenreId");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_ReleaseYear",
                table: "Movies",
                column: "ReleaseYear");

            migrationBuilder.CreateIndex(
                name: "IX_Movies_Title",
                table: "Movies",
                column: "Title");

            migrationBuilder.CreateIndex(
                name: "IX_Ratings_MovieId",
                table: "Ratings",
                column: "MovieId");

            migrationBuilder.CreateIndex(
                name: "IX_WatchPartyRooms_MovieId",
                table: "WatchPartyRooms",
                column: "MovieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "Comments");

            migrationBuilder.DropTable(
                name: "MovieGenres");

            migrationBuilder.DropTable(
                name: "Ratings");

            migrationBuilder.DropTable(
                name: "WatchPartyRooms");

            migrationBuilder.DropTable(
                name: "Episodes");

            migrationBuilder.DropTable(
                name: "Movies");

            migrationBuilder.DropTable(
                name: "Countries");

            migrationBuilder.DropTable(
                name: "Genres");
        }
    }
}
