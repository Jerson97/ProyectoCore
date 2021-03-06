using System;
using System.Threading;
using System.Threading.Tasks;
using FluentValidation;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    public class Editar
    {
        public class Ejecuta : IRequest{
            public Guid IdInstructor {get;set;}
            public string Nombre {get; set;}
            public string Apellidos {get;set;}
            public string Grado {get;set;}
            
        }
        public class EjecutaValida : AbstractValidator<Ejecuta>{
            public EjecutaValida(){
                RuleFor(x => x.Nombre).NotEmpty();
                RuleFor(x => x.Apellidos).NotEmpty();
                RuleFor(x => x.Grado).NotEmpty();
            }
        }

        public class Manejador : IRequestHandler<Ejecuta>
        {
            private readonly IInstructor _instructorRepository;
            public Manejador(IInstructor instructorRepository){
                _instructorRepository = instructorRepository;
            }
            public async Task<Unit> Handle(Ejecuta request, CancellationToken cancellationToken)
            {
               var resultado = await _instructorRepository.Actualiza(request.IdInstructor,request.Nombre,request.Apellidos,request.Grado);
               if(resultado>0){
                   return Unit.Value;
               }
               throw new Exception ("No se pudo actualizar la data");
            }
        }
    }
}