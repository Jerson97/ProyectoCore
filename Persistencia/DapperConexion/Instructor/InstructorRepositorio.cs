using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Dapper;

namespace Persistencia.DapperConexion.Instructor
{
    public class InstructorRepositorio : IInstructor
    {
        private readonly IFactoryConnection _factoryConnection;
        public InstructorRepositorio(IFactoryConnection factoryConnection)
        {
            _factoryConnection = factoryConnection;
        }
        public async Task<int> Actualiza(Guid instructorId, string nombre, string apellidos, string grado)
        {
            var storeProcedure = "usp_instructor_editar";
            try
            {
                var connection = _factoryConnection.GetDbConnection();
                var resultado = await connection.ExecuteAsync(storeProcedure, new
                {
                    InstructorId = instructorId,
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Grado = grado
                },

                commandType: CommandType.StoredProcedure

                );

                _factoryConnection.CloseConnection();

                return resultado;
            }
            catch (Exception e)
            {
                throw new Exception("No se pudo editar la data del instructor" + e);
            }
            finally
            {

            }
        }

        public async Task<int> Elimina(Guid id)
        {
            var storeProcedure = "usp_instructor_elimina";

            try
            {
                var connection = _factoryConnection.GetDbConnection();
                var resultado = await connection.ExecuteAsync(storeProcedure, new
                {
                    InstructorId = id
                },
                commandType: CommandType.StoredProcedure
                );
                _factoryConnection.CloseConnection();

                return resultado;

            }
            catch (Exception e)
            {
                throw new Exception("No se pudo eliminar", e);
            }

        }

        public async Task<int> Nuevo(string nombre, string apellidos, string grado)
        {
            var storeProcedure = "usp_instructor_nuevo";

            try
            {
                var connection = _factoryConnection.GetDbConnection();
                var resultado = await connection.ExecuteAsync(storeProcedure, new
                {//ExecuteAsync me va a devolver un valor de tipo entero
                    InstructorId = Guid.NewGuid(),
                    Nombre = nombre,
                    Apellidos = apellidos,
                    Grado = grado
                },
                    commandType: CommandType.StoredProcedure
                    );
                _factoryConnection.CloseConnection();

                return resultado;

            }
            catch (Exception e)
            {
                throw new Exception("No se pudo guardar el nuevo valor ", e);
            }

        }

        public async Task<IEnumerable<InstructorModel>> ObtenerLista()
        {
            //
            IEnumerable<InstructorModel> instructorList = null;
            var storeProcedure = "usp_Obtener_Instructores";
            try
            {
                var connection = _factoryConnection.GetDbConnection();
                //  ↓ esto me retornara una lista de data de instructores ↓
                instructorList = await connection.QueryAsync<InstructorModel>(storeProcedure, null, commandType: CommandType.StoredProcedure); //que tipo de data es la q quiero q retorne mi Query / en este caso  InstructorModel
                                                                                                                                               //QueryAsync me de vuelte un tipo "IEnumerable" x eso no puede ser list
            }
            catch (Exception e)
            {
                throw new Exception("Error en la consulta de datos", e);
            }
            finally
            {// siempre se va a ejecutar    
                _factoryConnection.CloseConnection();
            }
            return instructorList;
        }

        public async Task<InstructorModel> ObtenerPorId(Guid id)
        {
            
            var storeProcedure = "usp_obtener_InstructoresPorId";
            InstructorModel instructor = null;
            try
            {
                var connection = _factoryConnection.GetDbConnection();
                instructor = await connection.QueryFirstAsync<InstructorModel>(storeProcedure, new
                {
                    Id = id
                },
                    commandType: CommandType.StoredProcedure
                );

                return instructor;
            }

            catch (Exception e)
            {
                throw new Exception("No se pudo encontra al intructor", e);
            }
            finally
            {
                _factoryConnection.CloseConnection();
            }
            
        }
    }
}