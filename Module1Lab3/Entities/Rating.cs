using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Entities
{
    [Table("Rating")]
    public class Rating
    {
        public int RatingId { get; set; }

        [Required]
        public string Code { get; set; }

        public string Name { get; set; }

        public ICollection<Film> Films { get; set; }
    }
}
