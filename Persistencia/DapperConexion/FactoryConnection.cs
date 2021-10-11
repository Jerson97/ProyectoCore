using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Options;

namespace Persistencia.DapperConexion
{
    public class FactoryConnection : IFactoryConnection
    {
        private IDbConnection _connection;
        private readonly IOptions<ConexionConfiguracion> _configs;
        public FactoryConnection(IOptions<ConexionConfiguracion> configs){
            _configs = configs;
        }
        public void CloseConnection()
        {
            if(_connection != null && _connection.State == ConnectionState.Open){// si Conex es diferente de nulo y Conex.state = a ConexState.Open, Entonces yo lo q quiero es q la conexion se cierre
                _connection.Close();
            }
        }

        public IDbConnection GetDbConnection()
        {
            // vamos a evaluar si este existe o no
            if(_connection == null){//si el objeto conexion es nulo entonces creame 1
                _connection = new SqlConnection(_configs.Value.DefaultConnection);// con esto se crea la cadena de conexion
            }//ne cesito ahora evaluar cual es el estado de esta cadena
            if(_connection.State != ConnectionState.Open){//si el estado es diferente a  Abierto()connectionState(si no esta abierto)
                _connection.Open();                       // le indico q lo habra
            }
            return _connection;
        }
    }
}