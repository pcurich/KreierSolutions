using System.ComponentModel;

namespace Ks.Core.Domain.Contract
{
    public enum EntityTypeValues
    {
        [Description("Apoyo Social Económico")]
        Loan=1,
        [Description("Devolución")]
        Return=2,
        [Description("Beneficio")]
        Benefit=3
    }
}