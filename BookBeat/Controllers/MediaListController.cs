using BookBeat.Models;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Threading.Tasks;
using System.Web.Http.Tracing;
using BookBeat.Models.ViewModels;

namespace BookBeat.Controllers
{
    public class MediaListController : Controller
    {

        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static MediaListController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44366/api/");

        }

        private void GetApplicationCookie()
        {
            string token = "";
            //HTTP client is set up to be reused, otherwise it will exhaust server resources.
            //This is a bit dangerous because a previously authenticated cookie could be cached for
            //a follow-up request from someone else. Reset cookies in HTTP client before grabbing a new one.
            client.DefaultRequestHeaders.Remove("Cookie");
            if (!User.Identity.IsAuthenticated) return;

            HttpCookie cookie = System.Web.HttpContext.Current.Request.Cookies.Get(".AspNet.ApplicationCookie");
            if (cookie != null) token = cookie.Value;

            //collect token as it is submitted to the controller
            //use it to pass along to the WebAPI.
            Debug.WriteLine("Token Submitted is : " + token);
            if (token != "") client.DefaultRequestHeaders.Add("Cookie", ".AspNet.ApplicationCookie=" + token);

            return;
        }


        // GET: MediaList
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public ActionResult AddToLaterList(int mediaId, string mediaType, Book book, Track track)
        {
            
            HttpClientHandler handler = new HttpClientHandler();
            
        
            GetApplicationCookie();

            var userId = User.Identity.GetUserId();

            string jsonPayload = "";
            string url = "";
            bool isTrack = false;

            if (mediaType == "track")
            {
                
                jsonPayload = jss.Serialize(track);
                url = "trackdata/addtrack";
                isTrack = true;

            }
            else
            {
               
                jsonPayload = jss.Serialize(book);
                url = "bookdata/addbook";

            }

           
            HttpContent content = new StringContent(jsonPayload);
            content.Headers.ContentType.MediaType = "application/json";

            // send api request to add the track or book
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            string id = response.Content.ReadAsStringAsync().Result;

            mediaId = Int32.Parse(id);

            if (response.IsSuccessStatusCode)
            {
                // if adding the track or book was successful, proceed to add it to the listen later or read later list
                if (isTrack)
                {
                    // set the url to add the track to the listen later list
                    url = $"MediaListData/AddTrackToListenLaterList/{userId}/{mediaId}";
                }
                else
                {
                    // set the url to add the book to the read later list
                    url = $"MediaListData/AddBookToReadLaterList/{userId}/{mediaId}";
                }


                HttpContent contentTwo = new StringContent(jsonPayload);
                response = client.PostAsync(url, contentTwo).Result;


                if (response.IsSuccessStatusCode)
                {
                    
                    return RedirectToAction("ListenLater");
                }
                else
                {
                    
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }


        [HttpPost]
        public ActionResult AddToDiscoveredList(int mediaId, string mediaType, Book book, Track track)
        {
            // get user id
            var userId = User.Identity.GetUserId();

           
            string jsonPayload = "";
            string url = "";
            bool isTrack = false;

            if (mediaType == "track")
            {
                jsonPayload = jss.Serialize(track);
                url = "trackdata/addtrack";
                isTrack = true;
            }
            else
            {
                jsonPayload = jss.Serialize(book);
                url = "bookdata/addbook";
            }

          
            HttpContent content = new StringContent(jsonPayload);
            content.Headers.ContentType.MediaType = "application/json";

           
            HttpResponseMessage response = client.PostAsync(url, content).Result;
            string id = response.Content.ReadAsStringAsync().Result;
            

            mediaId = Int32.Parse(id);

       
            if (response.IsSuccessStatusCode)
            {
                
                if (isTrack)
                {
                    url = $"MediaListData/AddTrackToDiscoveredList/{userId}/{mediaId}";
                }
                else
                {
                    url = $"MediaListData/AddBookToReadLaterList/{userId}/{mediaId}";
                }

                HttpContent contentTwo = new StringContent(jsonPayload);
                response = client.PostAsync(url, contentTwo).Result;


                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction("DiscoveredList");
                }
                else
                {
                    return View("Error");
                }
            }
            else
            {
                return View("Error");
            }
        }


        // GET: Media/ListenLater
        [HttpGet]
        public ActionResult ListenLater()
        {
            LaterList laterList = new LaterList();

            // get user id
            var userId = User.Identity.GetUserId();

            // get music tracks for listen later list
            string url = $"MediaListData/GetListenLaterListOfTracks/{userId}";

            HttpResponseMessage musicResponse = client.GetAsync(url).Result;
            Debug.WriteLine(url);

            if (musicResponse.IsSuccessStatusCode)
            {
                Debug.WriteLine("Hello");
                laterList.Tracks = musicResponse.Content.ReadAsAsync<List<TrackListDTO>>().Result;
            }
            

            // get books for read later list
            HttpResponseMessage bookResponse = client.GetAsync($"MediaListData/GetListenLaterList/{userId}").Result;
            if (bookResponse.IsSuccessStatusCode)
            {
                laterList.Books = bookResponse.Content.ReadAsAsync<List<BookListDTO>>().Result;
            }
            

            return View(laterList);
        }

        // GET: Media/Discovered
        [HttpGet]
        public ActionResult DiscoveredList()
        {
            LaterList laterList = new LaterList();

            // Get user id
            var userId = User.Identity.GetUserId();

            // Get discovered music tracks
            string musicUrl = $"MediaListData/GetDiscoveredListOfTracks/{userId}";
            HttpResponseMessage musicResponse = client.GetAsync(musicUrl).Result;

            if (musicResponse.IsSuccessStatusCode)
            {
                laterList.Tracks = musicResponse.Content.ReadAsAsync<List<TrackListDTO>>().Result;
            }

            // Get discovered books
            string bookUrl = $"MediaListData/GetDiscoveredListOfBooks/{userId}";
            HttpResponseMessage bookResponse = client.GetAsync(bookUrl).Result;

            if (bookResponse.IsSuccessStatusCode)
            {
                laterList.Books = bookResponse.Content.ReadAsAsync<List<BookListDTO>>().Result;
            }

            return View(laterList);
        }




        // to remove media from listne later list
        [HttpPost]
        public ActionResult RemoveFromLaterList(int mediaId, string mediaType)
        {
            HttpClientHandler handler = new HttpClientHandler();

            GetApplicationCookie();

            var userId = User.Identity.GetUserId();

            string url = "";
         

            if (mediaType == "track")
            {
                url = $"MediaListData/RemoveTrackFromListenLater/{userId}/{mediaId}";
              
            }
            else
            {
                url = $"MediaListData/RemoveBookFromReadLater/{userId}/{mediaId}";
            }

            
            HttpResponseMessage response = client.DeleteAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
              
                return RedirectToAction("ListenLater");
            }
            else
            {
                
                return View("Error");
            }
        }





    }
}