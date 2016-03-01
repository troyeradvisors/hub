using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;
using System.Web.Mvc;

namespace Hub.Web.Models
{
	public class ClientKeyModel
	{
		static Repository<ClientKey> repoKey = new Repository<ClientKey>();
		static Repository<Client> repo = new Repository<Client>();
		public SelectList SelectClients(object selected) { return new SelectList(repo.FilterBy(e=>e.Name.ToLower()!="troyer advisors").OrderBy(e=>e.Name), "ID", "Name", selected); }
		[Required]
		public int? ClientID { get; set; }

		private List<string> _Keys = new List<string>();
		public List<string> Keys
		{
			get { return _Keys; }
			set { _Keys = value; }
		}

		[Required]
		[Range(1, int.MaxValue)]
		public int? Count { get; set; }
		public DateTime CreateDate { get { return DateTime.Now; } }
		[DataType(DataType.Date)]
		public DateTime? InvalidDate { get; set; }

		public void GenerateKeys()
		{
			Keys.Clear();
			Enumerable.Repeat(0, Count.Value).Select(e => new ClientKey() { Key = Guid.NewGuid(), ClientID = ClientID.Value, CreateDate = CreateDate, InvalidDate = InvalidDate })
				.ToList().ForEach(key => { repoKey.Add(key, false); Keys.Add(key.Key.ToString().ToUpper()); });
			repoKey.Save();
		}
	}
}