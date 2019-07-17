using System.Collections.Generic;
using System.IO;
using OfficeOpenXml;

namespace Ks.Batch.Util
{
    public static class ExcelFile
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sheetName"></param>
        /// <param name="offset"></param>
        /// <param name="stream"></param>
        /// <param name="properties"></param>
        /// <param name="data">row, col, value</param>
        public static void CreateReport(string sheetName,int offset, Stream stream, string[] properties, Dictionary<int,Dictionary<int,string>> data, string fileName)
        {
            using (var xlPackage = new ExcelPackage(stream))
            {
                // get handle to the existing worksheet
                var worksheet = xlPackage.Workbook.Worksheets.Add(sheetName);
                for (int i = 0; i < properties.Length; i++)
                {
                    worksheet.Cells[offset, i + 1].Value = properties[i];
                }

                var row = 2;
                var col = 1;

                foreach (var rows in data)
                {
                    foreach (var cols in rows.Value)
                    {
                        if (col <= properties.Length)
                            worksheet.Cells[row, col].Value = rows.Value[col-1].Trim();
                        else
                            break;
                                                
                        col++;
                    }
                    row++;
                    col = 1;
                }

                for (var i = 1; i <= worksheet.Dimension.Columns; i++)
                {
                    worksheet.Column(i).AutoFit();
                }

                xlPackage.SaveAs(new FileInfo(fileName));
            } 
        }

        
    }
}
