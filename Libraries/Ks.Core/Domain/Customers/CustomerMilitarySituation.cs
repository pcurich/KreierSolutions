using System.ComponentModel;

namespace Ks.Core.Domain.Customers
{
    public enum CustomerMilitarySituation
    {
        [Description("Actividad - COPERE (8001)")]
        Actividad = 1, //Go to COPERE
        [Description("Retiro - CPMP (6008)")]
        Retiro = 2, //Go To Cash
        [Description("Personal")]
        Personal = 3,
        [Description("InfoCorp")]
        InfoCorp = 4
    }
}