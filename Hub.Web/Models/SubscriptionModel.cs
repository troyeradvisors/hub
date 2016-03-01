using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using System.Web.Security;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;

namespace Hub.Web.Models
{
    public class SubscriptionModel
    {
        public Guid? UserID { get; set;}
        public int? ClientID { get; set;}
		public string UserName { get; set; }
		public string ClientName { get; set; }
        public List<Subscription> Subscriptions { get ;set;}
    }
}