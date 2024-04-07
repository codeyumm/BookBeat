using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace BookBeat.Models
{
    public class Book
    {
        [Key]
        public int BookID { get; set; }

        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        // ? to make nullable field for int
        public int? PublicationYear { get; set; }

        public string ISBN { get; set; }

        public string Description { get; set; }

        public string CoverImageURL { get; set; }

        // Review can have many book
        // TODO

        // MediaList can have many book
        // TODO
    }

    // DTO object to abstract some data
    public class BookDTO
    {
        public int BookID { get; set;}

        public string Title { get; set; }

        public string Author { get; set; }

        public string Genre { get; set; }

        public int? PublicationYear { get; set; }

        public string ISBN { get; set;}

        public string Description { get; set; }

        public string CoverImageURL { get; set; }

    
    }
}
