using apiVS.DTOs;
using AutoMapper;
using System.Linq;
using apiVS.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace apiVS.Controllers.v1
{
    [ApiController]
    [Route("api/books/{bookId:int}/comments")]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CommentsControllers : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private readonly UserManager<IdentityUser> userManager;

        public CommentsControllers(ApplicationDbContext context, IMapper mapper, UserManager<IdentityUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.userManager = userManager;
        }


        [HttpGet]
        public async Task<ActionResult<List<CommentDTO>>> Get([FromRoute] int bookId)
        {
            var comments = await context.Comments.Where(x => x.BookId == bookId).ToListAsync();
            return mapper.Map<List<CommentDTO>>(comments);
        }


        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        //Acceso a los claim del usuario que vienen a traves del jwt
        //[AllowAnonymous]
        //Dentro de un contexto authorize, voy a poder acceder a los claims  aun cuando este endpoint tenga la etiquita AllowAnonymus
        //porque el authorize esta a nivel global
        public async Task<ActionResult> Post([FromRoute] int bookId, [FromBody] CommentCreation commentCreation )
        {
            //A partir del email del usuario voy a conseguir el id del usuario
            //Inyectar el servicio que me va a permitir buscar al usuario
            //.Claims funciona en el contexto de authorize, de lo contrario no trae los resultado
            //Puedo 
            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault().Value;
            
            //Por cuestiones de seguridad, esta data necesaria no se agrego CommentDTO. A traves de los claim
            //es mas seguro debido a que primero el usuario tuvo que autenticarse
            
            var user = await userManager.FindByEmailAsync(email);

            var exist = await context.Books.AnyAsync(x => x.Id == bookId);
            if (!exist)
            {
                return NotFound();
            }
            var comment = mapper.Map<Comments>(commentCreation);
            comment.BookId = bookId;
            comment.UsuarioId = user.Id;
            context.Add(comment);
            await context.SaveChangesAsync();
            return Ok();
        }
    }
}
