using System.Text;

namespace RestWithASPNET.Hypermedia
{
    public class HyperMediaLink
    {
        public string Rel { get; set; }

        private string href;
        public string Href {
            get
            {
                object _look = new object();
                lock (_look) 
                {
                    StringBuilder sb = new StringBuilder(href);
                    return sb.Replace("%2F", "/").ToString();
                }

            }
            set { 
                href = value; 
            } 
        } // / localhost:8080%2fap

        public string Type { get; set; }
        public string Action { get; set; }
    }
}
