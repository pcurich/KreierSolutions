using Ks.Batch.Util;
using Ks.Batch.Util.Model;
using System;
using System.Configuration;
using System.IO;
using System.Threading;
using Topshelf.Logging;

namespace Ks.Batch.Reverse
{
    public  class Watcher
    {
        public static readonly LogWriter Log = HostLogger.Get<Watcher>();

        public string SysName { get; set; }
        private string Connection { get; set; }

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 3); //3 Sec because is not atomic
            var connection = ConfigurationManager.ConnectionStrings["ACMR"].ConnectionString;
            var SysName = e.Name;
            //Puede ser [Ks.Batch.Caja.Out,Ks.Batch.Caja.In,Ks.Batch.Copere.In,Ks.Batch.Copere.Out,Ks.Batch.Merge]

            var code = getCode(SysName);

            if (code != 0) {

                var dao = new DaoReverse(connection);
                dao.Connect();

                var batch = dao.GetScheduleBatch(SysName);

                Log.InfoFormat(LogMessages.BatchScheduler, e.Name, batch.ToString());

                if (batch.PeriodMonth == 1)
                {
                    batch.PeriodMonth = 12;
                    batch.PeriodYear--;
                }
                else
                {
                    batch.PeriodMonth--;
                }

                Log.InfoFormat(LogMessages.BatchScheduleRevert, batch.PeriodYear.ToString("D4") + batch.PeriodMonth.ToString("D2"));

                var result = dao.StartReverse(getCode(SysName), batch);


                //LANZAR UN ERROR CRITICO XQ SI UNO DE LOS 2 FALLA, NO TENGO MANERA DE RECREAR TODO OTRA VEZ

                if (result)
                {
                    var source = batch.SystemName.Replace(".In", "").Replace(".Out", "");
                    var period = batch.PeriodYear.ToString("D4") + batch.PeriodMonth.ToString("D2");

                    dao.DeleteReport(period, source + ".In");
                    Log.InfoFormat(LogMessages.BatchReportDelete, source + ".In", period);

                    dao.DeleteReport(period, source + ".Out");
                    Log.InfoFormat(LogMessages.BatchReportDelete, source + ".Out", period);

                    if (batch.NextExecutionOnUtc != null)
                        batch.NextExecutionOnUtc = batch.NextExecutionOnUtc.Value.AddSeconds(30);
                    batch.LastExecutionOnUtc = DateTime.UtcNow;
                    dao.UpdateScheduleBatch(batch);
                    FileHelper.DeleteFile(e.FullPath);
                }
            }
            else
            {
                Log.InfoFormat("El servicio {0} no es posible revertir", SysName);
            }
            
        }

        private static int getCode(string sys)
        {
            if (sys.ToUpper().Contains("CAJA"))
                return (int)CustomerMilitarySituation.Retiro;

            if (sys.ToUpper().Contains("COPERE"))
                return (int)CustomerMilitarySituation.Actividad;

            return 0;
        }
    }
}
