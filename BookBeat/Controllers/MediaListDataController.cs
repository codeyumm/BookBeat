using BookBeat.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

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

    }




}





