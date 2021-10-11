using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Aplicacion.ManejadorError;
using Dominio;
using FluentValidation;
using MediatR;
using Persistencia;

namespace Aplicacion.Cursos
{
    public class Editar
    {
        
        public class Ejecuta : IRequest{
        public Guid CursoId { get; set; }
        public string Titulo { get; set; }
        public string Descripcion { get; set; }
        public DateTime? FechaPublicacion { get; set; }
        public List<Guid> ListaInstructor {get; set;}
        public decimal? Precio {get; set;}
        public decimal? Promocion {get; set;}
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
                // Actualizar la data del curso
                var curso = await _context.Curso.FindAsync(request.CursoId);
                 if(curso ==null){
                    //throw new Exception("No se peude eliminar  el curso");
                    throw new ManejadorException(HttpStatusCode.NotFound, new {curso = "No se encontro el curso"});

                }

                curso.Titulo =request.Titulo ?? curso.Titulo;
                curso.Descripcion = request.Descripcion ?? curso.Descripcion;
                curso.FechaPublicacion = request.FechaPublicacion ?? curso.FechaPublicacion;
                curso.FechaCreacion = DateTime.UtcNow;

                /*Actualizar el Precio del Curso*/
                var precioEntidad = _context.Precio.Where(x => x.CursoId == curso.CursoId).FirstOrDefault();// Esto me va a devolver el primer valor que encuentre y cumpla la condicion
                if(precioEntidad!=null){// Si precioEntidad es diferente de nulo entonces si voy a poder actualizar este valor
                    precioEntidad.Promocion = request.Promocion ?? precioEntidad.Promocion; //Si el valor de Promocion fuera null, va a mantener el valor q tenia antes ??
                    precioEntidad.PrecioActual = request.Precio ?? precioEntidad.PrecioActual;

                }else{//Si el precio es null se inserta el dato
                    precioEntidad = new Precio{
                        PrecioId = Guid.NewGuid(),
                        PrecioActual = request.Precio ?? 0, //En caso de que no lo inserte lo voy a poner q sea 0 
                        Promocion = request.Promocion ?? 0,
                        CursoId = curso.CursoId
                    };
                   await _context.Precio.AddAsync(precioEntidad);
                }


                //
                //Con esto estoy verificando de que mi lista tenga valores para poder hacer esta tarea
                //de lo contrario no va hacer nada
                if(request.ListaInstructor!=null){
                    if(request.ListaInstructor.Count>0){
                        /*Eliminar los intructores actuales en la base de datos*/
                        var instructoresBD = _context.CursoInstructor.Where(x => x.CursoId == request.CursoId).ToList();

                        //Vamos ahora a ejecutar un Query para poder eliminar esta lista de instructores
                        //que tenga este curdoId => bucle
                        foreach(var instructorEliminar in instructoresBD){
                            
                            _context.CursoInstructor.Remove(instructorEliminar);

                        }
                        /*Fin del procedimiento para eliminar intructores*/



                        /*Procedimiento para agregar intructores que provienen de cliente*/
                        foreach(var id in request.ListaInstructor){
                            var nuevoIntructor = new CursoInstructor {
                                CursoId = request.CursoId,
                                InstructorId = id
                            };
                            _context.CursoInstructor.Add(nuevoIntructor);
                        }

                        /*Fin del procedimineto*/
                    }

                }

                var resultado = await _context.SaveChangesAsync();

                if(resultado > 0)
                    return Unit.Value;
                    throw new Exception("No se guardaron los cambios en el curso");
                

            }
        }
    }
}