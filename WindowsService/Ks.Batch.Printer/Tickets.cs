using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Ks.Batch.Printer
{
    public class Tickets
    {
        public const int LINE = 32;
        
        public static void Print(string chain, string impresora)
        {
            //using (StreamWriter sw = new StreamWriter(@"C:\reportes\TestFile.txt", true))
            //{
            //    sw.WriteLine(Cadena);
            //    sw.WriteLine("-------------------");
            //}
            RawPrinterHelper rph = new RawPrinterHelper();
            //rph.SendStringToPrinter(@"EPSON TM-U220 Receipt", Cadena);
            //rph.SendStringToPrinter(@"EPSON TM-U220 Receipt", "\x1B" + "m");// caracteres de corte
            //rph.SendStringToPrinter(@"EPSON TM-U220 Receipt", "\x1B" + "d" + "\x3"); // avanza 9 renglones
            rph.SendStringToPrinter(impresora, chain);
            rph.SendStringToPrinter(impresora, "31 03 16 3");
            rph.SendStringToPrinter(impresora, "\x1B" + "m");// caracteres de corte
            rph.SendStringToPrinter(impresora, "\x1B" + "d" + "\x3"); // avanza 9 renglones
        }

        string AlignCenter(string line, string regex = "", int cantCaracteres = LINE)
        {
            int longitud = cantCaracteres - line.Trim().Length;
            int coladerecha = longitud / 2;
            int colaizquierda = longitud - coladerecha - 1;
            string salida = String.Empty;

            for (int i = 0; i < coladerecha; i++)
            {
                salida += regex;
            }
            salida += line.Trim();

            for (int i = 0; i < colaizquierda; i++)
            {
                salida += regex;
            }

            salida += "\n";
            return salida;
        }

        string AlignRight(string line, int cantCaracteres = LINE, string relleno = "", bool salto = true)
        {
            string salida = String.Empty;
            for (int i = 0; i < cantCaracteres - line.Length; i++)
            {
                salida += relleno;
            }

            salida += line.Trim();
            if (salto)
                salida += "\n";
            else
                salida += " ";
            return salida;
        }

        string AlignLeft(string line, int cantCaracteres = LINE, string relleno = "", bool salto = true)
        {
            string salida = String.Empty;
            salida += line.Trim();
            for (int i = 0; i < cantCaracteres - line.Length; i++)
            {
                salida += relleno;
            }
            if (salto)
                salida += "\n";
            else
                salida += " ";
            return salida;
        }
    }
}
