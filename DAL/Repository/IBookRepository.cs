using System.Collections.Generic;
using DAL.Models;

namespace DAL.Repository
{
    public interface IBookRepository : IGenericRepository<Book>
    {
        IEnumerable<Book> GetFilteredSortedPage(BookFieldType sortBy, BookFieldType filterBy, string filterString, int pageIndex, int pageSize);
        int GetFilteredSortedPageCount(BookFieldType filterBy, string filterString, int pageSize);
        int GetTotalCount();
    }
}
