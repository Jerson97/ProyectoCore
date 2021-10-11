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
    public class RolNuevo
    {
        // esta clase se va a encargar de manejar la logica  para poder crear un nuevo rol dentro de nuestra BD
        //del sistema identityCore
        public class Ejecuta: IRequest{
            public string Nombre { get; set; }
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
                //Ahora vamos a validar que ese rol que se va a crear no exista
                var role = await _roleManager.FindByNameAsync(request.Nombre);
                if(role!=null){// si este rol existe entonces tiene que ser difernte de nulo xq ya existe
                    throw new ManejadorException(HttpStatusCode.BadRequest, new {mensaje = "ya existe el rol"});
                }
                //si no lo encuentra, continuaria y crea el rol
                var resultado = await _roleManager.CreateAsync(new IdentityRole(request.Nombre));
                if(resultado.Succeeded){
                    return Unit.Value;
                }
                throw new Exception("No se pudo guardar el rol");
            }
        }
    }
}