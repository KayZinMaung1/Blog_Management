namespace Blog.Models
{
    public class Paginator
    {
        public int PerPage { get; set; }
        public int CurrentPage { get; set; }   

        public Paginator()
        {
            this.PerPage = 2;
            this.CurrentPage = 1;
        }

        public Paginator(int per_page, int current_page)
        {
            this.PerPage = per_page>6? 6: per_page;
            this.CurrentPage = current_page< 1? 1: current_page;
        }
    }
}
