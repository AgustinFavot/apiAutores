using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using apiVS.Services;

namespace apiVS
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            //Limpiamos el mapeo automatico de los claims que realiza asp.net core
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();

            services.AddDbContext<ApplicationDbContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnections")));
            services.AddControllers().AddNewtonsoftJson(options => options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

            //El jwt esta firmado con una llave, el sistema va a validar que el token recibido haya sido firmado con esa clave
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options => options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(Configuration["llaveJwt"])),
                    ClockSkew = TimeSpan.Zero
                }) ;

            services.AddSwaggerGen(options => 
            {
                options.SwaggerDoc("apiVS", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "api VS",
                    Version = "1"
                });

                //Configurar swagger para token
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "Bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });

                options.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { 
                        new OpenApiSecurityScheme{ 
                            Reference = new OpenApiReference{ 
                                Type = ReferenceType.SecurityScheme,
                                Id = "Bearer"
                            }
                        },
                        new string[] { }
                    }
                });                
            });            

            services.AddAutoMapper(typeof(Startup));

            services.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            //Configuracion de claims para la autorizacion de usuarios
            services.AddAuthorization(options => options.AddPolicy("EsAdmin", politica => politica.RequireClaim("esAdmin")));

            //Area de la api que sea utilizada solo por usuarios que contengan el claim esVendedor
            //services.AddAuthorization(options => options.AddPolicy("EsVendedor", politica => politica.RequireClaim("ssVendedor")));


            //Servicio de proteccion de datos
            services.AddDataProtection();

            //Configuracion del servicio de hasheo
            services.AddTransient<HashService>();

            //Configuracion de CORS
            services.AddCors(options => {
                options.AddDefaultPolicy(builder =>
                {
                    builder.WithOrigins().AllowAnyMethod().AllowAnyHeader().WithExposedHeaders(new string[] { "Cantidad de registros" });
                });            
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => {
                    options.SwaggerEndpoint("/swagger/apiVS/swagger.json","API VS");
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
