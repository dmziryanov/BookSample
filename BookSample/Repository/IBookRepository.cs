using System.Collections.Generic;
using BookSample.Models;

namespace Repository
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        IEnumerable<Book> GetFilteredSortedPage(BookFieldType sortBy, BookFieldType filterBy, string filterString, int pageIndex, int pageSize);
        int GetFilteredSortedPageCount(BookFieldType filterBy, string filterString, int pageSize);
        int GetTotalCount();
    }
}
