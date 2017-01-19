using System.ComponentModel;

namespace Ks.Core.Domain.Messages
{
    public enum WorkFlowType
    {
        [Description("Aportaciones")]
        Contribution=1,
        [Description("Apoyo Social Económico")]
        Loan = 2,
        [Description("Beneficios")]
        Benefit = 3

    }
}