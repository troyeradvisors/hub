using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
using System.IO;
using System.Text.RegularExpressions;

namespace Hub.Web.Models
{
	public class Sync
	{
		//public string IsChecked;
		public string Path;
	}

	public class Documents
	{
		public static Repository<ServiceType> RTypes = new Repository<ServiceType>();
        public static Repository<Service> RServices = new Repository<Service>();
	
		static string[] Filter = { "_notes", ".DS_Store" };
		static string[] FilterExtensions = { ".php", ".html" };
		List<string> AllowedDirectories;
		public List<string> PhysicalDirectories;
		public List<string> UsedDirectories;
		public List<string> ValidDirectories;
		public List<string> UnusedDirectories;
		public List<string> MissingDirectories;
		
		public Documents(User user)
		{
			Service[] services = new Repository<Subscription>().AllWith("Service").Where(s => s.User.ID == user.ID && s.Service.ServiceType.IsDocument).Select(s => s.Service).ToArray();
			AllowedDirectories = services.Select(s => s.DocumentPath).ToList();
        
			DirectoryInfo dir = new DirectoryInfo(Service.DocumentRoot);
			PhysicalDirectories = dir.EnumerateDirectories("*", SearchOption.AllDirectories).Select(e => e.FullName).Where(e => Filter.All(f => !e.ToLower().Contains(f.ToLower()))).ToList();

			var usedTypes = RTypes.FilterBy(e=>e.IsDocument).Select(e => Service.DocumentRoot + "\\" + e.Name).ToList();
			var usedServices = RServices.AllWith("ServiceType").Where(e => e.ServiceType.IsDocument).AsEnumerable().Select(e => e.DocumentPath).ToList();
			UsedDirectories = usedTypes.Union(usedServices).ToList();
			ValidDirectories = PhysicalDirectories.Where(e=>usedTypes.Any(f=> f.Equals(e, StringComparison.InvariantCultureIgnoreCase)) || usedServices.Any(f=> e.StartsWith(f, StringComparison.InvariantCultureIgnoreCase) || f.StartsWith(e, StringComparison.InvariantCultureIgnoreCase))).ToList();

			UnusedDirectories = PhysicalDirectories.Except(ValidDirectories).OrderBy(e=>e).ToList();
			MissingDirectories = UsedDirectories.Except(ValidDirectories).OrderBy(e=>e).ToList();
		}

		public Result FindAllowedFiles(DirectoryInfo current = null, bool isService = false)
		{
			current = current ?? new DirectoryInfo(Service.DocumentRoot); 
			Result files = new Result();
			files.IsService = isService || IsAllowed(current.FullName);

			if (files.IsService) // everything is allowed
				return new Result { IsService = true, Directories = current.GetDirectories().Where(x => Filter.All(filter => filter.ToLower() != x.Name.ToLower())).ToList(), Files = current.GetFiles().Where(x => Filter.All(filter => filter.ToLower() != x.Name.ToLower())).Where(x=> FilterExtensions.All(filter => filter.ToLower() != x.Extension.ToLower())).ToList() };

			files.Directories = FilterDirectories(current, AllowedDirectories);
			return files;
		}

		public List<string> NavigateMissing(string current = null)
		{
			current = current ?? Service.DocumentRoot;
			return NavigatePaths(current, MissingDirectories);
		}

		public List<string> NavigateUnused(string current = null)
		{
			current = current ?? Service.DocumentRoot;
			return NavigatePaths(current, UnusedDirectories);
		}

		public static List<DirectoryInfo> FilterDirectories(DirectoryInfo current, List<string> paths)
		{
			return current.GetDirectories().Where(dir => paths.Any(p => p.StartsWith(dir.FullName, StringComparison.InvariantCultureIgnoreCase) || dir.FullName.StartsWith(p, StringComparison.InvariantCultureIgnoreCase)) ).ToList();
		}

		public static List<String> NavigatePaths(string current, List<string> paths)
		{
			string[] p = paths.Where(pp => pp.StartsWith(current, StringComparison.InvariantCultureIgnoreCase)).ToArray();
			List<string> next = new List<string>();
			foreach (string s in p)
			{
				Match match = Regex.Match(s, @"("+Regex.Escape(current.Trim('\\')) + @"\\[\s\S]+?)(\\|$)");
				if (match.Success)
					next.Add(Path.Combine(current, match.Value.TrimEnd('\\')));
			}
			return next.Distinct().ToList();
		}

		public bool IsAllowed(string path)
		{
			return AllowedDirectories.Any(d => path.StartsWith(d, StringComparison.InvariantCultureIgnoreCase));
		}

		public class Result
		{
			public List<DirectoryInfo> Directories = new List<DirectoryInfo>();
			public List<FileInfo> Files = new List<FileInfo>();
			public bool IsService; // This means everything is allowed in this subdirectory
		}

		public static string ConvertToURL(string path)
		{
			if (HttpContext.Current.Request.IsLocal)
				return path;

			return path.ToLower().Replace(Service.DocumentRoot.ToLower(), Service.ServerRoot);
		}


		public static void CreateServicesAndTypes(List<string> paths)
		{
			var creates = ParseServicesAndTypes(paths);
			
			foreach (var d in creates)
			{
				string type = d.Item1;
				ServiceType serviceType = RTypes.All.FirstOrDefault(e => e.Name.ToLower() == type.ToLower());
				if (serviceType == null)
				{
					serviceType = new ServiceType() { IsDocument = true, Name = type };
					RTypes.Add(serviceType);
				}

				string service = d.Item2;
				if (service != "")
					RServices.Add(new Service() { Name = service, TypeID = serviceType.ID });
			}
		}

		public static void DeleteServicesAndTypes(List<string> paths)
		{
			var deletes = ParseServicesAndTypes(paths);
			
			foreach (var d in deletes)
			{
				string type = d.Item1;
				string service = d.Item2;
				
				ServiceType serviceType = RTypes.All.FirstOrDefault(e => e.Name.ToLower() == type.ToLower());
				if (service == "" && deletes.Where(e => e.Item1 == type).Count() == 1)
					RTypes.Delete(serviceType);

				if (service != "")
				{
					var s = RServices.All.FirstOrDefault(e => e.TypeID == serviceType.ID && e.Name.ToLower() == service.ToLower());
					RServices.Delete(s);
				}
			}
		}

		static List<Tuple<string, string>> ParseServicesAndTypes(List<string> paths)
		{
			var stuff = new List<Tuple<string,string>>();
			return paths.Select(path =>
				{
					var x = path.Replace(Service.DocumentRoot, "").Trim('\\');
					var y = x.Split('\\');
					return Tuple.Create(y[0], (y.Count()>1?y[1]:"")); 
				}).Distinct().ToList();
		}
	}
}