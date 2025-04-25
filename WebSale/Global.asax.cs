using Autofac;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebSale.App_Start;
using WebSale.Repository;

namespace WebSale
{
    public class MvcApplication : System.Web.HttpApplication
    {
        public static ConnectionMultiplexer Redis { get; private set; }
        protected void Application_Start()
        {

            //Redis = ConnectionMultiplexer.Connect("localhost:6380");
            GlobalConfiguration.Configure(WebApiConfig.Register);
            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            var emailSender = new EmailSender();
        }
    }
}
