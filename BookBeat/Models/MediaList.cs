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

            public int MediaID { get; set; } 

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
