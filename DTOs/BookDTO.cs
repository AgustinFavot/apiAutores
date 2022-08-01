using System.Collections.Generic;

namespace apiVS.DTOs
{
    public class BookDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public List<AuthorDTO> ListAuthors { get; set; }
        public List<CommentDTO> Comments { get; set; }
    }
}
