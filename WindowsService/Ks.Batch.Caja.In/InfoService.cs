using System;
using System.Collections.Generic;
using System.IO;
using Ks.Batch.Util.Model;
using Topshelf.Logging;

namespace Ks.Batch.Caja.In
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
                    if (line.Length >48)
                    {
                        result.Add(new Info
                        {
                            Year = Convert.ToInt32(line.Substring(33, 4)),
                            Month = Convert.ToInt32(line.Substring(37, 2)),
                            AdminCode = line.Substring(10, 9),
                            HasAdminCode = true,HasDni = true,
                            Dni = line.Substring(23, 8),
                            TotalPayed = Convert.ToDecimal(line.Substring(39, 10))
                        });
                    }
                }
                catch (Exception ex)
                {
                    Log.ErrorFormat("Read File with error message error: '{0}'",ex.Message);
                    Log.ErrorFormat("Read File with error in line: '{0}'", line);
                    return null;
                }
            }
            return result;
        }
    }
}