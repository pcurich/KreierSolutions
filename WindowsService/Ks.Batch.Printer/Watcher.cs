using System.IO;
using System.Threading;
using Topshelf.Logging;
using System.Drawing.Printing;
using System.Drawing;
using System;
using static Ks.Batch.Printer.ShoppingService;

namespace Ks.Batch.Printer
{
    public class Watcher
    {
        private static readonly LogWriter Log = HostLogger.Get<Watcher>();
        //public ShoppingCart jsonNode = new ShoppingCart();
        public static void FileCreated(object sender, FileSystemEventArgs e)
        {
            Thread.Sleep(1000 * 2); //10 Sec because is not atomic

            //jsonNode = LoadJson(e.FullPath);

            PrintDocument pd = new PrintDocument();
            PaperSize ps = new PaperSize("", 475, 550);

            pd.PrintPage += (sender2, args) => pd_PrintPage(e.FullPath, args); 

            pd.PrintController = new StandardPrintController();
            pd.DefaultPageSettings.Margins.Left = 0;
            pd.DefaultPageSettings.Margins.Right = 0;
            pd.DefaultPageSettings.Margins.Top = 0;
            pd.DefaultPageSettings.Margins.Bottom = 0;

            pd.DefaultPageSettings.PaperSize = ps;
            pd.Print();

            MoveFile(e.FullPath, e.Name);


        }

        public static void pd_PrintPage(object sender, PrintPageEventArgs e)
        {
            var jsonNode = LoadJson(sender.ToString());

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
            Graphics g = e.Graphics;
            //Bitmap myBitmap = new Bitmap(@"c:/print/background.png");
            //Graphics g = Graphics.FromImage(myBitmap);

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
            g.DrawString("Barista:".ToExtreme(jsonNode.User.Name), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 4);
            g.DrawString("Ticket Nro:".ToExtreme(jsonNode.TicketNumber.ToString("D7")), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 5);
            g.DrawString("Cliente:".ToExtreme(jsonNode.Customer != null ? jsonNode.Customer.Name + " " + jsonNode.Customer.LastName : ""), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 6);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 7);
            g.DrawString(" ".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 8);
            g.DrawString("".GetHeaderDetail(), fBody, sb, 0, LOGO_HEIGTH + BLANK + STEP * 9);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * 10);

            var i = 11;
            foreach (var p in jsonNode.Details)
            {
                g.DrawString((p.Quantity + " " + p.Product.Name).TrimEnd().ToExtreme(p.Price.ToString("C").Replace("S/", "").ToString().Trim()), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * i);
                i++;
            }

            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i));
            g.DrawString(("SubTotal: " + (jsonNode.Total * 0.82).ToString("C2")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP);
            g.DrawString(("Igv: " + (jsonNode.Total - jsonNode.Total * 0.82).ToString("C")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 2);
            g.DrawString(("Total: " + jsonNode.Total.ToString("C2")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 3);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 4);

            g.DrawString(("Método: " + (jsonNode.PaymentType == "Cash" ? "Efectivo" : "Tarjeta")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 5);
            g.DrawString(("Entregado: " + (jsonNode.PaymentType == "Cash" ? jsonNode.Cash : jsonNode.Credit).ToString("C")).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 6);
            g.DrawString(("Vuelto: " + (Math.Abs(jsonNode.Change).ToString("C"))).ToRight(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 7);
            g.DrawString("-".Continuous(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 8);
            g.DrawString("Gracias Por su Preferencia".ToCenter(), fBody1, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 9);

            //g.DrawString(TType, fTType, sb, 0, LOGO_HEIGTH + BLANK + STEP * (i) + STEP * 6);
        }

        private static void MoveFile(string fullPath, string fileName)
        {
            var destiny = Path.Combine(Path.Combine(@"c:/print/queve", "done"), fileName);
            if (File.Exists(destiny))
                File.Delete(destiny);
            File.Move(fullPath, Path.Combine(Path.Combine(@"c:/print/queve", "done"), fileName));
        }
    }
}
