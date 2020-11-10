using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using WebApplication.Models;

namespace WebApplication.ViewModels
{
    public class UsersViewModel
    {
        [Display(Name = "Users")]
        public IEnumerable<User> Users { get; set; }
        public User User { get; set; }


        [Display(Name = "Modification")]
        public string Modification { get; set; }

        [Required]
        [Display(Name = "Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }


        [Required]
        [Display(Name = "New password")]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }


        public DeleteViewModel DeleteViewModel { get; set; }
    }
}
