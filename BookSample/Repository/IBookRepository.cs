using System.Collections.Generic;
using BookSample.Models;

namespace Repository
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        IEnumerable<Book> GetFilteredSortedPage(BookFieldType sortBy, BookFieldType filterBy, string filterString);
        int GetTotalCount();
    }
}
