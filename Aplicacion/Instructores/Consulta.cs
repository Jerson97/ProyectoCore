using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Persistencia.DapperConexion.Instructor;

namespace Aplicacion.Instructores
{
    //Cuando se quiere invocar la persistencia sea de entityframwork o dapper, necesito crear 2 clases para el mapper
    public class Consulta
    {
        public class Lista : IRequest<List<InstructorModel>>{
            
        }
        public class Manejador : IRequestHandler<Lista, List<InstructorModel>>
        {
            private readonly IInstructor _instructorRepository;
            public Manejador(IInstructor instructorRepository){
                _instructorRepository = instructorRepository;
            }
            public async Task<List<InstructorModel>> Handle(Lista request, CancellationToken cancellationToken)
            {
               var resultado = await _instructorRepository.ObtenerLista();
               return resultado.ToList();
            }
        }
    }
}