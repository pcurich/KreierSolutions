using System;
using System.Collections.Generic;
using Ks.Core;
using Ks.Core.Domain.Contract;

namespace Ks.Services.Contract
{
    public interface IBenefitService
    {
        #region Benefit
        void DeleteBenefit(Benefit benefit);
        void InsertBenefit(Benefit benefit);
        void UpdateBenefit(Benefit benefit);
        Benefit GetBenefitById(int benefitId);
        List<Benefit> GetAllBenefits();
        IPagedList<Benefit> SearchBenefits(int pageIndex = 0, int pageSize = Int32.MaxValue);
        #endregion
    }
}