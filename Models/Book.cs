using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace apiVS.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }

        [StringLength(maximumLength: 250, MinimumLength =10)]
        public List<Comments> Comments { get; set; }
        public List<AuthorsBooks> ListAutoresLibros { get; set; }

        //public void sort()
        //{
        //    for (int i = 0; i < AuthorsBooks.Count; i++)
        //    {
        //        AuthorsBooks[i].Order = i;
        //    }
        //}
    }
}
