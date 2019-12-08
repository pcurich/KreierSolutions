using NUnit.Framework;
using System.Collections.Generic;

namespace Ks.Batch.Test
{
    [TestFixture]
    public class UnitTest1
    {
        [SetUp]
        public void Init()
        {

        }

        [Test]
        [Category("Caja")]
        public void Caja_Leer_Archivo()
        {
            List<Util.Model.Info> data = Caja.In.InfoService.ReadFile("../../Caja/CPMP 2019 05_6008 MAYO.txt", "es-PE");
            Assert.IsNotNull(data);
        }
    }
}
