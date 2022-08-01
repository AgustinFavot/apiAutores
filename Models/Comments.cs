using Microsoft.AspNetCore.Identity;

namespace apiVS.Models
{
    public class Comments
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public int BookId { get; set; }
        public Book Book { get; set; }
        public string UsuarioId { get; set; }
        public IdentityUser Usuario { get; set; }
    }
}
