using System;
using System.Collections.Generic;
using System.Linq;
using Ks.Core.Domain.Contract;
using Ks.Core.Infrastructure;
using Ks.Services.Contract;

namespace Ks.Admin.Extensions
{
    public static class BenefitHelper
    {
        public static ContributionBenefit CreateBenefit(this Contribution contribution,
            Benefit benefit, decimal amountBaseOfBenefit, int numberOfLiquidation, int year)
        {
            var amounLoan = 0m;
            var loans = EngineContext.Current.Resolve<ILoanService>().GetLoansByCustomer(contribution.CustomerId).Where(x => x.Active).ToList();
            var tabDetail = EngineContext.Current.Resolve<ITabService>().GetValueFromActive(35);

            var contributions = EngineContext.Current.Resolve<IContributionService>().GetPaymentByContributionId(contribution.Id);
            var amountTotal = contributions.Sum(x => x.AmountPayed);
            var amountCaja = contributions.Where(x => x.BankName == "Caja").Sum(x => x.AmountPayed);
            var amountCopere = contributions.Where(x => x.BankName == "Copere").Sum(x => x.AmountPayed);
            
            var model = new ContributionBenefit();
            
            if (benefit.CancelLoans)
            {
                foreach (var loan in loans)
                {
                    amounLoan += loan.TotalAmount - loan.TotalPayed;
                }

                model.TotalLoan = loans.Count;
                model.TotalLoanToPay = amounLoan;
            }

            model.BenefitId = benefit.Id;
            model.ContributionId = contribution.Id;
            model.NumberOfLiquidation = numberOfLiquidation;
            model.AmountBaseOfBenefit = amountBaseOfBenefit;
            model.YearInActivity = year;
            model.TabValue = tabDetail != null ? tabDetail.TabValue : 0;
            model.Discount = benefit.Discount;
            model.TotalContributionCopere = amountCopere;
            model.TotalContributionCaja = amountCaja;
            model.TotalContributionPersonalPayment = amountTotal - amountCaja - amountCopere;

            model.SubTotalToPay = (decimal)benefit.Discount *
                                amountBaseOfBenefit * amounLoan *
                                (tabDetail != null ? (decimal)tabDetail.TabValue : 0);

            model.ReserveFund = model.SubTotalToPay - model.TotalContributionCaja - model.TotalContributionCopere -
                                   model.TotalContributionPersonalPayment;
            
            model.CustomField1 = benefit.CustomField1;
            model.CustomField2 = benefit.CustomField2;
            model.TotalToPay = model.SubTotalToPay - model.TotalLoanToPay;
            model.CreatedOnUtc = DateTime.UtcNow;


            return model;
        }

    }
}