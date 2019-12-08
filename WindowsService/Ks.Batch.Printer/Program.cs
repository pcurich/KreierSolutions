using System;
using System.Collections.Generic;
using System.Text;

namespace Ks.Batch.Printer
{
    class Program
    {
        static void Main(string[] args) {
            var str =   "Turno: M  Ticket # 001-00000001  " +
                        "*********************************" +
                        "                                 " +
                        "Atendio: Darwin                  " +
                        "Cliente: Pedro Curich Gonzales   " +
                        "                                 " +
                        "Fecha: 24/11/2018  Hora: 14:25:44" +
                        "*********************************" +
                        "ARTICULO     |CANT|PRECIO|IMPORTE" +
                        "*********************************" +
                        "Mocaccino         2  8.00   15.00" +
                        "Latte 8onz        2  8.00   15.00" +
                        "Americano 16 onz  2  8.00   15.00" +
                        "=================================" +
                        "         SUBTOTAL .... S/ 1000.00" +
                        "         IGV ......... S/   15.00" +
                        "         TOTAL ....... S/  200.00" +
                        "                                 " +
                        "         EFECTIVO .... S/  150.00" +
                        "         CAMBIO ...... S/   60.00" +
                        "                                 " +
                        "ARTICULOS VENDIDOS: 3            ";
            Tickets.Print(str, "ZJ-58");
        }
    }
}
