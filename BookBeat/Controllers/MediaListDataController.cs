using BookBeat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace BookBeat.Controllers
{
    public class MediaListDataController : ApiController
    {

        /// get database context/
        public ApplicationDbContext db = new ApplicationDbContext();


        /// <summary>
        /// Adds a song to listen later list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @listenLater.json https://localhost:44127/api/tracklistdatax/AddToListenLater
        /// response: Ok
        /// </example>


        // add song to listen later list
        // we will need trackId, UserId
        // before addig we have to check that
        // if song and user exist or not and if user already has that song in listen later list
        // send some error message . .
        // else
        // add that song to listen later list

        [HttpPost]
        [Route("api/MediaListData/AddTrackToListenLaterList/{userId}/{trackId}")]

        public IHttpActionResult AddTrackToListenLaterList(string userId, int trackId)
        {

            Debug.WriteLine(" got user id : " + userId);
            Debug.WriteLine(" got track id : " + trackId);


            // check track with trackId from request exist or not
            // get track with trackId
            // if the query returns 1 track exist if it returns 0 track doesn't exist
            bool isTrackExist = (db.Tracks.Where(t => t.Id == trackId).Count() == 1) ? true : false;

            // check if user already have the trak in thier listen later list or not
            bool isAlreadyAdded = (db.MediaLists.Where(track => track.TrackID == trackId).
                                                Where(user => user.UserID == userId).Count() == 1) ? true : false;


            // check if user already have the trak in thier discoverd list or not
            // turnary operator returns true if record exists or false
            bool isInDiscoverdList = (db.MediaLists.Where(track => track.TrackID == trackId)
                                                .Where(user => user.UserID == userId)
                                                .Where(dList => dList.IsAlreadyHeardOrRead == true).Count() == 1) ? true : false;

            // if track exist and it is not in user's listen later list add that song to list
            // else send an error message

            Debug.WriteLine(" is track exist : " + isTrackExist);
            Debug.WriteLine(" is track added : " + isAlreadyAdded);


            // at this point there are four possiblity
            // 1. track doesnt exist in database -> send message to user
            // 2. track exist -> track is already in listen later list -> send message to user
            // 3. track exist -> track is in discoverd list -> update track status in tracklist to listen later = 1 and discoverd = 0
            // 4. track exist -> track is not in both list -> add tracklist to tracklists database with discoverd = 0 and listen later = 1

            if (isTrackExist)
            {
                Debug.WriteLine("Given track is in database");

                // when i'ts a new track entry in list
                if (!isAlreadyAdded && !isInDiscoverdList)
                {
                    MediaList medialist = new MediaList();
                    medialist.UserID = userId;
                    medialist.TrackID = trackId;
                    medialist.IsAlreadyHeardOrRead = false;
                    medialist.IsAddedLater = true;
                    medialist.MediaType = "track";

                    db.MediaLists.Add(medialist);
                    db.SaveChanges();

                    Debug.WriteLine("Tried to adding in database");
                    return Ok("Track Added to listen later list of user");


                }
                else if (isInDiscoverdList) // if track is in discoverd list
                {
                    // get the track and change the value of discoverd and listen later
                    MediaList medialist = db.MediaLists.Where(user => user.UserID == userId)
                                                        .Where(track => track.TrackID == trackId).SingleOrDefault();

                    medialist.IsAlreadyHeardOrRead = false;
                    medialist.IsAddedLater = true;

                    // update in database
                    db.Entry(medialist).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok("Track Added to listen later list of user");

                }
                else if (isAlreadyAdded)
                {
                    return BadRequest("Track is already in listen later list");
                }

                return BadRequest("There was some error");
            }
            else
            {
                Debug.Write("There was some error");
                return BadRequest("There was some error");
            }


        }


        /// <summary>
        /// Adds a book to listen later list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <param name="bookId">ID OF BOOK</param>
        /// <param name="userId">ID OF USER from asp net user table</param>
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @readLater.json https://api/MediaListData/AddBookToListenLaterList/{userId}/{bookId}
        /// response: Ok
        /// </example> 



        [HttpPost]
        [Route("api/MediaListData/AddBookToReadLaterList/{userId}/{bookId}")]
        public IHttpActionResult AddBookToReadLaterList(string userId, int bookId)
        {
            Debug.WriteLine("Got user id: " + userId);
            Debug.WriteLine("Got book id: " + bookId);

            // Check if the book with bookId exists
            bool isBookExist = (db.Books.Where(b => b.BookID == bookId).Count() == 1);

            // Check if the user already has the book in their listen later list
            bool isAlreadyAdded = (db.MediaLists.Where(b => b.BookID == bookId)
                                                .Where(u => u.UserID == userId)
                                                .Count() == 1);

            // Check if the user already has the book in their discovered list
            bool isInDiscoveredList = (db.MediaLists.Where(b => b.BookID == bookId)
                                                     .Where(u => u.UserID == userId)
                                                     .Where(m => m.IsAlreadyHeardOrRead)
                                                     .Count() == 1);

            if (isBookExist)
            {
                Debug.WriteLine("Given book exists in the database");

                if (!isAlreadyAdded && !isInDiscoveredList) // Book is not in listen later or discovered list
                {
                    MediaList mediaList = new MediaList
                    {
                        UserID = userId,
                        BookID = bookId,
                        IsAlreadyHeardOrRead = false,
                        IsAddedLater = true,
                        MediaType = "book"
                    };

                    db.MediaLists.Add(mediaList);
                    db.SaveChanges();

                    return Ok("Book added to listen later list of the user");
                }
                else if (isInDiscoveredList) // Book is in discovered list
                {
                    MediaList mediaList = db.MediaLists.Where(b => b.UserID == userId)
                                                       .Where(b => b.BookID == bookId)
                                                       .SingleOrDefault();

                    mediaList.IsAlreadyHeardOrRead = false;
                    mediaList.IsAddedLater = true;

                    db.Entry(mediaList).State = EntityState.Modified;
                    db.SaveChanges();

                    return Ok("Book added to listen later list of the user");
                }
                else if (isAlreadyAdded) // Book is already in listen later list
                {
                    return BadRequest("Book is already in the listen later list");
                }
            }
            else
            {
                Debug.Write("The book does not exist in the database");
                return BadRequest("The book does not exist in the database");
            }

            Debug.Write("There was some error");
            return BadRequest("There was some error");
        }


        /// <summary>
        /// Remove a song to listen later list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44127/api/TrackListData/removeFromListenLater/1/4
        /// response: User 1 removed trak with id 4 from his list
        /// </example>
        /// 


        // remove song from listen later list
        // check if song exist in listen later list for a particular user or  not if exist move further
        // else send an error message
        // if req is valid remove song from listen later list

        [HttpPost]
        [Route("api/MediaListData/RemoveTrackFromListenLater/{userId}/{trackId}")]

        public IHttpActionResult RemoveTrackFromListenLater(string userId, int trackId)
        {
            Debug.WriteLine("--- removing from list ----");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // QUERY -> Find a record which has userId, trackId and it should have listen later == 1
            // SingleOrDefault - either return one element or default vaue if result is null
            // here we are sure that we will recieve either one row or null so we can use SingleOrDefault


            MediaList medialist = db.MediaLists.Where(user => user.UserID == userId)
                                                .Where(track => track.TrackID == trackId)
                                                .Where(media => media.MediaType == "track")
                                                .Where(listenLater => listenLater.IsAddedLater == true).SingleOrDefault();



            if (medialist != null)
            {

                Debug.WriteLine("remove from list");

                // remove from list
                db.MediaLists.Remove(medialist);
                db.SaveChanges();

            }
            else
            {
                // send error message
                Debug.WriteLine("can't remove from list");

                return BadRequest("Can't remove from list");

            }

            return Ok($" User {userId} removed track with id {trackId}  his list ");
        }


        /// <summary>
        /// Remove a book from listen later list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44127/api/BookListData/removeFromListenLater/1/4
        /// response: User 1 removed book with id 4 from his list
        /// </example>

        // Remove book from listen later list
        // Check if the book exists in the listen later list for a particular user or not. If it exists, proceed further.
        // Otherwise, send an error message.
        // If the request is valid, remove the book from the listen later list.

        [HttpPost]
        [Route("api/MediaListData/RemoveBookFromReadLater/{userId}/{bookId}")]
        public IHttpActionResult RemoveBookFromReadLater(string userId, int bookId)
        {
            Debug.WriteLine("--- Removing from list ----");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // QUERY -> Find a record which has userId, bookId and it should have listen later == 1
            // SingleOrDefault - either return one element or default value if the result is null
            // Here we are sure that we will receive either one row or null, so we can use SingleOrDefault

            MediaList mediaList = db.MediaLists.Where(user => user.UserID == userId)
                                               .Where(book => book.BookID == bookId)
                                               .Where(media => media.MediaType == "book")
                                               .Where(listenLater => listenLater.IsAddedLater == true)
                                               .SingleOrDefault();

            if (mediaList != null)
            {
                Debug.WriteLine("Removing from list");

                // Remove from list
                db.MediaLists.Remove(mediaList);
                db.SaveChanges();
            }
            else
            {
                // Send error message
                Debug.WriteLine("Can't remove from list");
                return BadRequest("Can't remove from list");
            }

            return Ok($"User {userId} removed book with id {bookId} from his list");
        }

        /// <summary>
        /// Returns all a list of songs in listen later list
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: all songs in user's listen later list 
        /// </returns>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/TrackListData/GetListenLaterList/18
        /// 
        /// response: [{"Id":5,"Title":"Maut Ka Safar","Username":"PunkRebel","UserId":18,"TrackId":5,"Artist":"Seedhe Maut"},
        ///             {"Id":6,"Title":"Gravity Waves".....]
        /// </example>

        [HttpGet]
        [ResponseType(typeof(MediaList))]
        [Route("api/TrackListData/GetListenLaterList/{userId}")]

        public IHttpActionResult GetListenLaterList(string userId)
        {

            // check if model state is valid or not
            if (!ModelState.IsValid)
            {

                return BadRequest(ModelState);

            }

            // if data is valid
            // get listen later list of user accordin to userId and listen later should be true and discovered should be false
            List<MediaList> tracklist = db.MediaLists.Where(tl => tl.UserID == userId)
                                                    .Where(tl => tl.IsAddedLater == true)
                                                    .Where( media =>  media.MediaType == "track")
                                                    .Where(tl => tl.IsAlreadyHeardOrRead == false)
                                                    .ToList();


            // if we send tracklist it will send lot of data, which is not useful to user
            // to reduce the load using dto
            // create tracklist dto object
            List<TrackListDTO> listenLaterList = new List<TrackListDTO> { };



            // iterate through each object of tracklist
            foreach (var track in tracklist)
            {

                // get the value according to dto
                // append object to listenlaterlist
                listenLaterList.Add(new TrackListDTO
                {
                    Id = (int)track.TrackID,
                    Title = track.Tracks.Title,
                    UserName = track.User.UserName,
                    Artist = track.Tracks.Artist,
                    UserID = track.User.Id,
                    TrackID = track.Tracks.Id,
                    Album = track.Tracks.Album,
                    AlbumArt = track.Tracks.AlbumArt
                });
            }

            return Ok(listenLaterList);
        }



    }




}





