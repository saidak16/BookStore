using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BookStore.Models.Repositorios
{
    public class AuthorRepository : IBookstoreRepository<Author>
    {
        List<Author> authors;

        public AuthorRepository()
        {
            authors = new List<Author>()
            {
                new Author {Id = 1, FullName = "Ahmed Khojaly"},
                new Author {Id = 2, FullName = "Ahmed Mohammed"},
                new Author {Id = 3, FullName = "Ali Salah"}
            };
        }
        public void Add(Author entity)
        {
            entity.Id = authors.Max(a => a.Id) + 1;
            authors.Add(entity);
        }

        public void Delete(int id)
        {
            var author = Find(id);
            authors.Remove(author);
        }

        public Author Find(int id)
        {
            var author = authors.SingleOrDefault(a => a.Id == id);
            return author;
        }

        public IList<Author> List()
        {
            return authors;
        }

        public List<Book> Search(string term)
        {
            throw new NotImplementedException();
        }

        public void Update(int id, Author newAuthor)
        {
            var author = Find(id);

            author.FullName = newAuthor.FullName;
        }

        List<Author> IBookstoreRepository<Author>.Search(string term)
        {
            throw new NotImplementedException();
        }
    }
}
