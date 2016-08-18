using Ks.Web.Framework.Mvc;

namespace Ks.Admin.Models.Security
{
    public partial class PermissionRecordModel : BaseKsModel
    {
        public string Name { get; set; }
        public string SystemName { get; set; }
    }
}