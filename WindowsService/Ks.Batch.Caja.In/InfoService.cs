using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Caja.In
{
    public class InfoService
    {
        private static readonly LogWriter Log = HostLogger.Get<InfoService>();

        public static List<Info> ReadFile(string path,string defaultCulture)
        {
            var culture = new CultureInfo(defaultCulture);
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            var lines = File.ReadLines(path);
            var result = new List<Info>();

            foreach (var line in lines)
            {
                try
                {
                    //002960082011253380001LE07477168  2019050000034.85
                    if (line.Length >48)
                    {
                        result.Add(new Info
                        {
                            Year = Convert.ToInt32(line.Substring(33, 4)),
                            Month = Convert.ToInt32(line.Substring(37, 2)),
                            AdminCode = line.Substring(10, 9),
                            HasAdminCode = true,
                            HasDni = true,
                            Dni = line.Substring(23, 8),
                            TotalPayed = Convert.ToDecimal(line.Substring(39, 10)),
                            InfoContribution = null,
                            InfoLoans = null,
                            IsUnique = true
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Read File with error in line: '{0}' with message error: '{1}'", line, ex.Message);
                    return null;
                } 
            }

            Log.InfoFormat("3.- Lines of File: {0} | Amount Total: {1} ", result.Count, result.Sum(x => x.TotalPayed));
            return result;
        }
    }
}