using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ks.Batch.Printer
{
    public static class HelperTicket
    {
        public static string ToCenter(this string text)
        {
            var line = new StringBuilder();
            if (text.Length > Config.MAXCAR)
            {
                int count = 0;
                for (int longitudTexto = text.Length; longitudTexto > Config.MAXCAR; longitudTexto -= Config.MAXCAR)
                {
                    line.AppendLine(text.Substring(count, Config.MAXCAR));
                    count += Config.MAXCAR;
                }

                string espacios = "";
                int centrar = (Config.MAXCAR - text.Substring(count, text.Length - count).Length) / 2;
                for (int i = 0; i < centrar; i++)
                {
                    espacios += " ";
                }

                line.AppendLine(espacios + text.Substring(count, text.Length - count));
            }
            else
            {
                string espacios = "";

                int centrar = (Config.MAXCAR - text.Length) / 2;

                for (int i = 0; i < centrar; i++)
                {
                    espacios += " ";
                }

                line.AppendLine(espacios + text);

            }

            return line.ToString();
        }

        public static string ToExtreme(this string textL, string textR)
        {

            string textoIzq, textoDer, complete = "", space = "";
            var line = new StringBuilder();

            if (textL.Length > Config.MAXCAR - 6)
            {
                var cut = textL.Length - Config.MAXCAR - 6;
                textoIzq = textL.Remove(Config.MAXCAR - 6, cut);
            }
            else
            { textoIzq = textL; }

            complete = textoIzq;

            if (textR.Length > 24)
            {
                var cut = textR.Length - 24;
                textoDer = textR.Remove(24, cut);
            }
            else
            { textoDer = textR; }

            int nroEspacios = Config.MAXCAR - (textoIzq.Length + textoDer.Length);
            for (int i = 0; i < nroEspacios; i++)
            {
                space += " ";
            }
            complete += space + textR;
            return line.AppendLine(complete).ToString();
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="text"></param>
        /// <param name="defaul">Guion (-) SubGuion(_) Asterisco(*) Igual(=) </param>
        /// <returns></returns>
        public static string Continuous(this string text, string defaul = " ")
        {
            if (text != "")
                defaul = text;

            var line = new StringBuilder();
            string character = "";
            for (int i = 0; i < Config.MAXCAR; i++)
            {
                character += defaul;
            }
            return line.AppendLine(character).ToString();
        }

        public static string ToLeft(this string texto)
        {
            var line = new StringBuilder();
            if (texto.Length > Config.MAXCAR)
            {
                int caracterActual = 0;
                for (int longitudTexto = texto.Length; longitudTexto > Config.MAXCAR; longitudTexto -= Config.MAXCAR)
                {
                    line.AppendLine(texto.Substring(caracterActual, Config.MAXCAR));
                    caracterActual += Config.MAXCAR;
                }
                line.AppendLine(texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                line.AppendLine(texto);
            }
            return line.ToString();
        }

        public static string ToRight(this string texto)
        {
            var line = new StringBuilder();
            if (texto.Length > Config.MAXCAR)
            {
                int caracterActual = 0;
                for (int longitudTexto = texto.Length; longitudTexto > Config.MAXCAR; longitudTexto -= Config.MAXCAR)
                {
                    line.AppendLine(texto.Substring(caracterActual, Config.MAXCAR));
                    caracterActual += Config.MAXCAR;
                }

                string space = "";

                for (int i = 0; i < (Config.MAXCAR - texto.Substring(caracterActual, texto.Length - caracterActual).Length); i++)
                {
                    space += " ";
                }

                line.AppendLine(space + texto.Substring(caracterActual, texto.Length - caracterActual));
            }
            else
            {
                string space = "";
                for (int i = 0; i < (Config.MAXCAR - texto.Length); i++)
                {
                    space += " ";
                }
                line.AppendLine(space + texto);
            }

            return line.ToString();
        }

        public static string GetHeaderDetail(this string text)
        {
            var line = new StringBuilder();
            return line.AppendLine("CANT |ITEM".ToExtreme("|PRECIO ")).ToString();
        }
    }
}