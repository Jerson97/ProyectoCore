using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class UsuarioRolAgregar
    {
        public class Ejecuta :IRequest {
            public string UserName { get; set; }   // Le indico que usuario tendra el rol
            public string RolNombre { get; set; }  //El rol que tendra el usuario
        }

        public class EjecutaValidador : AbstractValidator<Ejecuta>{
            public EjecutaValidador(){
                RuleFor(x => x.UserName).NotEmpty();
                RuleFor(x => x.RolNombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager ){
                _userManager = userManager;
                _roleManager = roleManager;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByNameAsync(request.RolNombre);
                if(role == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "no existe el rol"});
                }
                var usuarioIden = await _userManager.FindByNameAsync(request.UserName);
                if(usuarioIden == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "no existe el Usuario"});
                }
                var resultado = await _userManager.AddToRoleAsync(usuarioIden,request.RolNombre);    
                if(resultado.Succeeded){
                    return Unit.Value;
                }     

                throw new System.Exception("No se pudo agregar el rol al usuario");      
            }
        }
    }
}