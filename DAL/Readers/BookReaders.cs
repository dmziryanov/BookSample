using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;
using DAL.Models;

namespace DAL.Readers
{
    public class BookReaderFactory : IBookReaderFactory
    {
        public BookReader GetReader(string fileExt)
        {
            if (fileExt == ".xml")
            {
                return new XmlBookReader();
            }

            return new CsvBookReader();
        }
    }

    public interface IBookReaderFactory
    {
        BookReader GetReader(string fileExt);
    }

    public abstract class BookReader
    {
        public abstract IEnumerable<Book> GetBooks(StreamReader stream);
    }

    public class XmlBookReader : BookReader
    {
        public override IEnumerable<Book> GetBooks(StreamReader stream)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(BookList));
            return ((BookList)serializer.Deserialize(stream)).Books;
        }
    }

    public class CsvBookReader : BookReader
    {
        public override IEnumerable<Book> GetBooks(StreamReader stream)
        {
            var csv = stream.ReadLine();
            while (!string.IsNullOrEmpty(csv))
            {
                var str = csv.Split(';');
                var book = new Book()
                {
                    Title = str[0],
                    Author = str[1],
                    Year = Convert.ToInt32(str[2])
                };

                yield return book;
                csv = stream.ReadLine();
            }
        }
    }
}