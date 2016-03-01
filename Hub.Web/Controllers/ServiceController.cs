using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
using System.Web.Security;

namespace Hub.Controllers
{
	[Authorize(Roles = "Admin")]
    public class ServiceController : Controller
    {
        private Repository<Service> repo = new Repository<Service>();
		Repository<ServiceType> types = new Repository<ServiceType>();

        public ViewResult Index()
        {
            return View(repo.All.OrderBy(e=>e.ServiceType.Name).ThenBy(e=>e.Name));
        }

        public ActionResult Create()
        {
			ViewBag.TypeList = new SelectList(types.All.OrderBy(e=>e.Name), "ID", "Name");
            return View();
        } 

        [HttpPost]
        public ActionResult Create(Service service)
        {
            if (ModelState.IsValid)
            {
                repo.Add(service);
                return RedirectToAction("Index");  
            }

			ViewBag.TypeList = new SelectList(types.All.OrderBy(e=>e.Name), "ID", "Name");
            return View(service);
        }
        
        public ActionResult Edit(int id)
        {
            Service service = repo.FindBy(id);
			ViewBag.TypeList = new SelectList(types.All.OrderBy(e=>e.Name), "ID", "Name");
			return View(service);
        }

        [HttpPost]
        public ActionResult Edit(Service service)
        {
            if (ModelState.IsValid)
            {
                repo.Update(service);
                return RedirectToAction("Index");
            }
			ViewBag.TypeList = new SelectList(types.All.OrderBy(e=>e.Name), "ID", "Name");
            return View(service);
        }

        public ActionResult Delete(int id)
        {            
            Service service = repo.FindBy(id);
            repo.Delete(service);
            return RedirectToAction("Index");
        }

    }
}