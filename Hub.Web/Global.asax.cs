using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using MvcBase.Infrastructure;

namespace Hub
{
	// Note: For instructions on enabling IIS6 or IIS7 classic mode, 
	// visit http://go.microsoft.com/?LinkId=9394801

	public class MvcApplication : System.Web.HttpApplication
	{
		public static void RegisterGlobalFilters(GlobalFilterCollection filters)
		{
			filters.Add(new HandleErrorAttribute());
			filters.Add(new System.Web.Mvc.AuthorizeAttribute());
		}

		public static void RegisterRoutes(RouteCollection routes)
		{
			routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

			routes.MapRoute(
				name: "Default",
				url: "{controller}/{action}/{id}",
				defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional }
			);
		}

		public static void RegisterBinders(ModelBinderDictionary binders)
		{
			binders.Add(typeof(int[,]), new Binder2D<int>());
			binders.Add(typeof(double[,]), new Binder2D<double>());
			binders.Add(typeof(int[, ,]), new Binder3D<int>());
			binders.Add(typeof(double[, ,]), new Binder3D<double>());
		}

		public static void RegisterBundles(BundleCollection bundles)
		{
			//BundleTable.Bundles.RegisterTemplateBundles();
			//BundleTable.Bundles.EnableDefaultBundles();

			var bundle = new Bundle("~/Content/Styles/css", new CssMinify());
			bundle.AddDirectory("~/Content/Styles", "*.css", false);
			bundles.Add(bundle);


			bundle = new Bundle("~/Content/Styles/themes/ui-lightness/css", new CssMinify());
			bundle.AddDirectory("~/Content/Styles/themes/ui-lightness", "*.css", false);
			bundle.AddDirectory("~/Content/Styles/themes", "*.css", false);
			bundles.Add(bundle);


			bundle = new Bundle("~/Content/Scripts/js", new JsMinify());
			bundle.AddDirectory("~/Content/Scripts", "*.js", true);
			bundles.Add(bundle);
		}

		protected void Application_Start()
		{
			AreaRegistration.RegisterAllAreas();

			ModelMetadataProviders.Current = new ConventionModelMetadataProvider();

			RegisterBinders(ModelBinders.Binders);
			RegisterGlobalFilters(GlobalFilters.Filters);
			RegisterRoutes(RouteTable.Routes);
			RegisterBundles(BundleTable.Bundles);
		}
	}
}