using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Copere.In
{
    public class InfoService
    {
        private static readonly LogWriter Log = HostLogger.Get<InfoService>();

        public static List<Info> ReadFile(string path)
        {
            CultureInfo culture = new CultureInfo("es-PE");
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var lines = File.ReadLines(path);
            var result = new List<Info>();
            var count = 0;

            foreach (var line in lines)
            {
                try
                {
                    if (line.Length == 28)
                    {
                        result.Add(new Info
                        {
                            Year = Convert.ToInt32(line.Substring(0, 4)),
                            Month = Convert.ToInt32(line.Substring(4, 2)),
                            HasAdminCode = true,
                            HasDni = false,
                            AdminCode = line.Substring(10, 9),
                            TotalPayed = Convert.ToDecimal(line.Substring(19, 9))/100
                        });
                    }
                    else
                    {
                        if (line.Length >= 100 && !line.ToUpper().Contains("TOTAL"))
                        {
                            result.Add(new Info
                            {
                                Year = Convert.ToInt32(line.Substring(5, 4)),
                                Month = Convert.ToInt32(line.Substring(10, 2)),
                                HasAdminCode = true,
                                HasDni = false,
                                AdminCode = line.Substring(34, 9),
                                TotalPayed = Convert.ToDecimal(line.Substring(86, 10))
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (count != 0)
                    {
                        Log.ErrorFormat("Read File with error message error: '{0}'", ex.Message);
                        Log.ErrorFormat("Read File with error in line: '{0}'", line);
                        return null;
                    }
                    count++;
                }
            }
            return result;
        }
    }
}