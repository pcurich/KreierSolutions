using System;

namespace Ks.Admin.Models.Contract
{
    public class ContributionListModel
    {
        public int SearchLetter { get; set; }
        public string SearchDni { get; set; }
        public string SearchAdmCode { get; set; }
        public DateTime? SearchDateCreatedFrom { get; set; }
        public DateTime? SearchDateCreatedTo { get; set; }
        public bool IsActive { get; set; }
    }
}