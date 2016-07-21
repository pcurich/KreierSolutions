namespace Ks.Web.Framework.Mvc
{
    public class DeleteConfirmationModel : BaseKsEntityModel
    {
        public string ControllerName { get; set; }
        public string ActionName { get; set; }
        public string WindowId { get; set; }
    }
}