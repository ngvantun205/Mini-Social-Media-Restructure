namespace Mini_Social_Media.AppService {
    public class ReportService : IReportService {
        private readonly IReportRepository _reportRepository;
        public ReportService(IReportRepository reportRepository) {
            _reportRepository = reportRepository;
        }
        public async Task<ReportViewModel> AddReport(ReportInputModel inputModel, int userId) {
            if (inputModel == null)
                return new ReportViewModel();
            var report = new Report() {
                Content = inputModel.Content,
                EntityId = inputModel.EntityId,
                Type = inputModel.Type,
                CreatedAt = DateTime.UtcNow,
                ReporterId = userId,
            };
            await _reportRepository.AddAsync(report);
            return new ReportViewModel() {
            Content= inputModel.Content,
            EntityId = inputModel.EntityId,
            Type = inputModel.Type,
            CreatedAt = DateTime.UtcNow,
            ReporterId = userId,
            };
        }
    }
}
