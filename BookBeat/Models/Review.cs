using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBeat.Models
{
    public class Review
    {
        [Key]
        public int Id { get; set; }

        // userid which is foregin key from aspnetuser table
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

        [ForeignKey("Books")]
        public int? BookID { get; set; }
        public virtual Book Books { get; set; }

        [ForeignKey("Tracks")]
        public int? TrackID { get; set; }
        public virtual Track Tracks { get; set; }

        public string Title { get; set; }

        public string MediaType { get; set; } 

        public string Content { get; set; } 

        public int Rating { get; set; } 
 

    }




}