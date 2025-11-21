namespace Mini_Social_Media.DTO {
    public class ApiResponse<T> {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public T? Data { get; set; }
    }

    public class PagedResponse<T> {
        public List<T> Items { get; set; } = new();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalItems { get; set; }
        public int TotalPages { get; set; }
    }
}
