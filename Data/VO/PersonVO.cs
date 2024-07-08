using RestWithASPNET.Hypermedia;
using RestWithASPNET.Hypermedia.Abstract;

namespace RestWithASPNETUdemy.Data.VO
{

    public class PersonVO : ISupportsHypermedia
    {
        //Serialização dos VO, isso vai mudar a forma que o JSON do objeto sera criado.
        //Esse codigo ficara comentado.

        //[JsonPropertyName("code")]
        public long Id { get; set; }

        //[JsonPropertyName("name")]
        public string FirstName { get; set; }

        //[JsonPropertyName("last_name")]
        public string LastName { get; set; }

        //[JsonIgnore]
        public string Address { get; set; }

        //[JsonPropertyName("sex")]
        public string Gender { get; set; }

        public bool Enabled { get; set; }
        public List<HyperMediaLink> Links { get; set; } = new List<HyperMediaLink>(); 
            public PersonVO() { }//Ja suporta Hypermedia, quando for entra no ContentResponseEnricher ele ja sabe que o tipo suporta Enricher
    }
}
