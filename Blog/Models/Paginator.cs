namespace Blog.Models
{
    public class Paginator
    {
        public int per_page { get; set; }
        public int current_page { get; set; }   

        public Paginator()
        {
            this.per_page = 2;
            this.current_page = 1;
        }

        public Paginator(int per_page, int current_page)
        {
            this.per_page = per_page>6? 6: per_page;
            this.current_page = current_page< 1? 1: current_page;
        }
    }
}
