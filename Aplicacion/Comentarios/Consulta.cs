using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Persistencia;

namespace Aplicacion.Comentarios
{
    public class Consulta
    {
        public class Ejecuta :IRequest<List<Comentario>>{
            
        }

        public class Manejador : IRequestHandler<Ejecuta, List<Comentario>>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context){
                _context = context;
            }
            public async Task<List<Comentario>> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                var lst = await _context.Comentario.ToListAsync();
                return lst;
            }
        }
    }
}