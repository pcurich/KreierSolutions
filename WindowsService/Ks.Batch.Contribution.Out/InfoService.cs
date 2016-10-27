using System;
using System.Collections.Generic;
using System.IO;
using Topshelf.Logging;

namespace Ks.Batch.Contribution.Out
{
    public class InfoService
    {
        private static readonly LogWriter Log = HostLogger.Get<InfoService>();

        public static List<Info> ReadFile(string path)
        {
            var lines = File.ReadLines(path);
            var result = new List<Info>();

            foreach (var line in lines)
            {
                try
                {
                    if (line.Length >= 100)
                    {
                        result.Add(new Info
                        {
                            Year = Convert.ToInt32(line.Substring(5, 4)),
                            Month = Convert.ToInt32(line.Substring(10, 2)),
                            AdminCode = line.Substring(34, 9),
                            Total = Convert.ToDecimal(line.Substring(86, 10))
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Read File with error message error: '{0}'",ex.Message);
                    Log.ErrorFormat("Read File with error in line: '{0}'", line);
                }
            }
            return result;
        }
    }
}