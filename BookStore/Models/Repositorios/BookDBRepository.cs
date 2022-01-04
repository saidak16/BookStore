using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.Repositorios
{
    public class BookDBRepository : IBookstoreRepository<Book>
    {
        BookStoreDBContext db;

        public BookDBRepository(BookStoreDBContext _db)
        {
            db = _db;
        }

        public void Add(Book entity)
        {
            db.Add(entity);
            db.SaveChanges();
        }

        public void Delete(int id)
        {
            var book = Find(id);

            db.Remove(book);
            db.SaveChanges();
        }

        public Book Find(int id)
        {
            var book = db.Books.Include(a => a.Author).SingleOrDefault(b => b.Id == id);

            return book;
        }

        public IList<Book> List()
        {
            return db.Books.Include(a => a.Author).ToList();
        }

        public void Update(int id, Book newBook)
        {
            db.Update(newBook);
            db.SaveChanges();
        }

        public List<Book> Search(string term)
        {
            var result = db.Books.Include(a => a.Author).Where(b => b.Title.Contains(term) ||
            b.Description.Contains(term) ||
            b.Author.FullName.Contains(term)).ToList();

            return result;
        }
    }
}
