﻿using System;
using System.Web;
using System.Web.Mvc;
using Ks.Core.Data;
using Ks.Core.Infrastructure;
using Ks.Services.Security;

namespace Ks.Web.Framework
{
    public class PublicSystemAllowNavigationAttribut: ActionFilterAttribute
    {
        private readonly bool _ignore;

        /// <summary>
        /// Contructor
        /// </summary>
        /// <param name="ignore">Pass false in order to ignore this functionality for a certain action method</param>
        public PublicSystemAllowNavigationAttribut(bool ignore = false)
        {
            this._ignore = ignore;
        }
        
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext == null || filterContext.HttpContext == null)
                return;

            //search the solution by "[PublicStoreAllowNavigation(true)]" keyword 
            //in order to find method available even when a store is closed
            if (_ignore)
                return;

            HttpRequestBase request = filterContext.HttpContext.Request;
            if (request == null)
                return;

            string actionName = filterContext.ActionDescriptor.ActionName;
            if (String.IsNullOrEmpty(actionName))
                return;

            string controllerName = filterContext.Controller.ToString();
            if (String.IsNullOrEmpty(controllerName))
                return;

            //don't apply filter to child methods
            if (filterContext.IsChildAction)
                return;

            if (!DataSettingsHelper.DatabaseIsInstalled())
                return;
            
            var permissionService = EngineContext.Current.Resolve<IPermissionService>();
            var publicStoreAllowNavigation = permissionService.Authorize(StandardPermissionProvider.PublicStoreAllowNavigation);
            if (publicStoreAllowNavigation)
                return;

            filterContext.Result = new HttpUnauthorizedResult();
        }
    }
}
