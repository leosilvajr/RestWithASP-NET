using RestWithASPNET.Hypermedia;
using RestWithASPNET.Hypermedia.Abstract;

namespace RestWithASPNET.Data.VO
{
    public class BookVO : ISupportsHypermedia
    {
        public long Id { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public decimal Price { get; set; }

        public DateTime LaunchDaate { get; set; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>();

        public BookVO()
        {
            
        }
    }
}
