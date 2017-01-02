using System.Collections.Generic;
using System.Web.Mvc;
using Ks.Core.Domain.Contract;

namespace Ks.Admin.Extensions
{
    public static class BankHelper
    {
        public static List<SelectListItem> PrepareBanks(this BankSettings bankSettings)
        {
            var model = new List<SelectListItem>();
            model.Insert(0, new SelectListItem { Value = "0", Text = "----------------" });

            if (bankSettings.IsActive1)
                model.Add(new SelectListItem { Value = bankSettings.AccountNumber1, Text = bankSettings.NameBank1 });
            if (bankSettings.IsActive2)
                model.Add(new SelectListItem { Value = bankSettings.AccountNumber2, Text = bankSettings.NameBank2 });
            if (bankSettings.IsActive3)
                model.Add(new SelectListItem { Value = bankSettings.AccountNumber3, Text = bankSettings.NameBank3 });
            if (bankSettings.IsActive4)
                model.Add(new SelectListItem { Value = bankSettings.AccountNumber4, Text = bankSettings.NameBank4 });
            if (bankSettings.IsActive5)
                model.Add(new SelectListItem { Value = bankSettings.AccountNumber5, Text = bankSettings.NameBank5 });

            return model;
        }
    }
}