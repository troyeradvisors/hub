using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using Hub.Web.Models;
using Hub.Domain.Repositories;
using Hub.Domain.Entities;

namespace Hub.Controllers
{
    public class ProfileController : Controller
    {
		Repository<User> repo = new Repository<User>();


		[AllowAnonymous]
		public ActionResult Home()
		{
			return Redirect("http://troyeradvisors.com");
		}

		[AllowAnonymous]
        public ActionResult Login()
        {
            return View();
        }

		[AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LogOnModel model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (Membership.ValidateUser(model.UserName, model.Password))
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, model.Save);
					//if (Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
					//	&& !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    if (returnUrl != null) {
                        return Redirect(returnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Index", "Home");
        }
  
        public ActionResult Index()
        {
			if (User.IsInRole("Demo")) return new HttpUnauthorizedResult();

			ViewBag.User = Hub.Domain.Entities.User.Current;
			ViewBag.Pass = new ChangePasswordModel();
			return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult UpdatePassword(ChangePasswordModel pass)
        {
			if (User.IsInRole("Demo")) return new HttpUnauthorizedResult();

            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                    changePasswordSucceeded = currentUser.ChangePassword(pass.OldPassword, pass.NewPassword);
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

			ViewBag.User = Hub.Domain.Entities.User.Current;
			ViewBag.Pass = new ChangePasswordModel();
			return View();
        }


		[HttpPost]
		public ActionResult UpdateProfile(User user)
		{
			if (User.IsInRole("Demo")) return new HttpUnauthorizedResult();

			if (ModelState.IsValid)
			{
				bool sameUserName = User.Identity.Name.Equals(user.Email, StringComparison.InvariantCultureIgnoreCase);
				repo.Update(user);
				if (!sameUserName)
					return RedirectToAction("Logout");
				return RedirectToAction("Index");
			}
			return View();
		}

    }
}
