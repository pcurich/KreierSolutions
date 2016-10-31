using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Test
{
    class Program
    {
        static void Main(string[] args)
        {
            var schedule = new ScheduleBatch
            {
                Id = 0,
                Name = "Exportacion para COPERE",
                SystemName = "Ks.Batch.Contribution.Copere.Out",
                PathRead = @"C:\Users\Pedro Curich\Dropbox\Kreier Solutions\Cooperativa\AUXILIO COOPERATIVO MILITAR\CAJA PENSION MILITAR POLICIAL",
                PathLog = @"C:\Users\Pedro Curich\Dropbox\Kreier Solutions\Cooperativa\AUXILIO COOPERATIVO MILITAR\CAJA PENSION MILITAR POLICIAL",
                PathMoveToDone = @"C:\Users\Pedro Curich\Dropbox\Kreier Solutions\Cooperativa\AUXILIO COOPERATIVO MILITAR\CAJA PENSION MILITAR POLICIAL",
                PathMoveToError = @"C:\Users\Pedro Curich\Dropbox\Kreier Solutions\Cooperativa\AUXILIO COOPERATIVO MILITAR\CAJA PENSION MILITAR POLICIAL",
                FrecuencyId = 7,
                StartExecutionOnUtc = DateTime.UtcNow,
                NextExecutionOnUtc = DateTime.UtcNow,
                LastExecutionOnUtc = DateTime.UtcNow,
                Enabled = true
            };

            XmlHelper.Serialize(schedule, Path.Combine(@"C:\KS\ACMR\WinService\Ks.Batch.Contribution.Copere.Out", "ScheduleBatch.xml"));

        }
    }
}
