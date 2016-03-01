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
	[Authorize(Roles="Admin")]
    public class ServiceTypeController : Controller
    {
		private Repository<ServiceType> repo = new Repository<ServiceType>();

        //
        // GET: /ServiceType/

        public ViewResult Index()
        {
            return View(repo.All.OrderBy(e=>e.Name).ThenBy(e=>e.IsDocument));
        }

        //
        // GET: /ServiceType/Details/5

        public ViewResult Details(int id)
        {
            ServiceType servicetype = repo.FindBy(id);
            return View(servicetype);
        }

        //
        // GET: /ServiceType/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /ServiceType/Create

        [HttpPost]
        public ActionResult Create(ServiceType servicetype)
        {
            if (ModelState.IsValid)
            {
                repo.Add(servicetype);
                return RedirectToAction("Index");  
            }

            return View(servicetype);
        }
        
        //
        // GET: /ServiceType/Edit/5
 
        public ActionResult Edit(int id)
        {
            ServiceType servicetype = repo.FindBy(id);
            return View(servicetype);
        }

        //
        // POST: /ServiceType/Edit/5

        [HttpPost]
        public ActionResult Edit(ServiceType servicetype)        
		{
            if (ModelState.IsValid)
            {
                repo.Update(servicetype);
                return RedirectToAction("Index");
            }
            return View(servicetype);
        }

		public ActionResult Delete(int id)
		{
			repo.Delete(repo.FindBy(id));
			return RedirectToAction("Index");
		}
    }
}