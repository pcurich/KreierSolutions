namespace Ks.Batch.Contribution.Copere.Out
{
    public class Info
    {
        public string Tdesc { get; set; } // CHAR(1) Tipo de descuento: 8 para todos los registros
        public string Tmov { get; set; } // CHAR(1) Tipo de Movimiento ABC
        public string NuAdmi { get; set; } // CHAR(9) Numero Administrativo
        public string Codigo { get; set; } // CHAR(4) Código de descuento
        public decimal MontoDesc { get; set; } // Numerico(13,2) Monto del descuento con 2 decimales
        public decimal MontoAnt { get; set; } // Numerico(13,2) Monto del descuento anterior con 2 decimales solo para movimientos de tipo C
        public int NuCtas { get; set; } // Numerico(3) N° de cuotas del descuento    
        public string CodAct { get; set; } //CHAR(4) Codigo de actividad (dejar en blanco)
        public string DocFuent { get; set; } //CHAR(4) N° del documento fuente (dejar en blanco)
        public string MesProc { get; set; } //CHAR(6) Año y Mes de proceso AAAAMM

    }
}
