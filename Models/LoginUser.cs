using System.ComponentModel.DataAnnotations;

namespace Examprep.Models
{
    public class LoginUser
    {
         [Required]
        [EmailAddress]
        [Display(Name = "Your Email")]

        public string LoginEmail { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [MinLength(8, ErrorMessage = "YOUR PASSOWORD SHOULD BE AT LEAST 8 CHARACTER PLEAS")]
        [Display(Name = "Your Password")]
        public string LoginPassword { get; set; }
    }
}