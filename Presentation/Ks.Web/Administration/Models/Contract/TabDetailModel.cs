using System;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class TabDetailModel : BaseKsEntityModel
    {
        public int YearInActivity { get; set; }
        public double Value { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; }
    }
}