using BookBeat.Models;
using BookBeat.Models.ViewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;


namespace BookBeat.Controllers
{
    public class HomeController : Controller
    {

        private static readonly HttpClient client;

        static HomeController()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AllowAutoRedirect = false,
                UseCookies = false
            };

            client = new HttpClient(handler);
            client.BaseAddress = new Uri("https://localhost:44324/api/");

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



        public ActionResult Index()
        {
            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        // Home page
        // TODO
        // add api for book too

        // GET - Home/Search
        public async Task<ActionResult> Search()
        {


            GetApplicationCookie();

            var userId = User.Identity.GetUserId();

            // get owin context to use apis to get info about users
            var userManager = HttpContext.GetOwinContext().GetUserManager<ApplicationUserManager>();

            // get info of current user
            var currentUser = await userManager.FindByIdAsync(userId);


            Debug.WriteLine("user id --- " + userId);


            // get viewmodel
            HomeMedia ViewModel = new HomeMedia();

            HttpClient client = new HttpClient();
            string musicUrl = "https://api.deezer.com/chart/0/tracks";
            string bookUrl = "https://books-api7.p.rapidapi.com/books/get/random/";

            // Fetch music data
            HttpResponseMessage musicResponse = await client.GetAsync(musicUrl);
            List<Track> tracksModel = new List<Track>();

            if (musicResponse.IsSuccessStatusCode)
            {
                Debug.WriteLine("Music response success");
                var musicJson = await musicResponse.Content.ReadAsStringAsync();
                JObject musicData = JObject.Parse(musicJson);
                JArray musicTracks = (JArray)musicData["data"];

                foreach (var track in musicTracks)
                {
                    tracksModel.Add(new Track
                    {
                        Title = track["title"].ToString(),
                        Artist = track["artist"]["name"].ToString(),
                        AlbumArt = track["album"]["cover_big"].ToString()
                    });
                }
            }

            ViewModel.tracks = tracksModel;

            // Fetch book data

            // empty book list
            List<Book> books = new List<Book>();

            // Fetch 10 random books
            for (int i = 0; i < 10; i++)
            {
                HttpRequestMessage bookRequest = new HttpRequestMessage
                {
                    Method = HttpMethod.Get,
                    RequestUri = new Uri(bookUrl),
                    Headers =
                {
                    { "X-RapidAPI-Key", "92cd5fcfa4mshd97d398e5b8c2d3p1b650cjsn6c18a16505b8" },
                    { "X-RapidAPI-Host", "books-api7.p.rapidapi.com" },
                },
                
                };

                HttpResponseMessage bookResponse = await client.SendAsync(bookRequest);

                if (bookResponse.IsSuccessStatusCode)
                {
                    Debug.WriteLine($"Book response success {i + 1}");

                    var bookJson = await bookResponse.Content.ReadAsStringAsync();
                    JObject bookData = JObject.Parse(bookJson);

                    // Extract book information and add to book model
                    string title = bookData["title"].ToString();
                    string author = bookData["author"]["first_name"].ToString() + " " + bookData["author"]["last_name"].ToString();
                    string coverUrl = bookData["cover"].ToString();

                    books.Add(new Book
                    {
                        Title = title,
                        Author = author,
                        CoverImageURL = coverUrl
                    });
                }
            }

            // set book model to viewmodel
            ViewModel.books = books;


            
            return View(ViewModel);
        }


        // GET - localhost:44387/home/tracksearch
        // to handle user search for a track

        public async Task<ActionResult> MediaSearch(string searchQuery)
        {
            string searchString = searchQuery;

 
            HttpClient client = new HttpClient();

            // API URL for music search
            string musicUrl = $"https://api.deezer.com/search?q={searchString}";

            // API URL for Google Books search
            string bookUrl = $"https://www.googleapis.com/books/v1/volumes?q={HttpUtility.UrlEncode(searchString)}";

            
            List<Track> tracks = new List<Track>();

          
            List<Book> books = new List<Book>();

            
            HttpResponseMessage musicResponse = await client.GetAsync(musicUrl);

            if (musicResponse.IsSuccessStatusCode)
            {
                var musicJson = await musicResponse.Content.ReadAsStringAsync();
                JObject musicData = JObject.Parse(musicJson);
                JArray musicTracks = (JArray)musicData["data"];

                // Iterate over music tracks and add to list
                foreach (var trackItem in musicTracks)
                {
                    string title = trackItem["title"].ToString();
                    string artist = trackItem["artist"]["name"].ToString();
                    string albumArt = trackItem["album"]["cover_big"].ToString();

                    tracks.Add(new Track
                    {
                        Title = title,
                        Artist = artist,
                        AlbumArt = albumArt
                    });
                }
            }

            // Fetch books from Google Books API
            HttpResponseMessage bookResponse = await client.GetAsync(bookUrl);

            if (bookResponse.IsSuccessStatusCode)
            {
                var bookJson = await bookResponse.Content.ReadAsStringAsync();
                JObject bookData = JObject.Parse(bookJson);

                JArray items = (JArray)bookData["items"];

               
                foreach (var item in items)
                {
                    var volumeInfo = item["volumeInfo"];
                    string title = volumeInfo["title"].ToString();

                    // api is givng array of authors converting array of object into string of array
                    string[] authorArray = volumeInfo["authors"]?.ToObject<string[]>();
                    // if array is not null join author array by ","

                    string authors = authorArray != null ? string.Join(", ", authorArray) : "";


                    string coverUrl = volumeInfo["imageLinks"]?["thumbnail"]?.ToString();

                    books.Add(new Book
                    {
                        Title = title,
                        Author = authors,
                        CoverImageURL = coverUrl
                    });
                }
            }

            // Create ViewModel with tracks and books
            HomeMedia viewModel = new HomeMedia
            {
                tracks = tracks,
                books = books
            };

            return View(viewModel);
        }




    }
}