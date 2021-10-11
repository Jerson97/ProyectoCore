using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Persistencia.DapperConexion.Instructor
{
    public interface IInstructor
    {
        //Aca indicamos lo que quiero q me retorne
         Task<IEnumerable<InstructorModel>> ObtenerLista();  
         Task<InstructorModel> ObtenerPorId(Guid id);  
         Task<int> Nuevo(string nombre, string apellidos, string grado);
         Task<int> Actualiza(Guid InstructorId, string nombre, string apellidos, string grado);
         Task<int> Elimina(Guid id);
    }
}