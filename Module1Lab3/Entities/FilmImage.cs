using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MovieApp.Entities
{
    [Table("FilmImage")]
    public class FilmImage
    {
        [Key]
        public int FilmImageId { get; set; }
        public string Title { get; set; }
        public string ImageUrl { get; set; }

        public int FilmId { get; set; }

        [ForeignKey(nameof(FilmId))]
        public Film Film { get; set; }
    }
}
