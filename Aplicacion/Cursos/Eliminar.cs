using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Eliminar
    {
        public class Ejecuta : IRequest{
            public Guid Id { get; set; }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador (CursosOnlineContext context){
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                //Se elimina la lista de instructores que tienen este curso dentro de la tabla curso instructor
                var instructoresDB = _context.CursoInstructor.Where(x => x.CursoId == request.Id);
                foreach(var instructor in instructoresDB){
                    _context.CursoInstructor.Remove(instructor);
                }

                var ComentarioDB = _context.Comentario.Where(x => x.CursoId == request.Id); //que me devuelva todo los comentarios relacionados a este curso
                foreach(var comentario in ComentarioDB){
                    _context.Comentario.Remove(comentario);
                }

                var PrecioDB = _context.Precio.Where(x => x.CursoId == request.Id).FirstOrDefault();
                if(PrecioDB!= null){//si el precio existe
                    
                    _context.Precio.Remove(PrecioDB);
                }
                
                

                var curso =  await _context.Curso.FindAsync(request.Id);
                if(curso ==null){
                    //throw new Exception("No se peude eliminar  el curso");
                    throw new ManejadorException(HttpStatusCode.NotFound, new {curso = "No se encontro el curso"});

                }
                _context.Remove(curso);

                var resultado = await _context.SaveChangesAsync();

                if(resultado>0){
                    return Unit.Value;
                }
                throw new Exception("No se pudieron guardar los cambios");
            }
        }
    }
}