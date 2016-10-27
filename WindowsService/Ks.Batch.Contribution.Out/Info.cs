 
namespace Ks.Batch.Contribution.Out
{
    public class Info
    {
        public int Year { get; set; } // CHAR(1) Tipo de descuento: 8 para todos los registros
        public int  Month { get; set; } // CHAR(1) Tipo de Movimiento ABC
        public string AdminCode { get; set; } // CHAR(9) Numero Administrativo
        public decimal Total { get; set; } // CHAR(4) Código de descuento
    }
}
