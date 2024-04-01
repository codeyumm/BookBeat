using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using BookBeat.Models;
using System.Diagnostics;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Web;

namespace BookBeat.Controllers
{
    public class TrackDataController : ApiController
    {
        // get database context
        public ApplicationDbContext db = new ApplicationDbContext();

        // Add Track
        [HttpPost]
        [ResponseType(typeof(Track))]
        [Route("api/TrackData/AddTrack")]


        public IHttpActionResult AddTrack(Track track)
        {
            // if model is not valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a track with the same details already exists
            var existingTrack = db.Tracks.FirstOrDefault(t =>
                t.Title == track.Title &&
                t.Album == track.Album &&
                t.AlbumArt == track.AlbumArt &&
                t.Artist == track.Artist);

            if (existingTrack != null)
            {
                return Ok(existingTrack.Id);
            }

            if (track.Id != 0 && db.Tracks.Any(t => t.Id == track.Id))
            {
                Debug.WriteLine("******" + track.Id);
                return Ok(track.Id);
            }

            // Track doesn't exist, add it to the database
            db.Tracks.Add(track);
            db.SaveChanges();

            Debug.WriteLine("Track added with id " + track.Id);

            return Ok(track.Id);

        }


        // Update Track

        /// <summary>
        /// Updates a track in database
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @trackUpdate.json https://localhost:44387/api/TrackData/UpdateTrack/1
        /// Response: Ok
        /// </example>
        /// 


        [HttpPost]
        [Route("api/TrackData/UpdateTrack/{id}")]

        public IHttpActionResult UpdateTrack(int id, Track track)
        {
            // Model state contains all value from post request
            // and it kind of assign those value with model
            // and if there are any invalid value we can throw some message on webpage
            // Model state is kindof server side validation

            Debug.WriteLine("-----------IN update method --------------------");


            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // check if id from get request and id of track is same or not

            if (id != track.Id)
            {

                Debug.WriteLine("ID mismatche between passed id and passed track id");
                return BadRequest();
            }

            // if everything is valid
            // update track to database
            // Entry() return entity of track
            // then changing the sate of that track to "Modified"

            db.Entry(track).State = EntityState.Modified;

            // save to data and check if there are any error while saving to database

            try
            {
                db.SaveChanges();
            }
            catch (DbUpdateConcurrencyException)
            {

                throw;

            }

            Debug.WriteLine("Passed try catch block");

            return StatusCode(HttpStatusCode.NoContent);

        }


        // Delete Track
        /// <summary>
        /// Deletes a track in database
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44387/api/TrackData/DeletTrack/1
        /// Response: Ok
        /// </example>
        /// 

        [HttpPost]
        [Route("api/TrackData/DeleteTrack/{id}")]
        public IHttpActionResult DeleteTrack(int id)
        {

            // get track of given id
            Track track = db.Tracks.Find(id);

            // check if given id's track exist or not
            if (track == null)
            {
                Debug.WriteLine("------ Track doesn't exist ------");

                return NotFound();
            }

            // remove track from database
            db.Tracks.Remove(track);
            db.SaveChanges();

            return Ok();

        }




    }
}
