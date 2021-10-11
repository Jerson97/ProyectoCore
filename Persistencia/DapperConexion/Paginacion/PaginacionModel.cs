using System.Collections.Generic;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionModel
    {
        //1ra propieda la lita de records  ↓↓
        public List<IDictionary<string,object>> ListaRecords {get; set;}
        // [{cursoId: "123123", "titulo": "aspnet"}] , [{cursoId: "34343434", "titulo": "react"}]

        public  int TotalRecords {get; set;}  // me representa el total de registro que esta en la base de datos respecto a cursos
        
        public int NumeroPaginas {get; set;}
    }
}