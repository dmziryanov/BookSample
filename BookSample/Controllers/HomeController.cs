using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Serialization;
using BookSample.Common;
using BookSample.Models;
using ExcelLibrary.SpreadSheet;
using Repository;

namespace BookSample.Controllers
{
    namespace SimpleLogin.Controllers
    {
        public class HomeController : Controller
        {

            private readonly IBookRepository _bookRepository;
            private readonly int pageSize = 5;
            private string _booksdocXls = "Book.xls";
            private IBookReaderFactory _bookReaderFactory;

            public HomeController()
            {
                
            }


            public HomeController(IBookRepository BookRepository, IBookReaderFactory bookReaderFactory)
            {
                _bookRepository = BookRepository;
                _bookReaderFactory = bookReaderFactory;
            }

            [HttpGet, ActionName("Index")]
            public ViewResult Index()
            {
                return View();
            }


            [HttpGet, ActionName("Error")]
            public ViewResult GetResult()
            {
                return View("Error");
            }

            [HttpGet, ActionName("GetData")]
            public JsonResult GetData()
            {
                int currentPage = 0;
                if (int.TryParse(Request.QueryString["currentPage"], out currentPage))
                    return Json(GetBooks().Skip(pageSize * (currentPage - 1)).Take(pageSize), JsonRequestBehavior.AllowGet);
                else
                    return Json(GetBooks().Skip(0).Take(pageSize), JsonRequestBehavior.AllowGet);
            }

            private List<Book> GetBooks()
            {
                BookFieldType sortbookField;
                BookFieldType filterbookField;

                if (Request.QueryString["FilterBy"] == BookFieldType.Author.ToString())
                {
                    filterbookField = BookFieldType.Author;
                }
                else if ((Request.QueryString["FilterBy"] == BookFieldType.Title.ToString()))
                {
                    filterbookField = BookFieldType.Title;
                }
                else filterbookField = BookFieldType.Year;

                if (Request.QueryString["SortBy"] == BookFieldType.Author.ToString())
                {
                    sortbookField = BookFieldType.Author;
                }
                else if ((Request.QueryString["SortBy"] == BookFieldType.Title.ToString()))
                {
                    sortbookField = BookFieldType.Title;
                }
                else sortbookField = BookFieldType.Year;

                return _bookRepository.GetFilteredSortedPage(sortbookField, filterbookField, Request.QueryString["Filter"]).ToList();
            }

            [HttpGet, ActionName("GetPagesData")]
            public JsonResult GetPagesData()
            {
                var count = GetBooks().Count;

                if (count % pageSize == 0)
                    return Json(Enumerable.Range(0, count / pageSize).ToList().Select(x => new { pagenum = x +1 }), JsonRequestBehavior.AllowGet);
                
                return Json(Enumerable.Range(0, count / pageSize + 1).ToList().Select(x => new { pagenum = x + 1 }), JsonRequestBehavior.AllowGet);
            }

            [HttpPost]
            public ActionResult Upload(HttpPostedFileBase file)
            {
                if (file == null || file.ContentLength <= 0) return RedirectToAction("Index");
                
                try
                {
                    using (StreamReader st = new StreamReader(file.InputStream))
                    {
                        var newBooks = _bookReaderFactory.GetReader(Path.GetExtension(file.FileName));
                        _bookRepository.Merge(newBooks.GetBooks(st));    
                    }

                    return RedirectToAction("Index");
                }
                
                catch
                {
                    return RedirectToAction("Error");
                }
            }

            [HttpGet, ActionName("Edit")]
            public ActionResult Edit(int id)
            {
                Book book = _bookRepository.Get(id);

                if (book == null)
                {
                    return this.HttpNotFound();
                }

                return this.View(book);
            }

            [HttpGet, ActionName("Delete")]
            public ActionResult Delete(int id)
            {
                _bookRepository.Delete(id);
                return View("Index");
            }

            [HttpPost, ActionName("Edit")]
            [ValidateAntiForgeryToken]
            public ActionResult Edit([ModelBinder(typeof(BookModelBinder))]  Book entity)
            {
                _bookRepository.AddOrUpdate(entity);
                return View(entity);
            }

            [HttpGet, ActionName("Export")]
            public FileResult Export()
            {
                var fileExport = new ExportEntity<Book>(_bookRepository.GetAll(), _booksdocXls);
                FileInfo info = new FileInfo(fileExport.SaveToPath());
                return File(info.OpenRead(), "text/plain", info.Name);
            }
        }
    }

   
}
