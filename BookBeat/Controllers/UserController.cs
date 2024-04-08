using BookBeat.Models;
using BookBeat.Models.ViewModels;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace BookBeat.Controllers
{
    public class UserController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();

        static UserController()
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


        // GET: User
        public ActionResult Index()
        {
            return View();
        }


        // GET: User/DisplayProfile/id

        public ActionResult DisplayProfile()
        {
            // In profile we need data from 3 models.
            // 1. User
            // 2. MediaList with that we can access tracks and books
            // 3. Review
            // Using view models for this
            DetailsUser ViewModel = new DetailsUser();

            // Get user ID
            string id = User.Identity.GetUserId();

            // Client object for HttpClient to use http methods
            HttpClient client = new HttpClient();

            // For user info
            string userUrl = $"https://localhost:44366/api/UserData/GetProfileInfo/{id}";
            HttpResponseMessage userResponse = client.GetAsync(userUrl).Result;
          

            // Store the response in UserDTO object
            UserDTO user = userResponse.Content.ReadAsAsync<UserDTO>().Result;

            // Set user to view model
            ViewModel.SelectedUser = user;

            // For listen later list
            string listenLaterUrl = $"https://localhost:44366/api/MediaListData/GetListenLaterListOfTracks/{id}";
            HttpResponseMessage listenLaterResponse = client.GetAsync(listenLaterUrl).Result;
            if (!listenLaterResponse.IsSuccessStatusCode)
            {
                // Handle error when listen later list retrieval fails
                // For example, set listen later list to an empty list
                ViewModel.UserListenLaterList = new List<TrackListDTO>();
            }
            else
            {
                // Store response in tracklist object
                IEnumerable<TrackListDTO> listenLaterList = listenLaterResponse.Content.ReadAsAsync<IEnumerable<TrackListDTO>>().Result;
                ViewModel.UserListenLaterList = listenLaterList;
            }

            // For discovered list
            string discoveredUrl = $"https://localhost:44366/api/MediaListData/GetDiscoveredListOfTracks/{id}";
            HttpResponseMessage discoveredResponse = client.GetAsync(discoveredUrl).Result;
            if (!discoveredResponse.IsSuccessStatusCode)
            {
                // Handle error when discovered list retrieval fails
                // For example, set discovered list to an empty list
                ViewModel.UserDiscoverdListOfTrack = new List<TrackListDTO>();
            }
            else
            {
                // Store response in tracklist object
                IEnumerable<TrackListDTO> discoveredList = discoveredResponse.Content.ReadAsAsync<IEnumerable<TrackListDTO>>().Result;
                ViewModel.UserDiscoverdListOfTrack = discoveredList;
            }

            // Pass ViewModel to view
            return View(ViewModel);
        }

    }
}