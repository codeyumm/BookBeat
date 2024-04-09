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
            Debug.WriteLine("--- removing track from list ----");

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
        [Route("api/MediaListData/GetListenLaterListOfTracks/{userId}")]

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

        /// <summary>
        /// Returns a list of books in the listen later list of the user.
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All books in the user's listen later list 
        /// </returns>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/MediaListData/GetListenLaterListOfBooks/18
        /// 
        /// Response: [{"Id":5,"Title":"Hello World","Username":"Saloni","UserId":18,"BookId":5,"Author":"Saloni Pawar"},
        ///            {"Id":6,"Title":"Book Title 2".....]
        /// </example>

        [HttpGet]
        [ResponseType(typeof(MediaList))]
        [Route("api/MediaListData/GetListenLaterListOfBooks/{userId}")]
        public IHttpActionResult GetListenLaterListOfBooks(string userId)
        {
            // Check if model state is valid or not
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If data is valid, get the listen later list of books for the user
            List<MediaList> bookList = db.MediaLists.Where(bl => bl.UserID == userId)
                                                   .Where(bl => bl.IsAddedLater == true)
                                                   .Where(media => media.MediaType == "book")
                                                   .Where(bl => bl.IsAlreadyHeardOrRead == false)
                                                   .ToList();

            // Create a list to store book DTOs
            List<BookListDTO> listenLaterList = new List<BookListDTO>();

            // Iterate through each object of bookList
            foreach (var book in bookList)
            {
                // Create a book DTO object and add it to listenLaterList
                listenLaterList.Add(new BookListDTO
                {
                    Id = (int)book.BookID,
                    Title = book.Books.Title,
                    UserName = book.User.UserName,
                    Author = book.Books.Author,
                    UserID = book.User.Id,
                    BookID = book.Books.BookID,
                    Genre = book.Books.Genre,
                    CoverImage = book.Books.CoverImageURL
                });
            }

            return Ok(listenLaterList);
        }


        // ---------------------------------------------------------------------------
        //                          API FOR DISCOVERD LIST
        // ---------------------------------------------------------------------------

        /// <summary>
        /// Adds a song to the discovered list of the user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @discoverd.json https://localhost:44127/api/tracklistdatax/AddToDiscoveredList
        /// response: Ok
        /// </example>

        // Add song to discovered list
        // We need trackId and UserId
        // Before adding, we have to check if the song and user exist or not,
        // and if the user already has that song in the discovered list
        // If the song exists and is not in the user's discovered list, add the song to the list.
        
        [HttpPost]
        [Route("api/MediaListData/AddTrackToDiscoveredList/{userId}/{trackId}")]
        public IHttpActionResult AddTrackToDiscoveredList(string userId, int trackId)
        {
            // Check if the track with trackId from the request exists or not
            bool isTrackExist = db.Tracks.Any(t => t.Id == trackId);

            // Check if the track is in the user's discovered list
            bool isInDiscoveredList = db.MediaLists.Any(track => track.TrackID == trackId && track.UserID == userId && track.IsAlreadyHeardOrRead);

            // Check if the track is in the user's listen later list
            bool isInListenLaterList = db.MediaLists.Any(track => track.TrackID == trackId && track.UserID == userId && track.IsAddedLater);

            Debug.WriteLine($"User wants to add track {trackId},\nIs it in the discovered list? {isInDiscoveredList},\nIs it in the listen later list? {isInListenLaterList}");

            if (isTrackExist)
            {
                Debug.WriteLine("Given track is in the Track database");

                if (isInListenLaterList)
                {
                    // If the track is in the listen later list, update its status to discovered
                    MediaList MediaList = db.MediaLists.FirstOrDefault(user => user.UserID == userId && user.TrackID == trackId);
                    MediaList.IsAddedLater = false;
                    MediaList.IsAlreadyHeardOrRead = true;
                    db.SaveChanges();

                    return Ok("Song added to discovered list");
                }
                else if (isInDiscoveredList)
                {
                    // If the track is already in the discovered list, return an error message
                    return Ok("Song is already in discovered list");
                }
                else
                {
                    // If the track is not in either discovered or listen later list, add it to the discovered list
                    MediaList MediaList = new MediaList
                    {
                        TrackID = trackId,
                        UserID = userId,
                        IsAddedLater = false,
                        IsAlreadyHeardOrRead = true,
                        MediaType = "track"
                    };

                    db.MediaLists.Add(MediaList);
                    db.SaveChanges();

                    return Ok("Song added to discovered list");
                }
            }
            else
            {
                Debug.WriteLine("There is an error");

                return BadRequest("There was an error");
            }
        }

       


        /// <summary>
        /// Adds a book to the discovered list of the user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @discoverBook.json https://localhost:44127/api/booklist/AddToDiscoveredList
        /// response: Ok
        /// </example>

        // Add book to discovered list
        // We need bookId and UserId
        // Before adding, we have to check if the book and user exist or not,
        // and if the user already has that book in the discovered list
        // If the book exists and is not in the user's discovered list, add the book to the list.

        [HttpPost]
        [Route("api/BookList/AddBookToDiscoveredList/{userId}/{bookId}")]
        public IHttpActionResult AddBookToDiscoveredList(string userId, int bookId)
        {
            // Check if the book with bookId from the request exists or not
            bool isBookExist = db.Books.Any(b => b.BookID == bookId);

            // Check if the book is in the user's discovered list
            bool isInDiscoveredList = db.MediaLists.Any(book => book.BookID == bookId && book.UserID == userId && book.IsAlreadyHeardOrRead);

            // Check if the book is in the user's read later list
            bool isInReadLaterList = db.MediaLists.Any(book => book.BookID == bookId && book.UserID == userId && book.IsAddedLater);

            Debug.WriteLine($"User wants to add book {bookId},\nIs it in the discovered list? {isInDiscoveredList},\nIs it in the read later list? {isInReadLaterList}");

            if (isBookExist)
            {
                Debug.WriteLine("Given book is in the Book database");

                if (isInReadLaterList)
                {
                    // If the book is in the read later list, update its status to discovered
                    MediaList mediaList = db.MediaLists.FirstOrDefault(user => user.UserID == userId && user.BookID == bookId);
                    mediaList.IsAddedLater = false;
                    mediaList.IsAlreadyHeardOrRead = true;
                    db.SaveChanges();

                    return Ok("Book added to discovered list");
                }
                else if (isInDiscoveredList)
                {
                    // If the book is already in the discovered list, return an error message
                    return Ok("Book is already in discovered list");
                }
                else
                {
                    // If the book is not in either discovered or read later list, add it to the discovered list
                    MediaList mediaList = new MediaList
                    {
                        BookID = bookId,
                        UserID = userId,
                        IsAddedLater = false,
                        IsAlreadyHeardOrRead = true,
                        MediaType = "book"
                    };

                    db.MediaLists.Add(mediaList);
                    db.SaveChanges();

                    return Ok("Book added to discovered list");
                }
            }
            else
            {
                Debug.WriteLine("There is an error");

                return BadRequest("There was an error");
            }
        }

        /// <summary>
        /// Remove a song from the discovered list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44127/api/TrackListData/removeFromDiscovered/1/4
        /// response: User 1 removed track with id 4 from his list
        /// </example>

        // Remove song from discovered list
        // Check if the song exists in the discovered list for a particular user or not. If it exists, proceed further.
        // Otherwise, send an error message.
        // If the request is valid, remove the song from the discovered list.

        [HttpPost]
        [Route("api/MediaListData/RemoveTrackFromDiscovered/{userId}/{trackId}")]
        public IHttpActionResult RemoveTrackFromDiscovered(string userId, int trackId)
        {
            Debug.WriteLine("--- Removing from list ----");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // QUERY -> Find a record which has userId, trackId and it should have discovered == true
            // SingleOrDefault - either return one element or default value if the result is null
            // Here we are sure that we will receive either one row or null, so we can use SingleOrDefault

            MediaList mediaList = db.MediaLists.Where(user => user.UserID == userId)
                                               .Where(track => track.TrackID == trackId)
                                               .Where(media => media.MediaType == "track")
                                               .Where(discovered => discovered.IsAlreadyHeardOrRead)
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

            return Ok($"User {userId} removed track with id {trackId} from his list");
        }


        /// <summary>
        /// Remove a book from discovered list of user
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44127/api/BookListData/removeFromDiscovered/1/4
        /// response: User 1 removed book with id 4 from his list
        /// </example>

        // Remove book from discovered list
        // Check if the book exists in the discovered list for a particular user or not. If it exists, proceed further.
        // Otherwise, send an error message.
        // If the request is valid, remove the book from the discovered list.

        [HttpPost]
        [Route("api/MediaListData/RemoveBookFromDiscovered/{userId}/{bookId}")]
        public IHttpActionResult RemoveBookFromDiscovered(string userId, int bookId)
        {
            Debug.WriteLine("--- Removing track from  discoverd list ----");

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // QUERY -> Find a record which has userId, bookId and it should have discovered == true
            // SingleOrDefault - either return one element or default value if the result is null
            // Here we are sure that we will receive either one row or null, so we can use SingleOrDefault

            MediaList mediaList = db.MediaLists.Where(user => user.UserID == userId)
                                               .Where(book => book.BookID == bookId)
                                               .Where(media => media.MediaType == "book")
                                               .Where(discovered => discovered.IsAlreadyHeardOrRead == true)
                                               .SingleOrDefault();

            if (mediaList != null)
            {
                Debug.WriteLine("Removing from list got the list");

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
        /// Returns a list of songs in the discovered list of the user.
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All songs in the user's discovered list 
        /// </returns>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/MediaListData/GetDiscoveredListOfTracks/18
        /// 
        /// Response: [{"Id":5,"Title":"Maut Ka Safar","Username":"PunkRebel","UserId":18,"TrackId":5,"Artist":"Seedhe Maut"},
        ///            {"Id":6,"Title":"Gravity Waves".....]
        /// </example>

        [HttpGet]
        [ResponseType(typeof(MediaList))]
        [Route("api/MediaListData/GetDiscoveredListOfTracks/{userId}")]
        public IHttpActionResult GetDiscoveredListOfTracks(string userId)
        {
            // Check if model state is valid or not
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If data is valid, get the discovered list of songs for the user
            List<MediaList> trackList = db.MediaLists.Where(tl => tl.UserID == userId)
                                                     .Where(tl => tl.IsAlreadyHeardOrRead == true)
                                                     .Where(media => media.MediaType == "track")
                                                     .ToList();

            // Create a list to store song DTOs
            List<TrackListDTO> discoveredList = new List<TrackListDTO>();

            // Iterate through each object of trackList
            foreach (var track in trackList)
            {
                // Create a song DTO object and add it to discoveredList
                discoveredList.Add(new TrackListDTO
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

            return Ok(discoveredList);
        }

        /// <summary>
        /// Returns a list of books in the discovered list of the user.
        /// </summary>
        /// 
        /// <returns>
        /// HEADER: 200 (OK)
        /// CONTENT: All books in the user's discovered list 
        /// </returns>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/MediaListData/GetDiscoveredListOfBooks/18
        /// 
        /// Response: [{"Id":5,"Title":"Hello World","Username":"Rajat Dalal","UserId":18,"BookId":5,"Author":"Shwetabh"},
        ///            {"Id":6,"Title":"Book Title 2".....]
        /// </example>

        [HttpGet]
        [ResponseType(typeof(MediaList))]
        [Route("api/MediaListData/GetDiscoveredListOfBooks/{userId}")]
        public IHttpActionResult GetDiscoveredListOfBooks(string userId)
        {
            // Check if model state is valid or not
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // If data is valid, get the discovered list of books for the user
            List<MediaList> bookList = db.MediaLists.Where(bl => bl.UserID == userId)
                                                   .Where(bl => bl.IsAlreadyHeardOrRead == true)
                                                   .Where(media => media.MediaType == "book")
                                                   .ToList();

            // Create a list to store book DTOs
            List<BookListDTO> discoveredList = new List<BookListDTO>();

            // Iterate through each object of bookList
            foreach (var book in bookList)
            {
                // Create a book DTO object and add it to discoveredList
                discoveredList.Add(new BookListDTO
                {
                    Id = (int)book.BookID,
                    Title = book.Books.Title,
                    UserName = book.User.UserName,
                    Author = book.Books.Author,
                    UserID = book.User.Id,
                    BookID = book.Books.BookID,
                    Genre = book.Books.Genre,
                    CoverImage = book.Books.CoverImageURL
                });
            }

            return Ok(discoveredList);
        }










    }








}





