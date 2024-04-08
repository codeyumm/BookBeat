using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace BookBeat.Models.ViewModels
{
    public class DetailsUser
    {
        // In profile we need data from 3 models.
        // 1. User
        // 2. MediaList
        // 3. Review
        // Using view models for this


        public UserDTO SelectedUser { get; set; }

        public IEnumerable<TrackListDTO> UserListenLaterList { get; set; }

        public IEnumerable<TrackListDTO> UserDiscoverdListOfTrack { get; set; }

        public IEnumerable<BookListDTO> UserReadLaterList { get; set; }

        public IEnumerable<BookListDTO> UserDiscoverdListOfBook { get; set; }


        // public IEnumerable<ReviewDto> UserReviews { get; set; }

    }
}