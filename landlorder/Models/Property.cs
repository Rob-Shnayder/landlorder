using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace landlorder.Models
{
    public class Property
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public virtual int propertyID { get; set; }

        [Required]
        public string streetaddress { get; set; }

        [Required]
        public string city { get; set; }

        [Required]
        public string zip { get; set; }

        [Required]
        public string state { get; set; }

        [Required]
        public string country { get; set; }

        public string apartmentNum { get; set; }

        public string landlordName { get; set; }
    }
}