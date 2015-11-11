using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Web;
using ExcelLibrary.SpreadSheet;

namespace BookSample.Common
{
    public class ExportEntity<T> : IFileExport
    {
        private readonly string _filename;
        private readonly IEnumerable<T> _entities;
        private PropertyInfo[] _properties;

        public ExportEntity(IEnumerable<T> entities, string fileName)
        {
            _filename = fileName;
            _entities = entities;
        }

        public string SaveToPath()
        {
            var path = Path.Combine(HttpContext.Current.Server.MapPath(@"~/App_Data"), _filename);
            Workbook workbook = new Workbook();
            Worksheet worksheet = new Worksheet("First Sheet");
            workbook.Worksheets.Add(worksheet);
            var i = 0;
            int j;
            foreach (var entity in _entities)
            {
                i++;
                if (i == 1)
                    _properties = typeof (T).GetProperties();

                j = 0;
                foreach (var prop in _properties)
                {
                    if (prop.GetCustomAttributes(typeof(ExportAttribute), true).Length > 0)
                    {
                        j++;
                        worksheet.Cells[i, j] = new Cell(prop.GetValue(entity, null));
                    }
                }
            }

            workbook.Save(path);
            return path;
                
        }
    }

    public class ExportAttribute : Attribute
    {

    }

    public interface IFileExport
    {
        string SaveToPath();
    }
}