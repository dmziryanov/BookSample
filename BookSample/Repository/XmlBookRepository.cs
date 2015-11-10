using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using BookSample.Models;

namespace Repository
{
    public class XmlBookRepository : IBookRepository
    {
        private static readonly string _booksSampleXml = "books_sample_1.xml";
        private static readonly object locker = new object();

        public void AddOrUpdate(Book entity)
        {
            BookList tmp = new BookList();
            tmp.Books = GetAll().ToList();
            var rep = tmp.Books.FirstOrDefault(x => x.Id == entity.Id);
            tmp.Books.Remove(rep);
            tmp.Books.Add(entity);
            PersistXml(tmp);
        }

        public void Delete(int id)
        {
            BookList tmp = new BookList();
            tmp.Books = GetAll().ToList();
            var rep = tmp.Books.FirstOrDefault(x => x.Id == id);

            lock (locker)
            {
                tmp.Books.Remove(rep);
                PersistXml(tmp);
            }
        }

        private static void PersistXml(BookList tmp)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BookList));
            using (
                StreamWriter writer =
                    new StreamWriter(Path.Combine(HttpContext.Current.Server.MapPath(@"~/App_Data"),
                        _booksSampleXml)))
            {
                serializer.Serialize(writer, tmp);
            }
        }

        public IEnumerable<Book> GetAll()
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BookList));
            using (StreamReader reader = new StreamReader(Path.Combine(HttpContext.Current.Server.MapPath(@"~/App_Data"), _booksSampleXml)))
            {
                return ((BookList)serializer.Deserialize(reader)).Books;
            }
        }

        public void Merge(IEnumerable<Book> entities)
        {
            BookList tmp = new BookList();
            tmp.Books = GetAll().ToList();
            var seedId = tmp.Books.Max(x => x.Id) + 1;
            foreach (var book in entities)
            {
                if (book.Id == 0) book.Id = ++seedId;
            }
            tmp.Books = tmp.Books.Union(entities).ToList();
            PersistXml(tmp);
        }

        public Book Get(int id)
        {
            return GetAll().ToList().Where(x => x.Id == id).FirstOrDefault();
        }

        public IEnumerable<Book> GetFilteredSortedPage(BookFieldType sortBy, BookFieldType filterBy, string filterString)
        {
            IEnumerable<Book> initResult;
            XmlSerializer serializer = new XmlSerializer(typeof(BookList));
            lock (locker)
            {
                using (
                    StreamReader reader =
                        new StreamReader(Path.Combine(HttpContext.Current.Server.MapPath(@"~/App_Data"),
                            _booksSampleXml)))
                {
                    initResult = ((BookList)serializer.Deserialize(reader)).Books;
                }
            }
            IEnumerable<Book> filteredresult = initResult;
            IEnumerable<Book> sortedResult = initResult;

            if (!string.IsNullOrEmpty(filterString))
            {
                switch (filterBy)
                {
                    case BookFieldType.Author:
                        filteredresult = initResult.Where(x => x.Author.Contains(filterString));
                        break;

                    case BookFieldType.Title:
                        filteredresult = initResult.Where(x => x.Title.Contains(filterString));
                        break;


                    case BookFieldType.Year:
                        int year = 0;
                        if (int.TryParse(filterString, out year))
                            filteredresult = initResult.Where(x => x.Year == year);
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
            throw new System.NotImplementedException();
        }
    }
}