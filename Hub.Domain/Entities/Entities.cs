using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using Hub.Domain;
using Hub.Domain.Repositories;
using System.IO;
using System.Web;
using System.Web.Security;
using System.Web.Configuration;
namespace Hub.Domain.Entities
{
	[MetadataType(typeof(Client.Metadata))]
	public partial class Client
	{
		class Metadata { }

		public IQueryable<Service> AvailableServices
		{
			get
			{
				Repository<Service> services = new Repository<Service>();
				var ids = ClientSubscriptions.Select(s => s.ServiceID).Distinct().ToList();
				return services.All.Where(service => ids.All(id => service.ID != id));
			}

		}

		public IQueryable<ClientSubscription> ClientSubscriptions { get { return new Repository<ClientSubscription>().FilterBy(e => e.ClientID == ID); } }
	}


	[MetadataType(typeof(User.Metadata))]
	public partial class User
	{
		class Metadata
		{
			[Required]
			[DataType(DataType.EmailAddress)]
			//[RegularExpression(".+\\@.+\\..+", ErrorMessage = "Please enter a valid email address")]
			public string Email { get; set; }
			[Required]
			public string LastName { get; set; }
			[Required]
			public string FirstName { get; set; }
			//public int? ClientID { get; set; }
		}

		private List<string> _Roles;
		public List<string> Roles
		{
			get 
			{ 
				if (_Roles == null)
					_Roles = ID == Guid.Empty ? new List<string>() : System.Web.Security.Roles.GetRolesForUser(Email).ToList(); 
				return _Roles;
			}
			set { _Roles = value; }
		}

		public void SaveRoles()
		{
			var roles = System.Web.Security.Roles.GetRolesForUser(Email).ToList();
			if (roles.Count > 0)
				System.Web.Security.Roles.RemoveUserFromRoles(Email, System.Web.Security.Roles.GetRolesForUser(Email));
			if (Roles.Count > 0)
				System.Web.Security.Roles.AddUserToRoles(Email, Roles.ToArray());
		}
		public static User Current
		{
			get
			{
				return HttpContext.Current.Request.IsAuthenticated == false ? null : new Repository<User>().All.Single(e => e.Email.ToLower() == HttpContext.Current.User.Identity.Name.ToLower());
			}
		}

		public string FullName { get { return FirstName + " " + LastName; } }
		public string LastFirstName { get { return LastName + ", " + FirstName; } }

		public IQueryable<Service> AvailableServices
		{
			get
			{
				Repository<Service> services = new Repository<Service>();
				var ids = Subscriptions.Select(s => s.ServiceID).Distinct().ToList();
				return services.All.Where(service => ids.All(id => service.ID != id));
			}
		}

		public IQueryable<Subscription> Subscriptions { get { return new Repository<Subscription>().FilterBy(e => e.UserID == ID); } }

        
		public IQueryable<Subscription> ActiveSubscriptions 
		{ 
			get 
			{
				return Subscriptions.Where(e => e.EndDate > DateTime.Now || e.EndDate == null); 
			} 
		}
	}


	[MetadataType(typeof(UserSubscription.Metadata))]
	public partial class UserSubscription
	{
		class Metadata
		{
			[DataType(DataType.Date)]
			public DateTime? EndDate { get; set; }
			[Required]
			public DateTime ServiceID { get; set; }
		}
	}

	[MetadataType(typeof(ClientSubscription.Metadata))]
	public partial class ClientSubscription
	{
		class Metadata
		{
			[DataType(DataType.Date)]
			public DateTime? EndDate { get; set; }

			[Required]
			public DateTime ServiceID { get; set; }
		}

	}


	[MetadataType(typeof(Service.Metadata))]
	public partial class Service
	{
		class Metadata
		{
			[Required]
			public string Name { get; set; }

			[Display(Name = "Application URL")]
			public string URL { get; set; }
		}
		public readonly static string ServerRoot = WebConfigurationManager.AppSettings["DocumentUrl"];
        public readonly static string DocumentRoot = WebConfigurationManager.AppSettings["DocumentRoot"].Replace("/", "\\");
        public string DocumentPath { get { return this.ServiceType.IsDocument ? Path.Combine(DocumentRoot, this.ServiceType.Name, this.Name) : ""; } }
		public string ActualPath { get { string docpath = DocumentPath; return docpath == "" ? URL : this.ServiceType.Name + "/" + this.Name; } }
	}

	[MetadataType(typeof(ServiceType.Metadata))]
	public partial class ServiceType
	{
		class Metadata
		{
			public string Image { get; set; }
			[Required]
			public string Name { get; set; }
		}
		public ServiceType()
		{
			Image = "http://troyeradvisorsdashboards.com/services/resources/images/services/Application.png";
		}
	}

}
