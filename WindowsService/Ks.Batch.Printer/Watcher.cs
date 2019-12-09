using System.IO;
using System.Threading;
using Topshelf.Logging;
using System.Drawing.Printing;
using System.Drawing;
using System;

namespace Ks.Batch.Printer
{
    public class Watcher
    {
        private static readonly LogWriter Log = HostLogger.Get<Watcher>();

        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 2); //10 Sec because is not atomic

            var jsonNode = ShoppingService.LoadJson();

            int BODYFONTSISE = 6;
            int FONTSISE = 8;

            int PAPER_WIDTH = 188;
            int PAPER_HEIGTH = 100;

            int LOGO_WIDTH = 188;
            int LOGO_HEIGTH = 70;

            int STEP = 10;
            int BLANK = 30;

            string title = @"c:/print/CBT_Title.png";
            //string barcode = Application.StartupPath + "\\code128bar.jpg";
            //Graphics g = e.Graphics;
            Bitmap myBitmap = new Bitmap(@"c:/print/background.png");
            Graphics g = Graphics.FromImage(myBitmap);

            g.DrawRectangle(Pens.Black, 0, 0, PAPER_WIDTH, PAPER_HEIGTH);

            //string TType = "S";

            //if (rbReturn.Checked)
            //{
            //    TType = "R";
            //}

            g.DrawImage(Image.FromFile(title), 0, 0, LOGO_WIDTH, LOGO_HEIGTH);
            Font fBody = new Font("Lucida Console", BODYFONTSISE, FontStyle.Bold);
            Font fBody1 = new Font("Lucida Console", BODYFONTSISE, FontStyle.Regular);
            Font rs = new Font("Stencil", FONTSISE, FontStyle.Bold);
            Font fTType = new Font("", 150, FontStyle.Bold);
            SolidBrush sb = new SolidBrush(Color.Black);


            g.DrawString("Av. Arenales 1279".ToCenter(), fBody1, sb, 0, LOGO_HEIGTH + STEP);
            g.DrawString("Fecha:".ToExtreme(DateTime.Now.ToShortDateString()), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 2);
            g.DrawString("Hora:".ToExtreme(DateTime.Now.ToLongTimeString()), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 3);
            g.DrawString("Barista:".ToExtreme("Darwin Fernández"), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 4);
            g.DrawString("Ticket Nro:".ToExtreme(jsonNode.TicketNumber.ToString("D7")), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 5);
            g.DrawString("Cliente:".ToExtreme(jsonNode.Customer), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 6);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 7);
            g.DrawString(" ".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 8);
            g.DrawString("".GetHeaderDetail(), fBody, sb, 0, LOGO_HEIGTH + BLANK + STEP * 9);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 10);

            var i = 11;
            foreach (var p in jsonNode.Details)
            {
                g.DrawString((p.Quantity + " " + p.ProductName).TrimEnd().ToExtreme(p.Price.ToString("C").Replace("S/", "").ToString().Trim()), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * i);
                i++;
            }

            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i));
            g.DrawString(("SubTotal: " + (jsonNode.Total * 0.82).ToString("C2")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP);
            g.DrawString(("Igv: " + (jsonNode.Total - jsonNode.Total * 0.82).ToString("C")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 2);
            g.DrawString(("Total: " + jsonNode.Total.ToString("C2")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 3);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 4);
            g.DrawString("Gracias Por su Preferencia".ToCenter(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 5);

            //g.DrawString(TType, fTType, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 6);
        }
    }
}
