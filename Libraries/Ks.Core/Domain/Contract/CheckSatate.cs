using System.ComponentModel;

namespace Ks.Core.Domain.Contract
{
    public enum CheckSatate
    {
        [Description("Vigente")]
        Active = 1,
        [Description("Anulado")]
        Cancel = 2, 
    }
}