using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;
using System.Linq;

namespace Persistencia.DapperConexion.Paginacion
{
    public class PaginacionRepositorio : IPaginacion
    {
        private readonly IFactoryConnection _factoryConnection;
        public PaginacionRepositorio(IFactoryConnection factoryConnection){
            _factoryConnection = factoryConnection;
        }
        public async Task<PaginacionModel> devolverPaginacion(string storeProcedure, int numeroPagina, int cantidadElementos, IDictionary<string, object> parametrosFiltro, string ordenamientoColumna)
        {
            PaginacionModel paginacionModel = new PaginacionModel();
            List<IDictionary<string,object>> listaReporte = null;
            int totalRecords = 0;
            int totalPaginas = 0;

            try{
                var connection = _factoryConnection.GetDbConnection();
                //creamos los parametros y lo pasamos al query (parametros para insertar la data)
                //Parametros de entrada
                DynamicParameters parametros = new DynamicParameters();

                //parametroFiltro
                // Con este bucle estoy insertando toda las posibles filtros que puedas hacerle a tu logica en el Procedimiento alamcenados
                foreach(var param in parametrosFiltro){ //Que este bucle lea la data del arreglo parametroFiltro
                    parametros.Add("@" + param.Key, param.Value);  //param.key es el nombre q se le da al parametro y param.value le digo que se registre el valor de ese parametro

                }

                parametros.Add("@NumeroPagina", numeroPagina);           
                parametros.Add("@CantidadElementos", cantidadElementos);       //Estros 3 elementos van a ingresar  a la BD para pdoer ejecutar la operacion
                parametros.Add("@Ordenamiento", ordenamientoColumna);

                //Parametros de salida
                parametros.Add("@TotalRecords", totalRecords, DbType.Int32, ParameterDirection.Output);
                parametros.Add("@TotalPaginas", totalPaginas, DbType.Int32, ParameterDirection.Output);


                var result = await connection.QueryAsync(storeProcedure,parametros,commandType: CommandType.StoredProcedure);
                // convertimos task<Ienumerale> a IDictionary
                listaReporte = result.Select(x => (IDictionary<string, object>)x).ToList();
                paginacionModel.ListaRecords = listaReporte;
                paginacionModel.NumeroPaginas = parametros.Get<int>("@TotalPaginas");
                paginacionModel.TotalRecords = parametros.Get<int>("TotalRecords");
                // paginacionModel.TotalRecords = result.Count(); //parametros.Get<int>("@TotalPaginas");

            }catch(Exception e){
                throw new Exception("No se pudo ejecutar el procedimiento almacenado", e);
            }finally{
                _factoryConnection.CloseConnection();
            }
            
            return paginacionModel;
        }
    }
}