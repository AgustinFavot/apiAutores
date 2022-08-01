using apiVS.DTOs;
using AutoMapper;
using apiVS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace apiVS.Controllers.v1
{
    [ApiController]
    [Route("api/books")]
    public class BooksControllers : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public BooksControllers(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        [HttpGet]
        public async Task<ActionResult<List<BookDTO>>> Get()
        {
            var books = await context.Books.Include(x => x.Comments ).ToListAsync();
            return mapper.Map<List<BookDTO>>(books);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDTO>> GetById([FromRoute] int id)
        {
            //Debo mapear de Author a AuthorDTO que es lo que espera BookDTO
            var book = await context.Books.Include(x => x.ListAutoresLibros).ThenInclude(x => x.Authors).FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return BadRequest("El Id ingresado no corresponde a nigún libro");
            }
            return mapper.Map<BookDTO>(book);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] BookCreation bookCreation)
        {

            if (bookCreation.AuthorsIds == null)
            {
                return BadRequest("No se puede crear un libro sin autores");
            }
            var authors = await context.Authors.Where(x => bookCreation.AuthorsIds.Contains(x.Id)).Select(x => x.Id).ToListAsync();

            if (authors.Count != bookCreation.AuthorsIds.Count) return NotFound();

            var book = mapper.Map<Book>(bookCreation);
            context.Add(book);
            try
            {
                await context.SaveChangesAsync();
            }
            catch (System.Exception ex)
            {
                throw;
            }
            return Ok();       
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Post([FromRoute] int id, [FromBody] Book book)
        {
            var valid = await context.Books.AnyAsync(x => x.Id == id);
            if (!valid)
            {
                return BadRequest("El Id del libro no existe");
            }
            context.Update(book);
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> delete([FromRoute] int id)
        {
            var book = await context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (book == null)
            {
                return BadRequest("El Id del libro no existe");
            }
            context.Remove(book);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
