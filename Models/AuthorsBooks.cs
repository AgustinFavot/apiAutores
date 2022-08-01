namespace apiVS.Models
{
    public class AuthorsBooks
    {
        public int AuthorId { get; set; }
        public int BookId { get; set; }

        //Puede ser una regla de negocio, que consiste en ordenar los autores de acuerdo a su contribución
        public int Order { get; set; }

        //Propiedades de navegacion
        public Author Authors { get; set; }
        public Book Books { get; set; }
    }
}
