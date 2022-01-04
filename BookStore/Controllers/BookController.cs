using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BookStore.Models;
using BookStore.Models.Repositorios;
using BookStore.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.IO;

namespace BookStore.Controllers
{
    public class BookController : Controller
    {
        private readonly IBookstoreRepository<Book> bookRepository;
        private readonly IBookstoreRepository<Author> authorRepository;
        private readonly IHostingEnvironment hosting;

        public BookController(IBookstoreRepository<Book> bookRepository, IBookstoreRepository<Author> authorRepository, IHostingEnvironment hosting)
        {
            this.bookRepository = bookRepository;
            this.authorRepository = authorRepository;
            this.hosting = hosting;
        }

        // GET: BookController
        public ActionResult Index()
        {
            var books = bookRepository.List();
            return View(books);
        }

        // GET: BookController/Details/5
        public ActionResult Details(int id)
        {
            var book = bookRepository.Find(id);
            return View(book);
        }

        // GET: BookController/Create
        public ActionResult Create()
        {
            return View(GetAllAuthors());
        }

        // POST: BookController/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(BookAuthorViewModel model)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    string FileName = UploadFile(model.File) ?? string.Empty;

                    

                    if (model.AuthorId == -1)
                    {
                        ViewBag.Message = "Please select an author!";

                        return View(GetAllAuthors());
                    }

                    var author = authorRepository.Find(model.AuthorId);
                    var book = new Book
                    {
                        Id = model.BookId,
                        Title = model.Title,
                        Description = model.Desription,
                        Author = author,
                        ImageUrl = FileName
                    };

                    bookRepository.Add(book);
                    return RedirectToAction(nameof(Index));
                }
                catch
                {
                    return View();
                }
            }
            else
            {
                ModelState.AddModelError("", "Please Fill all fields");
                return View(GetAllAuthors());
            }
        }

        // GET: BookController/Edit/5
        public ActionResult Edit(int id)
        {
            var book = bookRepository.Find(id);
            var authorId = book.Author == null ? book.Author.Id = 0 : book.Author.Id;

            var viewModel = new BookAuthorViewModel
            {
                BookId = book.Id,
                Title = book.Title,
                Desription = book.Description,
                AuthorId = book.Author.Id,
                Authors = authorRepository.List().ToList(),
                ImageUrl = book.ImageUrl
            };

            return View(viewModel);
        }

        // POST: BookController/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(BookAuthorViewModel model)
        {
            try
            {
                string FileName = UploadFile(model.File, model.ImageUrl);


                var author = authorRepository.Find(model.AuthorId);
                var book = new Book
                {
                    Id = model.BookId,
                    Title = model.Title,
                    Description = model.Desription,
                    Author = author,
                    ImageUrl = FileName
                };

                bookRepository.Update(model.BookId, book);
                return RedirectToAction(nameof(Index));
            }
            catch(Exception ex)
            {
                return View();
            }
        }

        // GET: BookController/Delete/5
        public ActionResult Delete(int id)
        {
            var book = bookRepository.Find(id);

            return View(book);
        }

        // POST: BookController/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Delete(int id, Book book)
        {
            try
            {
                bookRepository.Delete(id);
                return RedirectToAction(nameof(Index));
            }
            catch
            {
                return View();
            }
        }

        //Get Author List
        List<Author> FillSelectList()
        {
            var author = authorRepository.List().ToList();
            author.Insert(0, new Author { Id = -1, FullName = "--- Select an Author ---" });
            return author;
        }

        //Fill the list of authors
        BookAuthorViewModel GetAllAuthors()
        {
            var model = new BookAuthorViewModel
            {
                Authors = FillSelectList()
            };

            return model;
        }

        string UploadFile(IFormFile file)
        {
            if (file != null)
            {
                string upload = Path.Combine(hosting.WebRootPath, "Uploads");
                string FullPath = Path.Combine(upload, file.FileName);
                file.CopyTo(new FileStream(FullPath, FileMode.Create));
                return file.FileName;
            }

            return null;
        }

        string UploadFile(IFormFile file, string imageURL)
        {
            if (file != null)
            {
                string upload = Path.Combine(hosting.WebRootPath, "Uploads");
                
                string newPath = Path.Combine(upload, file.FileName);
                string fullOldPath = Path.Combine(upload, imageURL);


                //Save new image
                file.CopyTo(new FileStream(newPath, FileMode.Create));
                return file.FileName;
            }

            return imageURL;
        }

        public ActionResult Search(string term)
        {
            var result = bookRepository.Search(term);

            return View("Index", result);
        }
    }
}
