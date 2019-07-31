using System.IO;
using System;

namespace Ks.Batch.Util
{
    public static class FileHelper
    {

        public static bool CreateBusyFile(string path, string nameFile = "busy", string extension =".txt")
        {
            var newPath = Path.Combine(path, nameFile + extension);
            var date = DateTime.Now;
            try
            {
                using (var myFile = File.Create(newPath))
                {
                    TextWriter tw = new StreamWriter(myFile);
                    tw.WriteLine(GetDateFormat(date));
                    tw.Close();
                }
            }
            catch (Exception)
            {
                //Log.Error("Problemas al crear archivo "+ newPath + " : " + e.Message);
            }

            return File.Exists(newPath);
        }

        public static bool DeleteBusyFile(string path, string nameFile = "busy", string extension = ".txt")
        {
            var newPath = Path.Combine(path, nameFile + extension);
            try
            {
                File.Delete(newPath);
            }
            catch(Exception)
            {
                //Log.Error("Problemas al borrar el archivo " + newPath + " : " + e.Message);
            }
            return File.Exists(newPath);
        }

        public static bool IsBusy(string path, string nameFile = "busy", string extension = ".txt")
        {
            var newPath = Path.Combine(path, nameFile + extension);
            return File.Exists(newPath);
        }

        public static void DeleteFile(string nameFile)
        { 
            if (File.Exists(nameFile))
                File.Delete(nameFile);
        }

        public static void PurgeFile(string pathBase, string nameFile = "busy", string extension = ".txt")
        {
            if(File.Exists(Path.Combine(pathBase, nameFile + extension)))
            { 
                var newPath = Path.Combine(pathBase, nameFile + extension);
                DateTime? oldTime = null;

                var lines = File.ReadLines(newPath);
        
                foreach (var line in lines)
                {
                    var r = line.Split('-');
                    if (line.Length > 0)
                        oldTime = new DateTime(Convert.ToInt32(r[0]), Convert.ToInt32(r[1]), Convert.ToInt32(r[2]), Convert.ToInt32(r[3]), Convert.ToInt32(r[4]), Convert.ToInt32(r[5]));
                }

                TimeSpan timeDiff = DateTime.Now -oldTime.Value;
                if (timeDiff.TotalSeconds > 60)
                {
                    //Log.Info("Archivo Purgado (60) : "+ timeDiff.TotalSeconds );
                    DeleteBusyFile(pathBase);
                }
            }
        }

        public static string GetDateFormat(DateTime date)
        {
            return string.Format("{0}-{1}-{2}-{3}-{4}-{5}", date.Year, date.Month.ToString("D2"), date.Day.ToString("D2"), date.Hour.ToString("D2"), date.Minute.ToString("D2"), date.Second.ToString("D2"));
        }

        public static string GetDate(DateTime date)
        {
            return string.Format("{0}{1}{2}", date.Year, date.Month.ToString("D2"), date.Day.ToString("D2"));
        }

        public static string GetTime(DateTime date)
        {
            return string.Format("{0}{1}{2}", date.Hour.ToString("D2"), date.Minute.ToString("D2"), date.Second.ToString("D2"));
        }
    }
}
