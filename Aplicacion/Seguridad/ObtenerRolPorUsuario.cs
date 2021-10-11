using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Aplicacion.Seguridad
{
    public class ObtenerRolPorUsuario
    {
        public class Ejecuta: IRequest<List<string>>{
            public string UserName { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta, List<string>>
        {
            private readonly UserManager<Usuario> _userManager;
            private readonly RoleManager<IdentityRole> _roleManager;
            public Manejador(UserManager<Usuario> userManager, RoleManager<IdentityRole> roleManager ){
                _userManager = userManager;
                _roleManager = roleManager;
            }
            public async Task<List<string>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var usuarioIden = await _userManager.FindByNameAsync(request.UserName);
                if(usuarioIden == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {mensaje = "no existe el usuario"});
                }

                var resultado = await _userManager.GetRolesAsync(usuarioIden); // Este metodo lo que hace es devolverme una lista de string
                return new List<string>(resultado); //Convertimos IList A List
            }
        }
    }
}