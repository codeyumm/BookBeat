using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BookBeat.Models
{
    public class Track
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Album { get; set; }

        public string AlbumArt { get; set; }

        public DateTime? ReleaseDate { get; set; } = new DateTime(2024, 12, 12);

        // Reviews can have many tracks
        // public ICollection<TrackList> TrackLists { get; set; }

        // TrackList can have many tracks
        // TODO
    }

    // DTO Object to abstract some data
    public class TrackDTO
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Artist { get; set; }

        public string Album { get; set;}
    }


}