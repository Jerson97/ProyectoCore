using System;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Seguridad
{
    public class ElimarUsuario
    {
        public class Ejecuta: IRequest{
            public string Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _contex;
            public Manejador(CursosOnlineContext context){
                _contex = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var user = await _contex.Users.FindAsync(request.Id);
                if(user == null){
                    throw new ManejadorException(HttpStatusCode.NotFound, new {curso = "No se encontro el usuario"});
                }
                _contex.Remove(user);

                var resultado = await _contex.SaveChangesAsync();

                if(resultado>0){
                    return Unit.Value;
                }
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}