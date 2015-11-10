using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Xml.Serialization;
using BookSample.Common;

namespace BookSample.Models
{
    [XmlRoot("Books")]
    public class BookList
    {
        [XmlElement("Book")]
        public List<Book> Books { get; set; }
    }

    public class Book : IComparable
    {

        [XmlElement("Id")]
        [Key]
        public int Id { get; set; }

        [Export]
        [Required(ErrorMessage = "Title is required", AllowEmptyStrings = false)]
        [Display(Name = "Title")]
        [XmlElement("Title")]
        [StringLength(255, ErrorMessage = "The {0} must be at least {2} characters long and {1} max.", MinimumLength = 2)]
        public string Title { get; set; }

        [Export]
        [Required(ErrorMessage = "Author is required", AllowEmptyStrings = false)]
        [Display(Name = "Author")]
        [XmlElement("Author")]
        [StringLength(255,ErrorMessage = "The {0} must be at least {2} characters long and {1} max.", MinimumLength = 2)]
        public string Author { get; set; }

        [Export]
        [Required(ErrorMessage = "Year is required")]
        [Display(Name = "Year")]
        [XmlElement("Year")]
        public int Year { get; set; }

        [Export]
        [Required(ErrorMessage = "ISBN is required")]
        [Display(Name = "ISBN")]
        [XmlElement("ISBN")]
        public int ISBN { get; set; }

        public string DisplayISBN {
            get { return "ISBN: " + ISBN;}
        }

        public string editurl
        {
            get { return "Home/Edit/" + Id; }
        }

        public string deleteurl
        {
            get { return "Home/Delete/" + Id; }
        }

        public int CompareTo(object obj)
        {
            var other = obj as Book;
            if (other == null) return -1;

            if (other.Id == Id) return 0;
            if (other.Id > Id) return -1;

            return 1;
        }
    }

    public class BookModelBinder : IModelBinder
    {
        public object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            if (controllerContext == null)
                throw new ArgumentNullException("controllerContext", "controllerContext is null.");
            if (bindingContext == null)
                throw new ArgumentNullException("bindingContext", "bindingContext is null.");

            var Title = TryGet<String>(bindingContext, "Title");
            var Author = TryGet<String>(bindingContext, "Author");
            var ISBN = TryGet<int>(bindingContext, "ISBN");
            var Year = TryGet<int>(bindingContext, "Year");
            var Id = TryGet<int>(bindingContext, "Id");
            return new Book() {Title = Title, Author = Author, ISBN = ISBN, Id = Id, Year = Year};
        }

        private T TryGet<T>(ModelBindingContext bindingContext, string key) 
        {
            if (String.IsNullOrEmpty(key))
                return default(T);

            ValueProviderResult valueResult = bindingContext.ValueProvider.GetValue(bindingContext.ModelName + "." + key);
            if (valueResult == null && bindingContext.FallbackToEmptyPrefix == true)
                valueResult = bindingContext.ValueProvider.GetValue(key);

            bindingContext.ModelState.SetModelValue(bindingContext.ModelName, valueResult);

            if (valueResult == null)
                return default(T);

            try
            {
                return (T)valueResult.ConvertTo(typeof(T));
            }
            catch (Exception ex)
            {
                bindingContext.ModelState.AddModelError(bindingContext.ModelName, ex);
                return default(T);
            }
        }
    }

    public enum BookFieldType
    {
        None = 0,
        Title = 1,
        Author = 2,
        Year = 3
    }
}