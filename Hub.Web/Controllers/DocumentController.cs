using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Hub.Web.Models;
using System.IO;
using Hub.Domain.Repositories;
using Hub.Domain.Entities;
using System.Text.RegularExpressions;
namespace Hub.Controllers
{
    public class DocumentController : Controller
    {
		Repository<Subscription> Subs = new Repository<Subscription>();

		Documents docs = new Documents(Hub.Domain.Entities.User.Current);
        public ActionResult Index()
        {
            return View(docs);
        }

		public ActionResult Download(string path)
		{
			if (!docs.IsAllowed(path)) return new HttpUnauthorizedResult();
			var contentType = getContentTypeFromExtension(path);
			return File(path, contentType, Path.GetFileName(path));
		}

		public ActionResult Open(string path)
		{
			if (!docs.IsAllowed(path)) return new HttpUnauthorizedResult();
			var contentType = getContentTypeFromExtension(path);
			return File(path, contentType);
		}

		//method to get content type of file from registry using file extension
		static string getContentTypeFromExtension(string path)
		{
			string contentType = "application/unknown";
			string ext = System.IO.Path.GetExtension(path).ToLower();
			Microsoft.Win32.RegistryKey regKey = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(ext);
			if (regKey != null && regKey.GetValue("Content Type") != null)
				contentType = regKey.GetValue("Content Type").ToString();
			return contentType;
		}

		[Authorize(Roles="Admin")]
		public ActionResult Sync()
		{
			return View(docs);
		}

		[Authorize(Roles = "Admin")]
		[HttpPost]
		public ActionResult Sync(bool[] delete, string[] deletePath, bool[] create, string[] createPath)
		{
			List<string> deletes = GetPathsForSync(delete, deletePath);
			List<string> creates = GetPathsForSync(create, createPath);

			Documents.CreateServicesAndTypes(creates);
			Documents.DeleteServicesAndTypes(deletes);

			docs = new Documents(Hub.Domain.Entities.User.Current);
			return RedirectToAction("Sync");
		}

		List<string> GetPathsForSync(bool[] checks, string[] paths)
		{
			List<string> stuff = new List<string>();
			if (paths == null) return stuff;
			for (int i = 0, j=0; i < paths.Count(); ++i, ++j)
			{
				if (checks[j])
				{
					stuff.Add(paths[i]); 
					++j; // add j because whenever a true is in the array a false is also added right after.
				}
			}
			return stuff.Distinct().ToList();
		}
    }
}
