using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Services.Background {
    public class StoryArchiverService : BackgroundService {
        // Vì BackgroundService là Singleton, mà DbContext là Scoped
        // Nên ta phải dùng IServiceProvider để tạo scope
        private readonly IServiceProvider _serviceProvider;

        public StoryArchiverService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                await ArchiveExpiredStories();

                // Nghỉ 10 phút rồi quét tiếp
                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task ArchiveExpiredStories() {
            using (var scope = _serviceProvider.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                // 1. Tìm các Story đã hết hạn (Quá 24h)
                var expiredStories = await context.Stories
                    .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                    .ToListAsync();

                if (expiredStories.Any()) {
                    var archives = new List<StoryArchive>();

                    // 2. Copy sang bảng Archive
                    foreach (var story in expiredStories) {
                        archives.Add(new StoryArchive {
                            UserId = story.UserId,
                            MediaUrl = story.MediaUrl,
                            MediaType = story.MediaType,
                            Caption = story.Caption,
                            CreatedAt = story.CreatedAt,
                            ViewCount = story.ViewCount,
                            ArchivedAt = DateTime.UtcNow
                        });
                    }

                    // 3. Lưu Archive và Xóa Story gốc
                    await context.StoryArchives.AddRangeAsync(archives);
                    context.Stories.RemoveRange(expiredStories);

                    // 4. Commit 1 lần (Transaction)
                    await context.SaveChangesAsync();

                    Console.WriteLine($"[System] Archived {expiredStories.Count} stories.");
                }
            }
        }
    }
}