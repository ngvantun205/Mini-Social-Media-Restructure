using Microsoft.EntityFrameworkCore;

namespace Mini_Social_Media.Services.Background {
    public class StoryArchiverService : BackgroundService {
        private readonly IServiceProvider _serviceProvider;

        public StoryArchiverService(IServiceProvider serviceProvider) {
            _serviceProvider = serviceProvider;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken) {
            while (!stoppingToken.IsCancellationRequested) {
                await ArchiveExpiredStories();

                await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
            }
        }

        private async Task ArchiveExpiredStories() {
            using (var scope = _serviceProvider.CreateScope()) {
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

                var expiredStories = await context.Stories
                    .Where(s => s.ExpiresAt <= DateTime.UtcNow)
                    .ToListAsync();

                if (expiredStories.Any()) {
                    var archives = new List<StoryArchive>();

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

                    await context.StoryArchives.AddRangeAsync(archives);
                    context.Stories.RemoveRange(expiredStories);

                    await context.SaveChangesAsync();

                    Console.WriteLine($"[System] Archived {expiredStories.Count} stories.");
                }
            }
        }
    }
}