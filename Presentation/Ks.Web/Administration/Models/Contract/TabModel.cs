using System;
using Ks.Core.Domain.Contract;
using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Contract
{
    public class TabModel : BaseKsEntityModel
    {
        public string Name { get; set; }
        public decimal BaseAmount { get; set; }
        public bool Published { get; set; }
        public int DisplayOrder { get; set; }
        public DateTime CreatedOn { get; set; }
        public DateTime UpdatedOn { get; set; } 
    }
}