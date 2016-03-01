using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
using Hub.Web.Models;
namespace Hub.Controllers
{
	[Authorize(Roles = "Admin")]
	public class SubscriptionController : Controller
	{
		private Repository<Subscription> repo = new Repository<Subscription>();
		private Repository<UserSubscription> userSubscriptions = new Repository<UserSubscription>();
		private Repository<ClientSubscription> clientSubscriptions = new Repository<ClientSubscription>();
		private Repository<Client> clients = new Repository<Client>();
		private Repository<User> users = new Repository<User>();
		private Repository<Service> services = new Repository<Service>();

		public ViewResult Index(Guid? userID, int? clientID)
		{
			ViewBag.Users = new SelectList(users.All.OrderBy(e => e.Client.Name).ThenBy(e=>e.Email), "ID", "Email", userID);
			ViewBag.Clients = new SelectList(clients.All.OrderBy(e => e.Name), "ID", "Name", clientID);
			User u = users.FindBy(userID);
			Client c=null;
			if (u != null && u.ClientID !=  null)
				c = u.Client;
			if (c == null)
				c = clients.FindBy(clientID);
			clientID = c == null ? null : (int?)c.ID;

			ViewBag.CurrentUser = u;
			ViewBag.CurrentClient = c;
			var model = new SubscriptionModel
				{
					Subscriptions = repo.AllWith("Client", "User").Where(s => (userID == null || userID == s.UserID) && (clientID == null || clientID == s.ClientID)).OrderBy(e=>e.Client.Name).ThenBy(e=>e.User.LastName).ThenBy(e=>e.Name).ToList(),
					ClientID = clientID,
					UserID = userID
				};
			return View(model);
		}

		#region Create
		public ActionResult CreateUser(Guid id)
		{

            ViewBag.Services = new SelectList(users.FindBy(id).AvailableServices, "ID", "Name");
			var model = new SubscriptionModel { UserID = id, UserName = users.FindBy(id).FullName };
			//TempData["SubscriptionModel"] = model;
			return View(model);
		}

		[HttpPost]
		public ActionResult CreateUser(SubscriptionModel m, UserSubscription s)
		{
			if (ModelState.IsValid)
			{
				userSubscriptions.Add(s);
				return RedirectToAction("Index", new { userID = m.UserID });
			}
			ViewBag.Services = new SelectList(users.FindBy(m.UserID).AvailableServices, "ID", "Name");
			return View(m);
		}

		public ActionResult CreateClient(int id)
		{
			ViewBag.Services = new SelectList(clients.FindBy(id).AvailableServices, "ID", "Name");
			return View(new SubscriptionModel { ClientID = id, ClientName = clients.FindBy(id).Name });
		}

		[HttpPost]
		public ActionResult CreateClient(SubscriptionModel m, ClientSubscription s)
		{
			if (ModelState.IsValid)
			{
				clientSubscriptions.Add(s);
				return RedirectToAction("Index", new { clientID = m.ClientID } );
			}
			ViewBag.Services = new SelectList(clients.FindBy(m.ClientID).AvailableServices, "ID", "Name");
			return View(m);
		}
		#endregion

		#region Edit
		public ActionResult EditUser(Guid userID, int serviceID)
		{
			return View(userSubscriptions.All.Single(e => e.UserID == userID && e.ServiceID == serviceID));
		}

		[HttpPost]
		public ActionResult EditUser(UserSubscription s)
		{
			if (ModelState.IsValid)
			{
				userSubscriptions.Update(s);
				return RedirectToAction("Index");
			}
			return View(s);
		}

		public ActionResult EditClient(int clientID, int serviceID)
		{
			return View(clientSubscriptions.All.Single(e => e.ClientID == clientID && e.ServiceID == serviceID));
		}

		[HttpPost]
		public ActionResult EditClient(ClientSubscription s)
		{
			if (ModelState.IsValid)
			{
				clientSubscriptions.Update(s);
				return RedirectToAction("Index");
			}
			return View(s);
		}
		#endregion

		#region Delete
		public ActionResult DeleteUser(Guid userID, int serviceID)
		{
			userSubscriptions.Delete(userSubscriptions.All.Single(e => e.UserID == userID && e.ServiceID == serviceID));
			return RedirectToAction("Index", new { userID = userID });
		}

		public ActionResult DeleteClient(int clientID, int serviceID)
		{
			clientSubscriptions.Delete(clientSubscriptions.All.Single(e => e.ClientID == clientID && e.ServiceID == serviceID));
			return RedirectToAction("Index", new { clientID = clientID });
		}
		#endregion

	}
}