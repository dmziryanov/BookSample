using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using BookSample.Models;

namespace Repository
{
    public class DBLocalBookRepository : IBookRepository
    {
        private BookContext _bookContext;

        public DBLocalBookRepository()
        {
            _bookContext = new BookContext();
        }

        public void AddOrUpdate(Book entity)
        {
            _bookContext.Books.AddOrUpdate(entity);
            _bookContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _bookContext.Database.ExecuteSqlCommand("Delete From Books where id = {0}", id);
        }

        public IEnumerable<Book> GetAll()
        {
            return _bookContext.Books;
        }

        public void Merge(IEnumerable<Book> entities)
        {
            var books = entities.ToArray();
            _bookContext.Books.AddOrUpdate(books);
            _bookContext.SaveChanges();
        }

        public Book Get(int id)
        {
            return _bookContext.Books.Find(id);
        }

        public IEnumerable<Book> GetFilteredSortedPage(BookFieldType orderBy, BookFieldType filterBy, string filterString, int pageIndex, int pageSize)
        {
            string whereClause = "WHERE rownum > " + pageIndex*pageSize + " AND rownum <=" + (pageIndex*pageSize + pageSize);
            whereClause += string.IsNullOrEmpty(filterString)  ? "" : " AND " + filterBy + "='" +filterString+"'";
            string orderByClause = "order by " + orderBy;
            return _bookContext.Database.SqlQuery<Book>("select * FROM(Select *, ROW_NUMBER() OVER (PARTITION BY 1 " + orderByClause + ") as rownum From Book) a " + whereClause);
        }

        public int GetFilteredSortedPageCount(BookFieldType filterBy, string filterString, int pageSize)
        {
            string whereClause = string.IsNullOrEmpty(filterString) ? "" : "WHERE " + filterBy + "=" + filterString;
            return _bookContext.Database.SqlQuery<int>("select COUNT(*) From Book " + whereClause).FirstOrDefault() / pageSize + 1;
        }

        public int GetTotalCount()
        {
            return _bookContext.Database.SqlQuery<int>("Select count(*) From Books").FirstOrDefault();
        }
    }
}