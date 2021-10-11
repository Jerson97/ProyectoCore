using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class RolEliminar
    {
        public class Ejecuta: IRequest{
            public string Nombre{get;set;} 
        }

         public class ValidaEjecuta: AbstractValidator<Ejecuta>{
            public ValidaEjecuta(){
                RuleFor(x => x.Nombre).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(RoleManager<IdentityRole> roleManager){
                _roleManager = roleManager;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                if(role == null){
                    throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje = "No existe este rol"});
                }
                var resultado = await _roleManager.DeleteAsync(role);
                if(resultado.Succeeded){
                    return Unit.Value;
                }
                throw new Exception("No se pudo guardar el rol");
                
            }
        }
    }

    
}