using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Aplicacion.Cursos;
using AutoMapper;
using Dominio;
using FluentValidation.AspNetCore;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Persistencia;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructor;
using Persistencia.DapperConexion.Paginacion;
using Seguridad;
using WebAPI.Middleware;

namespace WebAPI
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            //incorporamos los cors del proyecto  para que estos webservices puedan ser consumidos desde cualquier cliente
            services.AddCors(o => o.AddPolicy("corsApp", builder => {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader(); //Esta policy me va a permitir consumir cualquier endpoint
                
            }));


            services.AddDbContext<CursosOnlineContext>(opt => {
                opt.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddOptions();
            //Cadena de conexion de Dapper
            services.Configure<ConexionConfiguracion>(Configuration.GetSection("ConnectionStrings"));


            services.AddMediatR(typeof(Consulta.Manejador).Assembly);

            //agremas estos codigo para que nuestros controller tenga la autorizacion antes de procesar el request de un cliente
            services.AddControllers(opt => {
                var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                opt.Filters.Add(new AuthorizeFilter(policy));
            })
            .AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());

            var builder = services.AddIdentityCore<Usuario>();
            var identityBuilder = new IdentityBuilder(builder.UserType, builder.Services);

            identityBuilder.AddRoles<IdentityRole>();
            //necesitamos incluir la data de los roles dentro de los token de seguridad
            identityBuilder.AddClaimsPrincipalFactory<UserClaimsPrincipalFactory<Usuario,IdentityRole>>();

            identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>();
            identityBuilder.AddSignInManager<SignInManager<Usuario>>();
            services.TryAddSingleton<ISystemClock, SystemClock>();




            //agregamos la logica para que tenga la autorizacion, para que no pueda consumir los endpoint sin tener el token
            // importamos la clase JwtBearerDefaults Microsoft.AspNetCore.Authentication.JwtBearer
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("Mi palabra secreta"));
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt => {
                opt.TokenValidationParameters = new TokenValidationParameters{
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = key,
                    ValidateAudience = false,
                    ValidateIssuer = false
                };
            });

            services.AddScoped<IJwtGenerador, JwtGenerador>();
            //aqui se hace la inyeccion de UsuarioSesion
            services.AddScoped<IUsuarioSesion, UsuarioSesion>(); // =>  APLICACION => UsuarioActual

            services.AddAutoMapper(typeof(Consulta.Manejador));

            //DapperConexion Instructor
            services.AddTransient<IFactoryConnection, FactoryConnection>();
            services.AddScoped<IInstructor, InstructorRepositorio>();

            //intanciamos como dependencia inyection el objeto y paginacion
            services.AddScoped<IPaginacion, PaginacionRepositorio>();

            //metodo para soportarl el Swagger  => despues del metodo, habilitamos la interfas grafica que tiene el Swagger vamos
            // al metodo configure.
            services.AddSwaggerGen( c => {
                c.SwaggerDoc("v1",new OpenApiInfo{
                    Title = "Servicios para mantenimiento de Curos",
                    Version = "v1"
                } );
                //agregamos filtro adicional para que no me de conflicto cuando ejecuta los ezquemas de los endpoints
                c.CustomSchemaIds(c => c.FullName);
            });
            

            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseCors("corsApp");
            app.UseMiddleware<ManejadorErrorMiddleware>();

            if (env.IsDevelopment())
            {

                 //app.UseDeveloperExceptionPage();
                // app.UseSwagger();
                // app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "WebAPI v1"));
            }

            // app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            //Habilitamos la interfas del swagger
            app.UseSwagger();
            app.UseSwaggerUI( c=> {//para que esta herramienta nos genere una pagina con la documentacion con cada endpoitns q hemos creado
                c.SwaggerEndpoint("/swagger/v1/swagger.json","Cursos Online v1");
            });   

        }
    }
}

//3 paso