using System.Web.Mvc;

namespace Ks.Web.Framework.Mvc
{
    public class KsModelBinder : DefaultModelBinder
    {
        public override object BindModel(ControllerContext controllerContext, ModelBindingContext bindingContext)
        {
            var model = base.BindModel(controllerContext, bindingContext);
            if (model is BaseKsModel)
            {
                ((BaseKsModel)model).BindModel(controllerContext, bindingContext);
            }
            return model;
        }
    }
}
