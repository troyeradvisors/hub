using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Web.Mvc;
using Hub.Domain.Entities;
using Hub.Domain.Repositories;

namespace Hub.Web.Models
{
    public class ChangePasswordModel
    {
		public Guid UserID { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [DataType(DataType.Password)]
		[Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
	
    public class LogOnModel
    {
        [Required]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public bool Save { get; set; }
    }

}
