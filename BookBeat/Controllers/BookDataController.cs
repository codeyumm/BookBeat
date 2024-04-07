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
    public class BookDataController : ApiController
    {
        /// get database context/
        public ApplicationDbContext db = new ApplicationDbContext();

        /// Add Book
        [HttpPost]
        [ResponseType(typeof(Book))]
        [Route("api/BookData/AddBook")]


        public IHttpActionResult AddBook(Book book)
        {
            // if model is not valid
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Check if a book with the same details already exists
            var existingBook = db.Books.FirstOrDefault(b =>
                b.Title == book.Title &&
                b.Author == book.Author &&
                b.Genre == book.Genre &&
                b.PublicationYear == book.PublicationYear &&
                b.ISBN == book.ISBN &&
                b.Description == book.Description &&
                b.CoverImageURL == book.CoverImageURL);


            if (existingBook != null)
            {
                return Ok(existingBook.BookID);
            }

            if (book.BookID != 0 && db.Books.Any(b => b.BookID == book.BookID))
            {
                Debug.WriteLine("******" + book.BookID);
                return Ok(book.BookID);
            }

            // Book doesn't exist, add it to the database
            db.Books.Add(book);
            db.SaveChanges();

            Debug.WriteLine("Book added with id " + book.BookID);

            return Ok(book.BookID);

        }


        // Update Book

        /// <summary>
        /// Updates a book in database
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl -H "Content-Type:application/json" -d @bookUpdate.json https://localhost:44387/api/BookData/UpdateBook/1
        /// Response: Ok
        /// </example>
        /// 


        [HttpPost]
        [Route("api/BookData/UpdateBook/{id}")]

        public IHttpActionResult UpdateBook(int id, Book book)
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

            // check if id from get request and id of book is same or not

            if (id != book.BookID)
            {

                Debug.WriteLine("ID mismatche between passed id and passed book id");
                return BadRequest();
            }

            // if everything is valid
            // update book to database
            // Entry() return entity of book
            // then changing the sate of that book to "Modified"

            db.Entry(book).State = EntityState.Modified;

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


        // Delete Book
        /// <summary>
        /// Deletes a book in database
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK)
        /// </returns>
        /// 
        /// <example>
        /// POST: curl  -d "" https://localhost:44387/api/BookData/DeletBook/1
        /// Response: Ok
        /// </example>
        /// 

        [HttpPost]
        [Route("api/BookData/DeleteBook/{id}")]
        public IHttpActionResult DeleteBook(int id)
        {

            // get book of given id
            Book book = db.Books.Find(id);

            // check if given id's book exist or not
            if (book == null)
            {
                Debug.WriteLine("------ Book doesn't exist ------");

                return NotFound();
            }

            // remove book from database
            db.Books.Remove(book);
            db.SaveChanges();

            return Ok();

        }


        /// <summary>
        /// Returns all books in database
        /// </summary>
        /// 
        /// <returns>
        /// HTTP 200 (OK) with the following content:
        /// - A list of books
        /// </returns>
        /// 
        /// <example>
        /// GET: https://localhost:44387/api/BookData/ListBooks
        /// Response: [{"Id":1,"Title":"Sinister Flows"},
        ///             {"Id":2,"Title":"Gravitational Pull"...]
        /// </example>

        /// Display Books

        [HttpGet]
        [Route("api/BookData/ListBooks")]
        public List<BookDTO> ListBooks()
        {

            List<Book> Books = db.Books.ToList();

            List<BookDTO> BookDtos = new List<BookDTO>();
            Books.ForEach(book =>
            {

                BookDtos.Add(new BookDTO()
                {
                    BookID = book.BookID,
                    Title = book.Title,

                }); ;
            });

            return BookDtos;
        }
    }
}
