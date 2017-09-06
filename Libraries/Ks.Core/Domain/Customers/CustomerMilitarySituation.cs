using System.ComponentModel;

namespace Ks.Core.Domain.Customers
{
    public enum CustomerMilitarySituation
    {
        [Description("Actividad - COPERE (8001)")]
        Actividad = 1,  
        [Description("Retiro - CPMP (6008)")]
        Retiro = 2,  
        [Description("Personal")]
        Personal = 3,
        [Description("InfoCorp")]
        InfoCorp = 4,
        [Description("Goce de Beneficio")]
        Gozado=5,
        [Description("Personal Interno")]
        Interno = 6,
        [Description("Renuncia")]
        Renuncia = 7
    }
}