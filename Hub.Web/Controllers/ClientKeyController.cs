using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Web.Models;
using Hub.Domain.Repositories;
using Hub.Domain.Entities;
using System.Web.Security;

namespace Hub.Controllers
{
	[Authorize(Roles = "Admin")]
    public class ClientKeyController : Controller
    {
		Repository<ClientKey> repo = new Repository<ClientKey>();

        public ActionResult Index()
        {
            return View(new ClientKeyModel());
        }

		[HttpPost]
		public ActionResult Index(ClientKeyModel model)
		{
			model.GenerateKeys();
			return View(model);
		}

		[AllowAnonymous]
		public ActionResult Register()
		{
			return View();
		}

		[AllowAnonymous]
		[HttpPost]
		public ActionResult Register(UserModel model, Guid? key)
		{

			ClientKey clientKey = null;
			if (key == null || (clientKey = repo.FindBy(key)) == null)
				ModelState.AddModelError("", "Invalid Key");
			else if ((clientKey.InvalidDate ?? DateTime.MaxValue)< DateTime.Now)
				ModelState.AddModelError("", "Key has expired as of " + clientKey.InvalidDate.Value.ToShortDateString());
			if (ModelState.IsValid)
			{

				// Attempt to register the user
				MembershipCreateStatus createStatus;
				var user = Membership.CreateUser(model.User.Email, model.Password, model.User.Email, null, null, true, null, out createStatus);
				if (createStatus == MembershipCreateStatus.Success)
				{
					model.User.ID = (Guid)user.ProviderUserKey;
					model.User.ClientID = clientKey.ClientID;
					new Repository<User>().Add(model.User);
					repo.Delete(clientKey);
					FormsAuthentication.SetAuthCookie(model.User.Email, true);
					return RedirectToAction("Index", "Home");
				}
				else
				{
					ModelState.AddModelError("", ErrorCodeToString(createStatus));
				}
			}
			return View(model);

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


