using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace landlorder.Models
{
    public class Review
    {
        [Key]
        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int reviewID { get; set; }

        [Required]
        public int rating { get; set; }

        [Required]
        public string review { get; set; }
        
        [ForeignKey("propertyID")]
        public virtual Property propertyID { get; set; }

        [ForeignKey("userID")]
        public virtual Users userID { get; set; }    
    }
}