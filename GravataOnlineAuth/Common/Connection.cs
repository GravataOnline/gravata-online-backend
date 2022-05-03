using Dapper;
using Dapper.Contrib.Extensions;
using GravataOnlineAuth.Models.User;
using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace GravataOnlineAuth.Common
{
    public class Connections
    {
        private static string _connectionString;

        public static void SetMainConnection()
        {
            _connectionString = Base.CONNECTIONSTRING;
        }

        public static void SetAuthConnection()
        {
            _connectionString = Base.AUTHCONNECTIONSTRING;
        }


        private IDbConnection GetConnection()
        {
            try
            {
                IDbConnection connection = null;
                {
                    switch (Base.DBTYPE)
                    {
                        case "SQL":
                            connection = new SqlConnection(_connectionString);
                            break;
                        case "ORACLE":
                            connection = new OracleConnection(_connectionString);
                            break;
                    }
                }
                connection.Open();
                return connection;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected IEnumerable<T> Query<T>(string query)
        {
            IEnumerable<T> resultado = null;
            try
            {
                using (var connection = GetConnection())
                {
                    resultado = connection.Query<T>(query);
                    connection.Close();
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected IEnumerable<T> Query<T>(string consulta, object parametros = null, CommandType tipoComando = CommandType.StoredProcedure, bool comBuffer = true)
        {
            IEnumerable<T> resultado = null;
            try
            {

                using (var connection = GetConnection())
                {
                    resultado = connection.Query<T>(consulta, parametros, buffered: comBuffer, commandType: tipoComando);
                    connection.Close();
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected IEnumerable<T> Query<T>(Dictionary<string, object> query)
        {
            IEnumerable<T> resultado = null;
            try
            {
                using (var connection = GetConnection())
                {
                    foreach (KeyValuePair<string, object> t in query)
                    {
                        resultado = connection.Query<T>(t.Key, t.Value);
                        connection.Close();
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected IEnumerable<T> QueryMap<T>(Dictionary<string, object> query)
        {
            IEnumerable<T> resultado = null;
            try
            {
                using (var connection = GetConnection())
                {

                    foreach (KeyValuePair<string, object> t in query)
                    {
                        IEnumerable<dynamic> dados = connection.Query<dynamic>(t.Key, t.Value);
                        resultado = (Slapper.AutoMapper.MapDynamic<T>(dados) as IEnumerable<T>);
                        Slapper.AutoMapper.Cache.ClearAllCaches();
                        connection.Close();
                    }
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected IEnumerable<T> QueryMap<T>(string query)
        {
            IEnumerable<T> resultado = null;
            try
            {
                using (var connection = GetConnection())
                {
                    IEnumerable<dynamic> dados = connection.Query<dynamic>(query);
                    resultado = (Slapper.AutoMapper.MapDynamic<T>(dados) as IEnumerable<T>);
                    Slapper.AutoMapper.Cache.ClearAllCaches();
                    connection.Close();
                }
                return resultado;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }


        protected string Execute(string query)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Execute(query, transaction: transaction);
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected string Execute(string query, object parametros)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Execute(query, param: parametros, transaction: transaction);
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected void Insert<T>(T data) where T : class
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Insert<T>(data);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        protected void Update<T>(object data)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Update(data, transaction: transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        protected void Delete<T>(object data) where T : class
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Delete(data, transaction: transaction);
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        protected T Get<T>(int id) where T : class
        {
            using (var connection = GetConnection())
            {
                try
                {
                    return connection.Get<T>(id);
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }

        protected IEnumerable<T> GetAll<T>() where T : class
        {
            using (var connection = GetConnection())
            {
                try
                {
                    return connection.GetAll<T>();
                }
                catch (Exception ex)
                {
                    throw new Exception(ex.Message);
                }
            }
        }



        protected string Execute(string procedure, object parametros, CommandType tipoComando = CommandType.Text)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    connection.Execute(procedure, param: parametros, transaction: transaction, commandType: tipoComando);
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected string Execute(Dictionary<string, object> query)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (KeyValuePair<string, object> t in query)
                    {
                        connection.Execute(t.Key, t.Value, transaction: transaction);
                    }
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected string Execute(Dictionary<object, string> query)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (KeyValuePair<object, string> t in query)
                    {
                        connection.Execute(t.Value, t.Key, transaction: transaction);
                    }
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        protected string Execute(List<Dictionary<string, object>> queries)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var query in queries)
                    {
                        foreach (KeyValuePair<string, object> t in query)
                        {
                            connection.Execute(t.Key, t.Value, transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        protected string Execute(List<Dictionary<object, string>> queries)
        {
            using (var connection = GetConnection())
            {
                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (var query in queries)
                    {
                        foreach (KeyValuePair<object, string> t in query)
                        {
                            connection.Execute(t.Value, t.Key, transaction: transaction);
                        }
                    }
                    transaction.Commit();
                    connection.Close();

                    return "OK";
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public IEnumerable<int> ExecuteReturning<T>(Dictionary<string, object> query)
        {
            using (var connection = GetConnection())
            {
                List<int> retorno = new List<int>();

                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (KeyValuePair<string, object> t in query)
                    {
                        var param = new DynamicParameters(t.Value);
                        param.Add(name: "ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute(t.Key, param, transaction: transaction);

                        retorno.Add(param.Get<int>("ID"));
                    }
                    transaction.Commit();
                    connection.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

        public IEnumerable<int> ExecuteReturning<T>(Dictionary<object, string> query)
        {
            using (var connection = GetConnection())
            {
                List<int> retorno = new List<int>();

                var transaction = connection.BeginTransaction();
                try
                {
                    foreach (KeyValuePair<object, string> t in query)
                    {
                        var param = new DynamicParameters(t.Key);
                        param.Add(name: "ID", dbType: DbType.Int32, direction: ParameterDirection.Output);
                        connection.Execute(t.Value, param, transaction: transaction);

                        retorno.Add(param.Get<int>("ID"));
                    }
                    transaction.Commit();
                    connection.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected IEnumerable<T> ExecuteReturning<T>(string query, object parametros)
        {
            using (var connection = GetConnection())
            {
                IEnumerable<T> retorno;
                var transaction = connection.BeginTransaction();
                try
                {
                    retorno = connection.Query<T>(query, param: parametros, transaction: transaction);
                    transaction.Commit();
                    connection.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }


        protected IEnumerable<T> ExecuteReturning<T>(string procedure, object parametros, CommandType tipoComando = CommandType.Text)
        {
            using (var connection = GetConnection())
            {
                IEnumerable<T> retorno;
                var transaction = connection.BeginTransaction();
                try
                {
                    retorno = connection.Query<T>(procedure, param: parametros, transaction: transaction, commandType: tipoComando);
                    transaction.Commit();
                    connection.Close();

                    return retorno;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    throw new Exception(ex.Message);
                }
            }
        }

    }
}
