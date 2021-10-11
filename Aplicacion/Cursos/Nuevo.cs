using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Nuevo
    {
        public class Ejecuta : IRequest {
            public Guid? CursoId  { get; set; }
            public string Titulo { get; set; }
            public string Descripcion { get; set; }
            public DateTime? FechaPublicacion { get; set; }
            public List<Guid> ListaInstructor {get;set;}    
            public decimal Precio {get; set;}
            public decimal Promocion {get; set;}
            public string Alumno { get; set; }
            public int Puntaje { get; set; }
            public string ComentarioTexto { get; set; }
        }

        public class EjecutaValidacion : AbstractValidator<Ejecuta>{
            public EjecutaValidacion(){
                RuleFor(x => x.Titulo).NotEmpty();
                RuleFor(x => x.Descripcion).NotEmpty();
                RuleFor(x => x.FechaPublicacion).NotEmpty();
            }
        }
        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly CursosOnlineContext _context;
            public Manejador(CursosOnlineContext context){
                _context = context;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
                
                Guid _cursoId = Guid.NewGuid();
                if(request.CursoId != null){
                    _cursoId = request.CursoId ?? Guid.NewGuid() ;
                }

                var curso = new Curso {
                    CursoId = _cursoId,
                    Titulo = request.Titulo,
                    Descripcion = request.Descripcion,
                    FechaPublicacion = request.FechaPublicacion,
                    FechaCreacion = DateTime.UtcNow // UtcNow => se crea automaticamente la fecha (fecha actual del cliente)
                };
                _context.Curso.Add(curso);


                // estamos insertando la lista de instructores que va a tener mi curso
                if(request.ListaInstructor!=null){
                    //este arreglo va a recorrer todo la lista de id de ListaInstructor
                    //var cursoInstructor = null;
                    foreach(var id in request.ListaInstructor){
                        var cursoInstructor = new CursoInstructor{
                            CursoId = _cursoId,
                            InstructorId = id
                        };
                        _context.CursoInstructor.Add(cursoInstructor); //=>aqui ponemos el cursoinstrucot quue estamos almacenando
                    }
                }

                

                /*Agregar  logica para insertar un precio del curso*/
                //Guid _precioId = Guid.NewGuid();
                var precioEntidad = new Precio{
                    CursoId = _cursoId,
                    PrecioActual = request.Precio,
                    Promocion = request.Promocion,
                    PrecioId = Guid.NewGuid()
                };

                    _context.Precio.Add(precioEntidad);
            

                var ComentarioEntidad = new Comentario{
                    CursoId = _cursoId,
                    Alumno = request.Alumno,
                    Puntaje = request.Puntaje,
                    ComentarioTexto = request.ComentarioTexto,
                    ComentarioId = Guid.NewGuid()
                };
                _context.Comentario.Add(ComentarioEntidad);

                var valor = await _context.SaveChangesAsync();
                if(valor>0){
                    return Unit.Value;
                }
                throw new Exception("No se pudo insertar el curso");
            }
        }
    }
}