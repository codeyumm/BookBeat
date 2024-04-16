using BookBeat.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using System.Web.Http;


namespace BookBeat.Controllers
{
    public class UserDataController : ApiController
    {
        /// get database context/
        public ApplicationDbContext db = new ApplicationDbContext();

        /// <summary>
        ///  Returns information about user including username, email and name
        /// </summary>
        /// <param name="id">User id</param>
        /// <returns>UserDTO object with info</returns>
        [HttpGet]
        [Route("api/UserData/GetProfileInfo/{id}")] // here id is user id

        public IHttpActionResult GetProfileInfo(string id)
        {
            // Check if user ID is null or empty
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("User ID is null or empty");
            }

            // Check if user exists
            ApplicationUser user = db.Users.Find(id);
            if (user == null)
            {
                return NotFound(); // User not found
            }

            // Populate UserDTO with user data
            UserDTO userData = new UserDTO
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                UserName = user.UserName
                // Map other properties as needed
            };

            // Return user data
            return Ok(userData);
        }



        // <summary>
        /// Returns list of user who have song in thier listen later list
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK) with the following content:
        /// - list of user
        /// </returns>
        /// 
        /// <param name="id">id of track</param>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/UserData/findIntrestedUserForListenLater/12
        /// 
        /// Response:[{"Id":10,"Title":"Rap Resurrection","Username":"PunkRebel","UserId":18,"TrackId":12,"Artist":"Raftaar"},
        /// {"Id":30,"Title":"Rap Resurrection","Username":"RockNRoller","UserId":3,"TrackId":12,"Artist":"Raftaar"}]
        /// </example>


        [HttpGet]
        [Route("api/UserData/findIntrestedUserForListenLater/{id}")]

        public IHttpActionResult findIntrestedUserForListenLater(int id)
        {
            // check if track with given id exist or not

            Track track = db.Tracks.Find(id);

            // send badrequst if track is not found
            if (track == null)
            {
                return BadRequest("track is not in database");
            }

            // find all users who have given track in thier listen later list
            List<MediaList> tracklist = db.MediaLists.Where(t => t.TrackID == id)
                                                  .Where(listenlater => listenlater.IsAddedLater == true)
                                                  .ToList();

            List<TrackListDTO> tracklistDto = new List<TrackListDTO> { };


            // iterate through each tracklist row and set it to tracklistdto value
            // append new tracklistdto object in tracklistdto list
            foreach (var tl in tracklist)
            {

                tracklistDto.Add(new TrackListDTO
                {
                    Id = tl.ID,
                    Title = tl.Tracks.Title,
                    UserName = tl.User.UserName,
                    UserID = tl.UserID,
                    TrackID = (int)tl.TrackID,
                    Artist = tl.Tracks.Artist
                });
            }


            return Ok(tracklistDto);
        }


        /// <summary>
        /// Returns list of user who have song in thier discoverd list
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK) with the following content:
        /// - list of user
        /// </returns>
        /// 
        /// <param name="id">id of track</param>
        /// <param name="userId">id of user</param>
        /// <example>
        /// GET: https://localhost:44387/api/UserData/findIntrestedUserForDiscoverd/6/18
        /// 
        /// Response:[{"Id":16,"Title":"Anti-Gravity Anthem","Username":"MetalHead","UserId":12,"TrackId":18,"Artist":"SUPERVILLAIN"},{"Id":39,"Title":"Anti-Gravity Anthem","Username":"EDMAddict","UserId":7,"TrackId":18,"Artist":"SUPERVILLAIN"}]
        /// </example>


        [HttpGet]
        [Route("api/UserData/findIntrestedUserForDiscoverdTrack/{userId}/{trackId}")]

        public IHttpActionResult findIntrestedUserForDiscoverdTrack(string userId, int trackId)
        {
            // check if track with given id exist or not

            Track track = db.Tracks.Find(trackId);

            // send badrequst if track is not found
            if (track == null)
            {
                return BadRequest("track is not in database");
            }

            // find all users who have given track in thier listen later list
            List<MediaList> tracklist = db.MediaLists.Where(t => t.TrackID == trackId)
                                                  .Where(discoverd => discoverd.IsAlreadyHeardOrRead == true)
                                                  .ToList();

            List<TrackListDTO> tracklistDto = new List<TrackListDTO> { };


            // iterate through each tracklist row and set it to tracklistdto value
            // append new tracklistdto object in tracklistdto list
            foreach (var tl in tracklist)
            {

                if (tl.UserID == userId) { continue; }

                tracklistDto.Add(new TrackListDTO
                {
                    Id = tl.ID,
                    Title = tl.Tracks.Title,
                    UserName = tl.User.UserName,
                    UserID = tl.UserID,
                    TrackID = (int)tl.TrackID,
                    Artist = tl.Tracks.Artist

                });
            }

            return Ok(tracklistDto);
        }


        // add above two methods for book



        // to delete user

        [HttpPost]
        [Route("api/UserData/Remove/{id}")]

        public IHttpActionResult Remove(string id)
        {

            // check if user exist or not
            ApplicationUser user = db.Users.Find(id);

            if (user != null)
            {
                // remove user from database
                db.Users.Remove(user);
                db.SaveChanges();

                return Ok("User deleted from database.");
            }
            else
            {
                return BadRequest("User not found.");
            }

        }


        // get all usernames from database
        // get user info from username
        [HttpGet]
        [Route("api/UserData/getUsernames")] // here id is user id

        public IHttpActionResult getUsernames()
        {

            // check if user exist or not
            // get all info from users table except password

            List<ApplicationUser> user = db.Users.ToList();

            List<UserDTO> userDto = new List<UserDTO> { };



            if (user != null)
            {

                foreach (var u in user)
                {
                    userDto.Add(new UserDTO
                    {
                        // set value in userDto from user object

                        UserName = u.UserName,
                        Email = u.Email,
                        FirstName = u.FirstName,
                        LastName = u.LastName
                    });
                }



                // user exist and return userDto object as a response
                return Ok(userDto);
            }
            else
            {
                return BadRequest("No user found in database");
            }


        }


    }
}
