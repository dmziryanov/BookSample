using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using BookSample.Common;
using DAL.Models;
using DAL.Readers;
using DAL.Repository;

namespace BookSample.Controllers
{
    namespace SimpleLogin.Controllers
    {
        public class HomeController : Controller
        {
            private readonly IBookRepository _bookRepository;
            private readonly int pageSize = 5;
            private string _booksdocXls =  "Book.xls";
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
                int currentPage;
                BookFieldType sortbookField;
                BookFieldType filterbookField;
                string filterString;
                GetBooksRequestParams(out sortbookField, out filterbookField, out filterString,out currentPage);
                
                return Json(_bookRepository.GetFilteredSortedPage(sortbookField, filterbookField, filterString, currentPage - 1, pageSize), JsonRequestBehavior.AllowGet);
            }

            private void GetBooksRequestParams(out BookFieldType sortBy, out BookFieldType filterBy, out string filterString, out int currentPage)
            {
                if (Request.QueryString["FilterBy"] == BookFieldType.Author.ToString())
                {
                    filterBy = BookFieldType.Author;
                }
                else if ((Request.QueryString["FilterBy"] == BookFieldType.Title.ToString()))
                {
                    filterBy = BookFieldType.Title;
                }
                else filterBy = BookFieldType.Year;

                if (Request.QueryString["SortBy"] == BookFieldType.Author.ToString())
                {
                    sortBy = BookFieldType.Author;
                }
                else if ((Request.QueryString["SortBy"] == BookFieldType.Title.ToString()))
                {
                    sortBy = BookFieldType.Title;
                }
                else sortBy = BookFieldType.Year;
                
                filterString = Request.QueryString["Filter"];

                if (!(int.TryParse(Request.QueryString["currentPage"], out currentPage)))
                    currentPage = 1;
            }

            [HttpGet, ActionName("GetPagesData")]
            public JsonResult GetPagesData()
            {
                int currentPage;
                BookFieldType sortbookField;
                BookFieldType filterbookField;
                string filterString;
                GetBooksRequestParams(out sortbookField, out filterbookField, out filterString, out currentPage);
                var pageCount = _bookRepository.GetFilteredSortedPageCount(filterbookField, filterString, pageSize);
                return Json(Enumerable.Range(0, pageCount).ToList().Select(x => new { pagenum = x + 1 }), JsonRequestBehavior.AllowGet);
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
                var fileExport = new ExportEntity<Book>(_bookRepository.GetAll(), HttpContext.Server.MapPath(@"~/App_Data") + _booksdocXls);
                FileInfo info = new FileInfo(fileExport.SaveToPath());
                return File(info.OpenRead(), "text/plain", info.Name);
            }
        }
    }

   
}
