using System.Collections.Generic;
using System.IO;
using System.Linq;
using Quartz;

namespace Ks.Batch.Sync
{
    public class InfoJob : IJob
    {
        private const string PATH = @"C:\KS\ACMR\WinService";
        public void Execute(IJobExecutionContext context)
        {
            #region DataBase
            var dao = new DaoService();
            dao.Connect();
            List<ScheduleBatch> scheduleBatchs = dao.Process();
            dao.Close();
            #endregion

            #region ScheduleBatch

            var dirs = Directory.GetFiles(PATH, "*ScheduleBatch.xml",SearchOption.AllDirectories);

            foreach (var scheduleBatch in scheduleBatchs)
            {
                var batch = scheduleBatch;
                var file = dirs.FirstOrDefault(x => x.Contains(batch.SystemName));
                //file = C:\KS\ACMR\WinService\Ks.Batch.Contribution.Copere.Out\
                if(file == null)
                    continue;

                if (File.Exists(Path.Combine(file, "ScheduleBatch.xml")))
                    File.Delete(file);

                XmlHelper.Serialize(scheduleBatch,Path.Combine(file, "ScheduleBatch.xml"));
            }

            #endregion
        }

    }
}