namespace Blog.Models
{
    public class PagedListServer<T>
    {
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }
        public IEnumerable<T> Results { get; set; }

    }
}
