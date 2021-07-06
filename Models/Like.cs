using System.ComponentModel.DataAnnotations;

namespace Examprep.Models
{
    public class Like
    {
        [Key]
        public int LikeId { get; set; }
        public int UserId { get; set; }

        public int MovieId { get; set; }
        public User Fan { get; set; }

        public Movie FanOf{ get; set; }



    }
}