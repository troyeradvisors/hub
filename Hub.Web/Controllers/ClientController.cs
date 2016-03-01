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
    public class ClientController : Controller
    {
        private Repository<Client> repo = new Repository<Client>();

        //
        // GET: /Client/

        public ViewResult Index()
        {
            return View(repo.All.OrderBy(e=>e.Name));
        }


        //
        // GET: /Client/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Client/Create

        [HttpPost]
        public ActionResult Create(Client client)
        {
            if (ModelState.IsValid)
            {
                repo.Add(client);
                return RedirectToAction("Index");  
            }

            return View(client);
        }
        
        //
        // GET: /Client/Edit/5
 
        public ActionResult Edit(int id)
        {
            Client client = repo.FindBy(id);
            return View(client);
        }

        //
        // POST: /Client/Edit/5

        [HttpPost]
        public ActionResult Edit(Client client)
        {
            if (ModelState.IsValid)
            {
                repo.Update(client);
                return RedirectToAction("Index");
            }
            return View(client);
        }

        public ActionResult Delete(int id)
        {
            Client client = repo.FindBy(id);
            repo.Delete(client);
            return RedirectToAction("Index");
        }

    }
}