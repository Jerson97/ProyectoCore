using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.Contratos;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Persistencia;
using Microsoft.EntityFrameworkCore;
using Aplicacion.ManejadorError;
using System.Net;
using System;
using FluentValidation;

namespace Aplicacion.Seguridad
{
    public class Registrar
    {
        public class Ejecuta : IRequest<UsuarioData> {
            public string NombreCompleto {get;set;}
            public string Email {get;set;}
            public string Password {get;set;}

            public string Username {get;set;}
        }

        public class EjecutaValidador : AbstractValidator<Ejecuta>{
            public EjecutaValidador(){
                RuleFor(x => x.NombreCompleto).NotEmpty();
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
                RuleFor(x => x.Username).NotEmpty();
            }
        }

        

        public class Manejador : IRequestHandler<Ejecuta, UsuarioData>
        {
            private readonly CursosOnlineContext _context;
            private readonly UserManager<Usuario> _userManager;
            private readonly IJwtGenerador _jwtGenerador;
            public Manejador(CursosOnlineContext context, UserManager<Usuario> userManager, IJwtGenerador jwtGenerador){
                _context = context;
                _userManager=userManager;
                _jwtGenerador = jwtGenerador;
            }

            public async Task<UsuarioData> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
               var existe = await _context.Users.Where( x => x.Email == request.Email).AnyAsync();
               if(existe){
                  throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje = "Existe ya un usuario registrado con este email"});
               }
               
               var existeUserName = await _context.Users.Where( x => x.UserName == request.Username).AnyAsync();
               if(existeUserName){
                   throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje="Existe ya un usuario con este username"});
               }


               var usuario = new Usuario {
                 NombreCompleto = request.NombreCompleto,
                 Email = request.Email,
                 UserName = request.Username
               };

               var resultado = await _userManager.CreateAsync(usuario, request.Password);
               if(resultado.Succeeded){
                   return new UsuarioData {
                       NombreCompleto = usuario.NombreCompleto,
                       Token = _jwtGenerador.CrearToken(usuario, null),
                       userName = usuario.UserName,
                       Email = usuario.Email
                   };
               }
                 


               throw new Exception("No se pudo agregar al nuevo usuario") ;
            }
        }
    }
}