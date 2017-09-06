namespace Ks.Core.Domain.Contract
{
    public enum ContributionState
    {
        Pendiente = 1,
        EnProceso = 2,
        PagoParcial = 3,
        Pagado = 4,
        SinLiquidez = 5,
        Devolucion = 6,
        PagoPersonal = 7 
    }
}