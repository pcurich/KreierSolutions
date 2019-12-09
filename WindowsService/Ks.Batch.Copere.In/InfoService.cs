using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Copere.In
{
    public class InfoService
    {
        private static readonly LogWriter Log = HostLogger.Get<InfoService>();

        public static List<Info> ReadFile(string path, string defaultCulture, bool isContribution, bool isLoan)
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
                    //2019088033112646600000075556
                    if (line.Length == 28)
                    {
                        //se asume que viene totalizado
                        result.Add(new Info
                        {
                            Year = Convert.ToInt32(line.Substring(0, 4)),
                            Month = Convert.ToInt32(line.Substring(4, 2)),
                            HasAdminCode = true,
                            HasDni = false,
                            AdminCode = line.Substring(10, 9),

                            TotalLoan = (isLoan)? Convert.ToDecimal(line.Substring(19, 9)) / 100: 0,
                            TotalContribution = (isContribution) ? Convert.ToDecimal(line.Substring(19, 9)) / 100 : 0,

                            TotalPayed = Convert.ToDecimal(line.Substring(19, 9))/100,
                            InfoContribution = null,
                            InfoLoans = null,
                            IsUnique = false

                        });
                    }
                    else
                    {
                        //deprecado
                        if (line.Length >= 100 && !line.ToUpper().Contains("TOTAL"))
                        {
                            result.Add(new Info
                            {
                                Year = Convert.ToInt32(line.Substring(5, 4)),
                                Month = Convert.ToInt32(line.Substring(10, 2)),
                                HasAdminCode = true,
                                HasDni = false,
                                AdminCode = line.Substring(34, 9),

                                TotalLoan = (isLoan) ? Convert.ToDecimal(line.Substring(86, 10)) : 0,
                                TotalContribution = (isContribution) ? Convert.ToDecimal(line.Substring(86, 10)) : 0,

                                TotalPayed = Convert.ToDecimal(line.Substring(86, 10)),

                                InfoContribution = null,
                                InfoLoans = null,
                                IsUnique = true
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Read File with error in line: '{0}' with message error: '{1}'",line, ex.Message);
                    return null;
                } 
            }

            Log.InfoFormat("3.- Lines of File: {0} | Amount Total: {1} ", result.Count, result.Sum(x => x.TotalPayed));
            return result;
        }
    }
}