using apiVS.DTOs;
using AutoMapper;
using System.Linq;
using apiVS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using apiVS.Utils;

namespace apiVS.Controllers.v1
{
    [Route("api/authors")]
    [ApiController]
    //Authorize a nivel de controlador...
    //Con los Claims podemos agregar politicas de seguridad para que los usuarios puedan realizar determinadas acciones.
    //Esto es util cuando tenemos una jerarquia de usuarios en nuestro sistema
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Policy = "EsAdmin")]
    public class AuthorsControllers : ControllerBase
    {
        private readonly ApplicationDbContext context;

        private readonly IMapper mapper;

        public AuthorsControllers(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }


        //Proteccion a nivel de endpoint...
        [HttpGet]
        //Excepcion al authorize para que usuarios no autenticados puedan consumir
        //En caso de que el endpoint no contenga esta atributo, solo podra ser consumido por usuarios que contengan un jwt valido
        [AllowAnonymous]
        public async Task<ActionResult<List<AuthorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
        {
            try
            {
                var queryable = context.Authors.AsQueryable();
                await HttpContext.InsertarParametrosPaginacionEnCabecera(queryable);
                var authors = await queryable.OrderBy(autor => autor.Name).Paginar(paginacionDTO).ToListAsync();
                return mapper.Map<List<AuthorDTO>>(authors);

            }
            catch (System.Exception ex)
            {

                throw ex;
            }
            
        }


        [HttpGet("{name}")]
        public async Task<ActionResult<List<AuthorDTO>>> GetByName([FromRoute] string name)
        {
            var author = await context.Authors.Where(x => x.Name.Contains(name)).ToListAsync();
            if (author == null)
            {
                return NotFound();
            } 
            return mapper.Map<List<AuthorDTO>>(author);
        }


        [HttpGet("{id:int}")]
        public async Task<ActionResult<AuthorDTO>> GetById([FromRoute] int id)
        {
            var author = await context.Authors.FirstOrDefaultAsync(x => x.Id == id);
            if (author == null)
            {
                return BadRequest("El Autor no existe");
            }
            return mapper.Map<AuthorDTO>(author);
        }


        [HttpPost]
        public async Task<ActionResult> Post([FromBody] AuthorCreation authorCreation)
        {
            var exist = await context.Authors.AnyAsync(x => x.Name == authorCreation.Name);
            if (exist)
            {
                return BadRequest("Ya existe el Autor");
            }
            var author = mapper.Map<Author>(authorCreation);
            context.Add(author);
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpPut("{id:int}")]
        public async Task<ActionResult> Post([FromRoute] int id, [FromBody] Author author)
        {
            var valid = await context.Authors.AnyAsync(x => x.Id == id);
            if (!valid)
            {
                return BadRequest("El Id del autor no existe");
            }
            context.Update(author);
            await context.SaveChangesAsync();
            return Ok();
        }


        [HttpDelete("{id:int}")]
        public async Task<ActionResult> delete([FromRoute] int id)
        {
            var author = await context.Books.FirstOrDefaultAsync(x => x.Id == id);
            if (author == null)
            {
                return BadRequest("El Id del autor no existe");
            }
            context.Remove(author);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
