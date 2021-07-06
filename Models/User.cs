using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Examprep.Models
{
    public class User
    {
        [Key]

        [Required]
        public int UserId { get; set; }
        [Required]
        [Display(Name = "FIRST NAME")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "LAST NAME")]
        public string LastName { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
       
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "YOUR PASSOWORD SHOULD BE AT LEAST 8 CHARACTER PLEAS")]
        public string Password { get; set; }

        [NotMapped]
        [Compare("Password")]
        [DataType(DataType.Password)]
        [Display(Name = "Confirm")]
        public string Confirm { get; set; }
        public List<Movie> PostedMovie { get; set; }
        public List<Like> LikedMovie { get; set; }

         


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
    }
}