using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
using Hub.Web.Models;

namespace Hub.Controllers
{ 
	[Authorize(Roles="Admin")]
    public class UserController : Controller
    {
		private Repository<Client> Clients = new Repository<Client>();
		private Repository<User> Users = new Repository<User>();
        //
        // GET: /Admin/

        public ViewResult Index()
        {
            return View(Users.All.OrderBy(e=>e.Client.Name).ThenBy(e=>e.LastName));
        }

        //
        // GET: /Admin/Create

        public ActionResult Register()
        {
			ViewBag.ClientList = new SelectList(Clients.All, "ID", "Name");
			ViewBag.RoleList = new SelectList(Roles.GetAllRoles());
            return View();
        } 

        //
        // POST: /Admin/Create

		[HttpPost]
		public ActionResult Register(UserModel model)
		{
			if (ModelState.IsValid)
			{
				// Attempt to register the user
				MembershipCreateStatus createStatus;
				var user = Membership.CreateUser(model.User.Email, model.Password, model.User.Email, null, null, true, null, out createStatus);
				if (createStatus == MembershipCreateStatus.Success)
				{
					model.User.SaveRoles();
					model.User.ID = (Guid)user.ProviderUserKey;
					Users.Add(model.User);
					return RedirectToAction("Register");
				}
				else
				{
					ModelState.AddModelError("", ErrorCodeToString(createStatus));
				}
			}

			// If we got this far, something failed, redisplay form
			ViewBag.ClientList = new SelectList(Clients.All, "ID", "Name", model.User.ClientID);
			ViewBag.RoleList = new SelectList(Roles.GetAllRoles());
			return View(model);
		}


        //
        // GET: /Admin/Edit/5
 
        public ActionResult Edit(Guid id)
        {
            User user = Users.FindBy(id);
            ViewBag.ClientID = new SelectList(Clients.All, "ID", "Name", user.ClientID);
			ViewBag.RoleList = new SelectList(Roles.GetAllRoles(), user.Roles);
			ViewBag.Pass = new ChangePasswordModel() { UserID = id };
			return View(user);
        }

        //
        // POST: /Admin/Edit/5

        [HttpPost]
        public ActionResult Edit(User user)
        {
            if (ModelState.IsValid)
            {
				Users.Update(user);
				user.SaveRoles();
                return RedirectToAction("Index");
            }
            ViewBag.ClientID = new SelectList(Clients.All, "ID", "Name", user.ClientID);
			ViewBag.RoleList = new SelectList(Roles.GetAllRoles(), user.Roles);
			ViewBag.Pass = new ChangePasswordModel() { UserID = user.ID };
			return View(user);
        }


		[HttpPost]
		public ActionResult UpdatePassword(ChangePasswordModel pass)
		{
			if (User.IsInRole("Demo")) return new HttpUnauthorizedResult();
			
			if (pass.ConfirmPassword == pass.NewPassword && pass.ConfirmPassword.Length > 5)
			{

				// ChangePassword will throw an exception rather
				// than return false in certain failure scenarios.
				bool changePasswordSucceeded;
				try
				{
					MembershipUser currentUser = Membership.GetUser(pass.UserID);
					string password = currentUser.ResetPassword();
					changePasswordSucceeded = currentUser.ChangePassword(password, pass.ConfirmPassword);
				}
				catch (Exception)
				{
					changePasswordSucceeded = false;
				}
				if (!changePasswordSucceeded)
				{
					ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
				}
				else
					return RedirectToAction("Index");
			}
			return RedirectToAction("Edit", new { id = pass.UserID });
		}
       
        public ActionResult Delete(Guid id)
        {            
            User user = Users.FindBy(id);
            Users.Delete(user);
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            //db.Dispose();
            base.Dispose(disposing);
        }


		private static string ErrorCodeToString(MembershipCreateStatus createStatus)
		{
			// See http://go.microsoft.com/fwlink/?LinkID=177550 for
			// a full list of status codes.
			switch (createStatus)
			{
				case MembershipCreateStatus.DuplicateUserName:
					return "User name already exists. Please enter a different user name.";

				case MembershipCreateStatus.DuplicateEmail:
					return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

				case MembershipCreateStatus.InvalidPassword:
					return "The password provided is invalid. Please enter a valid password value.";

				case MembershipCreateStatus.InvalidEmail:
					return "The e-mail address provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidAnswer:
					return "The password retrieval answer provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidQuestion:
					return "The password retrieval question provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.InvalidUserName:
					return "The user name provided is invalid. Please check the value and try again.";

				case MembershipCreateStatus.ProviderError:
					return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				case MembershipCreateStatus.UserRejected:
					return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

				default:
					return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
			}
		}

    }
}