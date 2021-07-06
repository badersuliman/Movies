using System.ComponentModel.DataAnnotations;
using System;
using System.Collections.Generic;

namespace Examprep.Models
{
    public class Movie
    {
        [Key]
        public int MovieId { get; set; }
        [Required]
        [MinLength(3, ErrorMessage = "Must be at least 3 characters")]
        public string Title { get; set; }
        [Required]
         [MinLength(2, ErrorMessage = "Must be at least 2 characters")]
        [Display(Name = "Starring")]
        public string Star { get; set; }
        [Required]
         [MinLength(10, ErrorMessage = "Must be at least 10 characters")]
        [Display(Name = "Image URL")]

        public string ImgUrl { get; set; }
        [Required]
        [Display(Name = "Release Date")]
        [DataType(DataType.Date)]
        public DateTime ReleaseDate { get; set; }


        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime UpdatedAt { get; set; } = DateTime.Now;


        public int UserId { get; set; }
        public User PostedBy { get; set; }
         public List<Like> Fans { get; set; }
    }

}