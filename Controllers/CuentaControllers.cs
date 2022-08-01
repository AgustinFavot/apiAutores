using System;
using apiVS.DTOs;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using apiVS.Services;

namespace apiVS.Controllers.v1
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentaControllers: ControllerBase
    {
        private readonly UserManager<IdentityUser> manager;
        private readonly IConfiguration configuration;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly HashService hashService;
        private readonly IDataProtector dataProtector;

        //Identity nos provee un conjunto de clases para realizar toda la funcionalidad de un sistema de autenticacion
        //El servicio que me permite registrar un usuario UserManager<Clase que identifica al usuario de la app>
        public CuentaControllers(UserManager<IdentityUser> manager, IConfiguration configuration, SignInManager<IdentityUser> signInManager, IDataProtectionProvider protectionProvider, HashService hashService)
        {
            this.manager = manager;
            this.configuration = configuration;
            this.signInManager = signInManager;
            this.hashService = hashService;
            dataProtector = protectionProvider.CreateProtector("ASDINQWDANSBDSADK");
            
        }

        [HttpGet("Hash/{textoPlano}")]
        public ActionResult Hash( string textoPlano)
        {
            var rest1 = hashService.createSemilla(textoPlano);
            var rest2 = hashService.createSemilla(textoPlano);
            return Ok(new
            {
                rest1,
                rest2
            });
        }

            [HttpGet("Encriptar")]
        public ActionResult Encriptar() {
            //Puedo crear un dataProtector limitado por el tiempo
            var dataProtectorTiempo = dataProtector.ToTimeLimitedDataProtector();
            var textPlain = "HOLAAAA";
            var textCifrado = dataProtectorTiempo.Protect(textPlain, lifetime: TimeSpan.FromSeconds(5));
            var textDes = dataProtectorTiempo.Unprotect(textCifrado);

            return Ok(new
            {
                textPlain,
                textCifrado,
                textDes
            });
        } 

        [HttpPost("registrar")]
        public async Task<ActionResult<RespuestaAutenticacion>> Registrar([FromBody] CredencialesUsuario credenciales) {
            var user = new IdentityUser {
                UserName = credenciales.email,
                Email = credenciales.email
            };

            var result = await manager.CreateAsync(user, credenciales.password);

            if (result.Succeeded)
            {
                //Json Web Token: Standar que especifica el formato del token que los clientes deben utilzar para usar la aplicacion
                //El token se divide en tres partes:
                //Header : informacion acerca del algoritmo para la construccion del token y el tipo de token
                //Payload :  claims, data,
                //Verify signature : se utiliza para validar el token.
                return await CosntruirToekn (credenciales);
            }
            else {
                return BadRequest(result.Errors);
            }
        }


        [HttpPost("login")]
        public async Task<ActionResult<RespuestaAutenticacion>> login([FromBody] CredencialesUsuario credenciales) {
            //Para realizar el login, podemos utilizar una clase llamada SignInManager, la cual recibe como parametro la clase del usuario
            //lockoutOnFailure significa que si los intentos de logeos son incorrectos, el usuario puede ser bloqueado
            var result = await signInManager.PasswordSignInAsync(credenciales.email, credenciales.password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await CosntruirToekn(credenciales);
            }
            else {
                return BadRequest();
            }
        }

        //Logout: con JWT consiste en borrar el token, de esta manera el usuario no podra acceder mas a la aplicacion

        [HttpGet("RenovarToken")]
        //Autorizado para aquellas personas que tengan un token valido, este por expirar y por exp de usuario lo renovamos
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<RespuestaAutenticacion>> Renovar() {

            var email = HttpContext.User.Claims.Where(claim => claim.Type == "email").FirstOrDefault();
            var credenciales = new CredencialesUsuario()
            {
                email = email.Value
            };

            return await CosntruirToekn(credenciales);
        }

        //Agrego roles a los usuarios, para que de acuerdo a sos claims, son cosas que va a poder realizar
        [HttpPost("HacerAdmin")]
        public async Task<ActionResult> AddAdmin([FromBody] AdminDTO adminDTO)
        {
            //Necesito al usuario
            var user = await manager.FindByEmailAsync(adminDTO.email);
            if (user == null) return BadRequest();
            await manager.AddClaimAsync(user, new Claim("esAdmin", "1"));
            return NoContent();
        }

        //Remuevo claim
        [HttpPost("RemoverAdmin")]
        public async Task<ActionResult> RemoverAdmin([FromBody] AdminDTO adminDTO)
        {
            //Necesito al usuario
            var user = await manager.FindByEmailAsync(adminDTO.email);
            if (user == null) return BadRequest();
            await manager.RemoveClaimAsync(user, new Claim("esAdmin","1"));
            return NoContent();
        }

        private async Task<RespuestaAutenticacion> CosntruirToekn(CredencialesUsuario credenciales)
        {
            //Claim es un fragmento de informacion del usuario, como nombre, correo. Estos claim seran añadidos al token 
            //y podremos acceder a la inforamcion que contiene los claim.
            var claims = new List<Claim>()
            {
                new Claim("email", credenciales.email)
            };

            var user = await manager.FindByEmailAsync(credenciales.email);
            var claimsDB = await manager.GetClaimsAsync(user);

            claims.AddRange(claimsDB);

            //Firmar el token. Para eso necesito la llave secreta, ubicada en un proveedor de configuracion appsettings.json
            var privateKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["llaveJwt"]));
            
            var cred = new SigningCredentials(privateKey, SecurityAlgorithms.HmacSha256);

            var expiracion = DateTime.UtcNow.AddYears(1);

            //contruir token
            var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims,expires: expiracion, signingCredentials: cred);

            return new RespuestaAutenticacion()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
                Expiracion = expiracion
            };
        }

        //Logearme cuando me haya seteado los claims para que el token tenga esos datos
    }
}
