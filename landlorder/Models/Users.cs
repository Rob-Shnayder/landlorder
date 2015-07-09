using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace landlorder.Models
{
    public class Users
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int userID { get; set; }

        [Required]
        [StringLength(40, MinimumLength = 5)]
        public string username { get; set; }

        [StringLength(40, MinimumLength = 6)]
        [Required]
        public string password { get; set; }
    }


}