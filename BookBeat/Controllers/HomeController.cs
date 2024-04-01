using BookBeat.Models;
using BookBeat.Models.VIewModels;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace BookBeat.Controllers
{
    public class HomeController : Controller
    {
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
                Debug.WriteLine("Book response success");
                var bookJson = await bookResponse.Content.ReadAsStringAsync();
                JObject bookData = JObject.Parse(bookJson);

                // Extract book information and add to book model
                string title = bookData["title"].ToString();
                string author = bookData["author"].ToString();
                string coverUrl = bookData["cover"].ToString();

                books.Add(new Book
                {
                    Title = title,
                    Author = author,
                    CoverImageURL = coverUrl

                });
               
            }

            // set book model to viewmodel

            
            return View(tracksModel);
        }


        // GET - localhost:44387/home/tracksearch
        // to handle user search for a track
        public async Task<ActionResult> TrackSearch(string searchQuery)
        {

            // get the search string from request
            string searchString = searchQuery;

            // client object to make request
            HttpClient client = new HttpClient();

            // api url
            string url = $"https://api.deezer.com/search?q={searchString}";
            Debug.WriteLine(url);

            // empty list of track
            List<Track> trackList = new List<Track> { };

            // get response after making the request with api url
            HttpResponseMessage response = client.GetAsync(url).Result;

            // check if response was succesful or not
            if (response.IsSuccessStatusCode)
            {

                // store data as string
                var json = await response.Content.ReadAsStringAsync();

                // convert string into json
                JObject jsonData = JObject.Parse(json);

                // get data which is inside the json array
                JArray tracks = (JArray)jsonData["data"];

                foreach (var track in tracks)
                {


                    trackList.Add(new Track
                    {
                        Title = track["title"].ToString(),
                        Artist = track["artist"]["name"].ToString(),
                        AlbumArt = track["album"]["cover_big"].ToString()

                    });


                }


            }

            return View(trackList);
        }


    }
}