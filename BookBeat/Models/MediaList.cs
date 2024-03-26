using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBeat.Models
{
    public class MediaList
    {

        [Key]
        public int ID { get; set; }

        // using nullable int here to unassociate book or track

        
        [ForeignKey("Books")]
        public int? BookID { get; set; } 
        public virtual Book Books { get; set; }

        [ForeignKey("Tracks")]
        public int? TrackID { get; set; }
        public virtual Track Tracks { get; set; }

        // will have either book or song
        public string MediaType { get; set; } 

        // for listen later list
        public bool IsAddedLater { get; set; } 

        // for already discoverd/explored list
        public bool IsAlreadyHeardOrRead { get; set; }

        // userid which is foregin key from aspnetuser table
        [ForeignKey("User")]
        public string UserID { get; set; }
        public virtual ApplicationUser User { get; set; }

           
    }
}
