using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
namespace Hub.Controllers
{
    public class HomeController : Controller
    {
		Repository<Subscription> subs = new Repository<Subscription>();
        public ActionResult Index()
        {
			return View(Hub.Domain.Entities.User.Current.ActiveSubscriptions);
        }

        public ActionResult About()
        {
            return View();
        }

    }
}
