using System.Collections.Generic;

namespace apiVS.DTOs
{
    public class BookCreation
    {
        public string Title { get; set; }
        public List<int> AuthorsIds { get; set; }
    }
}
