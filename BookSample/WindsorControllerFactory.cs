﻿using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.SessionState;
using Castle.Windsor;

namespace Ioc_Windsor
{
    public class WindsorControllerFactory : DefaultControllerFactory
    {
        protected override IController GetControllerInstance(RequestContext requestContext, Type controllerType)
        {
            if (controllerType == null)
            {
                throw new HttpException(404, string.Format("The controller for path '{0}' could not be found.", requestContext.HttpContext.Request.Path));
            }

            var container = GetContainer(requestContext);

            return (IController)container.Resolve(controllerType);
        }

        public SessionStateBehavior GetControllerSessionBehavior(RequestContext requestContext, string controllerName)
        {
            return SessionStateBehavior.Default;
        }

        private IWindsorContainer GetContainer(RequestContext context)
        {
            var accessor = context.HttpContext.ApplicationInstance as IContainerAccessor;
            if (accessor == null)
                throw new InvalidOperationException(
                    "The Global Application class must implmement IContainerAccessor");

            return accessor.Container;
        }

        public void DisposeController(IController controller)
        {
            var disposable = controller as IDisposable;
            if (disposable != null)
                disposable.Dispose();
        }

        public override void ReleaseController(IController controller)
        {
            //throw new NotImplementedException();
        }
    }

}