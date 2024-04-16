using BookBeat.Models;
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
    public class BookController : Controller
    {
        private static readonly HttpClient client;
        private JavaScriptSerializer jss = new JavaScriptSerializer();
        private ApplicationDbContext db = new ApplicationDbContext();

        static BookController()
        {
            client = new HttpClient();
            client.BaseAddress = new Uri("https://localhost:44366/api/bookdata/");
        }

        //Objective: A webpage that list the books in our system
        // GET: Book/List
        public ActionResult List()
        {
            //objective: communicate with our book data api to retrieve a list of books
            //curl https://localhost:44324/api/bookdata/listbooks


            string url = "https://localhost:44366/api/bookdata/listbooks";
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            IEnumerable<BookDTO> books = response.Content.ReadAsAsync<IEnumerable<BookDTO>>().Result;
            //Debug.WriteLine("Number of books received : ");
            //Debug.WriteLine(books.Count());


            return View(books);
        }


        // GET: Book/ListUserBooks/?userId=1
        //Objective: A webpage that list the books in our system with a specific user id
        public ActionResult ListUserBooks(int userId)
        {
            // Retrieve the user ID

            ViewBag.UserId = userId;
            Debug.WriteLine(userId);
            //GET {resource}/api/bookdata/listbooks
            //https://localhost:44366/api/bookdata/listbooks/{userId}
            //Use HTTP client to access information

            HttpClient client = new HttpClient();
            //set the url
            string url = $"https://localhost:44366/api/bookdata/listbooks/{userId}";
            HttpResponseMessage response = client.GetAsync(url).Result;

            IEnumerable<BookDTO> books = response.Content.ReadAsAsync<IEnumerable<BookDTO>>().Result;

            return View(books);
        }

        // GET: Book/Details/5
        public ActionResult Details(int id)
        {
            //objective: communicate with our book data api to retrieve one book
            //curl https://localhost:44324/api/bookdata/findbook/{id}

            string url = "findbook/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            // Debug.WriteLine("The response code is ");
            // Debug.WriteLine(response.StatusCode);

            BookDTO selectedbook = response.Content.ReadAsAsync<BookDTO>().Result;
            // Debug.WriteLine("book received : ");




            return View(selectedbook);
        }

        public ActionResult Error()
        {

            return View();
        }

        // GET: Book/New
        public ActionResult New(int UserID)
        {
            // Retrieve list of genres from your data source
           // List<Genre> genres = db.Genres.ToList();

            // Pass the list of genres to the view
        //    ViewBag.Genres = genres;
          //  ViewBag.UserID = UserID;

            return View();
        }

        // POST: Book/Create
        [HttpPost]
        public ActionResult Create(Book book)
        {
            // Debug.WriteLine("the json payload is :");
            //Debug.WriteLine(book.BookName);
            //objective: add a new book into our system using the API
            //curl -H "Content-Type:application/json" -d @book.json https://localhost:44324/api/bookdata/addbook
            string url = "addbook";


            string jsonpayload = jss.Serialize(book);

            Debug.WriteLine(jsonpayload);


            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            HttpResponseMessage response = client.PostAsync(url, content).Result;
            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction("List");
            }
            else
            {
                Debug.WriteLine("---");

                return RedirectToAction("Error");
            }

        }

        // GET: Book/Edit/5
        public ActionResult Edit(int id)
        {
            //grab the book information
            ViewBag.UserId = id;
            //objective: communicate with our book data api to retrieve one book
            //curl https://localhost:44324/api/bookdata/findbook/{id}

            string url = "findbook/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            //Debug.WriteLine("The response code is ");
            //Debug.WriteLine(response.StatusCode);

            BookDTO selectedbook = response.Content.ReadAsAsync<BookDTO>().Result;
            //Debug.WriteLine("book received : ");
            //Debug.WriteLine(selectedbook.BookName);

            return View(selectedbook);
        }
        // POST: Book/Update/5
        [HttpPost]
        public ActionResult Update(int id, Book book)
        {

            //Debug.WriteLine("Genre ID received: " + genreId);
            Debug.WriteLine(book.Title);
            Debug.WriteLine("The new book info is-----------------------------:");

            Debug.WriteLine(book.Author);
            //Debug.WriteLine(genreId);

            //book.GenreID = genreId;

            //serialize into JSON
            //Send the request to the API

            string url = "updatebook/" + id;

            string jsonpayload = jss.Serialize(book);
            Debug.WriteLine(jsonpayload);




            HttpContent content = new StringContent(jsonpayload);
            content.Headers.ContentType.MediaType = "application/json";

            //POST: api/BookData/UpdateBook/{id}
            //Header : Content-Type: application/json
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("okay-:");

                return RedirectToAction("Details/" + id);
            }
            else
            {
                Debug.WriteLine("not okay-:");

                return RedirectToAction("Details/" + id);
            }


        }

        // GET: Book/Delete/5
        public ActionResult Delete(int id, string BookName)
        {
            ViewBag.id = id;
            ViewBag.BookName = BookName;

            return View();
        }

        // GET: Book/Delete/5
        public ActionResult ConfirmDelete(int id)
        {
            Debug.WriteLine("---- " + id);
            // call api


            // Call the API to confirm the delete action
            string url = "deletebook/" + id;


            HttpContent content = new StringContent("");
            content.Headers.ContentType.MediaType = "application/json";
            HttpResponseMessage response = client.PostAsync(url, content).Result;

            if (response.IsSuccessStatusCode)
            {
                Debug.WriteLine("Book deleted successfully.");
            }
            else
            {
                Debug.WriteLine("Failed to delete the book.");
            }

            return RedirectToAction("List");
        }

        public ActionResult BooksForGenre(int id)
        {
            // Retrieve books for the specified genre ID
            string url = "https://localhost:44366/api/genredata/booksforgenre/" + id;
            HttpResponseMessage response = client.GetAsync(url).Result;

            if (response.IsSuccessStatusCode)
            {
                IEnumerable<BookDTO> books = response.Content.ReadAsAsync<IEnumerable<BookDTO>>().Result;
                ViewBag.GenreId = id;
                return View(books);
            }
            else
            {
                // Handle error response
                Debug.WriteLine("Failed to retrieve books for genre with ID: " + id);
                return RedirectToAction("Error");
            }
        }

    }
}
