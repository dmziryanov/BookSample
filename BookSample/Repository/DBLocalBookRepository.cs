using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BookSample.Models;
using Castle.Core.Internal;

namespace Repository
{
    public class DBLocalBookRepository : IBookRepository
    {
        private BookContext bookContext;

        public DBLocalBookRepository()
        {
            bookContext = new BookContext();
        }

        public void AddOrUpdate(Book entity)
        {
            bookContext.Books.AddOrUpdate(entity);
            bookContext.SaveChanges();
        }

        public void Delete(int id)
        {
            bookContext.Database.ExecuteSqlCommand("Delete From Books where id = {0}", id);
        }

        public IEnumerable<Book> GetAll()
        {
            return bookContext.Books;
        }

        public void Merge(IEnumerable<Book> entities)
        {
            var books = entities.ToArray();
            bookContext.Books.AddOrUpdate(books);
            bookContext.SaveChanges();
        }

        public Book Get(int id)
        {
            return bookContext.Books.Find(id);
        }

        public IEnumerable<Book> GetFilteredSortedPage(BookFieldType sortBy, BookFieldType filterBy, string filterString)
        {

            IEnumerable<Book> filteredresult = bookContext.Books;
            IEnumerable<Book> sortedResult = filteredresult;

            if (!string.IsNullOrEmpty(filterString))
            {
                switch (filterBy)
                {
                    case BookFieldType.Author:
                        filteredresult = bookContext.Books.Where(x => x.Author.Contains(filterString));
                        break;

                    case BookFieldType.Title:
                        filteredresult = bookContext.Books.Where(x => x.Title.Contains(filterString));
                        break;


                    case BookFieldType.Year:
                        int year = 0; if (int.TryParse(filterString, out year))
                            filteredresult = bookContext.Books.Where(x => x.Year == year);
                        break;

                }
            }

            switch (sortBy)
            {
                case BookFieldType.Author:
                    sortedResult = filteredresult.OrderBy(x => x.Author);
                    break;

                case BookFieldType.Title:
                    sortedResult = filteredresult.OrderBy(x => x.Title);
                    break;

                case BookFieldType.Year:
                    sortedResult = filteredresult.OrderBy(x => x.Year);
                    break;
            }

            return sortedResult;
        }

        public int GetTotalCount()
        {
            return bookContext.Database.SqlQuery<int>("Select count(*) From Books").FirstOrDefault();
        }
    }
}