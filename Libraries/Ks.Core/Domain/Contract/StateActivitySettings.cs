using System;
using Ks.Core.Configuration;

namespace Ks.Core.Domain.Contract
{
    public class StateActivitySettings : ISettings
    {
        public string Periods { get; set; }
        public double Tea { get; set; }
        public double Safe { get; set; }

        #region Clase A

        public bool IsEnable1 { get; set; }
        public string StateName1 { get; set; }
        public int MinClycle1 { get; set; }
        public int MaxClycle1 { get; set; }
        public bool HasOnlySignature1 { get; set; }
        public decimal MinAmountWithSignature1 { get; set; }
        public decimal MaxAmountWithSignature1 { get; set; }
        public bool HasWarranty1 { get; set; }
        public decimal MinAmountWithWarranty1 { get; set; }
        public decimal MaxAmountWithWarranty1 { get; set; }

        #endregion

        #region Clase B

        public bool IsEnable2 { get; set; }
        public string StateName2 { get; set; }
        public int MinClycle2 { get; set; }
        public int MaxClycle2 { get; set; }
        public bool HasOnlySignature2 { get; set; }
        public decimal MinAmountWithSignature2 { get; set; }
        public decimal MaxAmountWithSignature2 { get; set; }
        public bool HasWarranty2 { get; set; }
        public decimal MinAmountWithWarranty2 { get; set; }
        public decimal MaxAmountWithWarranty2 { get; set; }

        #endregion

        #region Clase C

        public bool IsEnable3 { get; set; }
        public string StateName3 { get; set; }
        public int MinClycle3 { get; set; }
        public int MaxClycle3 { get; set; }
        public bool HasOnlySignature3 { get; set; }
        public decimal MinAmountWithSignature3 { get; set; }
        public decimal MaxAmountWithSignature3 { get; set; }
        public bool HasWarranty3 { get; set; }
        public decimal MinAmountWithWarranty3 { get; set; }
        public decimal MaxAmountWithWarranty3 { get; set; }

        #endregion

        #region Clase D

        public bool IsEnable4 { get; set; }
        public string StateName4 { get; set; }
        public int MinClycle4 { get; set; }
        public int MaxClycle4 { get; set; }
        public bool HasOnlySignature4 { get; set; }
        public decimal MinAmountWithSignature4 { get; set; }
        public decimal MaxAmountWithSignature4 { get; set; }
        public bool HasWarranty4 { get; set; }
        public decimal MinAmountWithWarranty4 { get; set; }
        public decimal MaxAmountWithWarranty4 { get; set; }

        #endregion

        #region Clase E

        public bool IsEnable5 { get; set; }
        public string StateName5 { get; set; }
        public int MinClycle5 { get; set; }
        public int MaxClycle5 { get; set; }
        public bool HasOnlySignature5 { get; set; }
        public decimal MinAmountWithSignature5 { get; set; }
        public decimal MaxAmountWithSignature5 { get; set; }
        public bool HasWarranty5 { get; set; }
        public decimal MinAmountWithWarranty5 { get; set; }
        public decimal MaxAmountWithWarranty5 { get; set; }

        #endregion

        
        public string CashFlow { get; set; }
    }
}