using System.ComponentModel.DataAnnotations.Schema;

namespace RestWithASPNET.Model
{
    [Table("books")]
    public class Book
    {
        [Column("id")]
        public long Id { get; set; }

        [Column("author")]
        public string Author { get; set; }

        [Column("launch_date")]
        public DateTime LaunchDaate { get; set; }

        [Column("price")]
        public decimal Price { get; set; }

        [Column("title")]
        public string Title { get; set; }

    }
}

//Cria a Model com os Atributos
//Cria a Repository com as Interfaces
//Cria o Business
//Cria a Controller