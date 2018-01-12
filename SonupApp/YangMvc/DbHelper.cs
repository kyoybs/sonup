using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Dapper;
using Dapper.Contrib.Extensions;
using Microsoft.AspNetCore.Http;
using MySql.Data.MySqlClient;
using System.Data.Common;

namespace YangMvc
{
    public class DbBase
    {
        protected DbSettings Settings { get; set; } = new DbSettings();

        protected DbBase() { }

        public static DbBase Create(HttpContext context, DbServer dbServer = DbServer.MainDb, int timeout = 10)
        {
            DbBase db = new DbBase();
            db.Settings.Server = dbServer;
            db.Settings.Timeout = timeout;
            db.MvcContext = context;
            return db;
        }

        private DatabaseType DbType;
        private DbProviderFactory DbFactory;
        private string ConnString;

        private IDbConnection GetConnection()
        {
            if (DbFactory == null)
            {
                //Check Config
                ConnString = Config.Get(this.Settings.Server.ToString());
                if (string.IsNullOrEmpty(ConnString))
                {
                    throw new ApplicationException("缺少数据库连接配置项: " + this.Settings.Server);
                }

                //Get Db Server Type
                string dbType = Config.Get("MainDbType");
                DbType = "MySql".EqualString(dbType) ? DatabaseType.MySql : DatabaseType.SqlServer;


                if (DbType == DatabaseType.MySql)
                {
                    DbFactory = new MySqlClientFactory();
                    //return new MySqlConnection(connStr);
                }
                else
                {
                    DbFactory = SqlClientFactory.Instance;
                }
                //return new SqlConnection(connStr);
            }

            var conn = DbFactory.CreateConnection();
            conn.ConnectionString = ConnString;
            return conn;
        }

        private DbDataAdapter GetAdapter(IDbCommand command = null)
        {
            var adapter = DbFactory.CreateDataAdapter();
            if (command != null)
            {
                adapter.SelectCommand = (DbCommand)command;
            }
            return adapter;
        }

        private IDataParameter GetParameter(string name , object value)
        {
            var parameter = DbFactory.CreateParameter();
            parameter.ParameterName = name;
            parameter.Value = value;
            return parameter;
            //if (DbType == DatabaseType.SqlServer)
            //    return new SqlParameter(name, value); 
            //return new MySqlParameter(name, value);
        }

        private void AddParameter(IDbCommand comm , string name , object value)
        {
            comm.Parameters.Add(GetParameter(name , value ));
        }
             
        protected T FirstOrDefault<T>(string sql, object param)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return conn.QueryFirstOrDefault<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected List<T> Query<T>(string sql, object param)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return conn.Query<T>(sql, param).ToList();
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected async Task<List<T>> QueryAsync<T>(string sql, object param)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return (await conn.QueryAsync<T>(sql, param)).ToList();
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected void LogError(string sql, object param, Exception ex)
        {
            if (param is SysErrorLog)
                return;

            SysErrorLog log = new SysErrorLog();
            log.IsSql = true;
            log.LogTitle = ex.Message;
            log.LogContent = sql;
            log.FormData = param?.ToJsonBat();
            log.LogTime = DateTime.Now;
            log.RequestUrl = "";

            this.SaveError = false;

            Insert(log);
        }

        protected List<dynamic> Query(string sql, object param, DbServer dbName = DbServer.MainDb)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return conn.Query(sql, param).ToList();
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected DataTable QueryDataTable(string sql, object param, DbServer dbName = DbServer.MainDb)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    var comm = conn.CreateCommand();
                    comm.CommandText = sql;

                    if (param != null)
                    {
                        Type type = param.GetType();
                        var props = type.GetProperties();
                        foreach (var item in props)
                        {
                            AddParameter(comm, item.Name, item.GetValue(param, null));
                        }
                    }

                    var sda = GetAdapter(comm);  
                    DataTable dt = new DataTable();
                    sda.Fill(dt);
                    return dt;
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected List<T> Query<T>(string sql, object param, Pager pageInfo, DbServer dbName = DbServer.MainDb)
        {
            if (string.IsNullOrEmpty(pageInfo.OrderBy))
            {
                throw new ApplicationException("Dev:分页需要设置OrderBy");
            }

            try
            {
                string sqlCount = $"SELECT COUNT(*) FROM ( {sql} ) A";
                pageInfo.RowCount = ExecuteScalar<int>(sqlCount, param);

                if (pageInfo.PageSize == 0)
                {
                    sql = $"SELECT TOP 600 * FROM ( {sql} ) A ORDER BY {pageInfo.OrderBy} ";
                }
                else
                {
                    sql = $@"SELECT * FROM (
SELECT ROW_NUMBER() OVER(ORDER BY {pageInfo.OrderBy}) AS RowNo , * FROM (
{sql} ) A  ) B WHERE B.RowNo > {pageInfo.PageSize * pageInfo.PageIndex} AND B.RowNo <={pageInfo.PageSize * (pageInfo.PageIndex + 1)} ";

                }

                return Query<T>(sql, param);
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected int Execute(string sql, object param, DbServer dbName = DbServer.MainDb)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return conn.Execute(sql, param);
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected async Task<int> ExecuteAsync(string sql, object param, DbServer dbName = DbServer.MainDb)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return await conn.ExecuteAsync(sql, param);
                }
            }
            catch (Exception ex)
            {
                LogError(sql, param, ex);
                throw ex;
            }
        }

        protected T ExecuteScalar<T>(string sql, object param, DbServer dbName = DbServer.MainDb)
        {
            try
            {
                using (IDbConnection conn = GetConnection())
                {
                    return conn.ExecuteScalar<T>(sql, param);
                }
            }
            catch (Exception ex)
            {
                if (SaveError == true)
                {
                    LogError(sql, param, ex);
                }
                throw ex;
            }
        }

        private static Dictionary<string, string> DbConnetions = new Dictionary<string, string>();

        private string GetDbConnectionString()
        {
            string key = Settings.Server.ToString();
            return Config.Get(key);
        }

        protected T Get<T>(int id) where T : class
        {
            using (IDbConnection conn = GetConnection())
            {
                return conn.Get<T>(id);
            }
        }

        protected void Insert<T>(T entity) where T : class
        {
            using (IDbConnection conn = GetConnection())
            {
                conn.Insert(entity);
            }
        }

        protected bool SaveError { get; set; } = true;

        public HttpContext MvcContext { get; set; }
    }

    public class DbSettings
    {
        public DbServer Server { get; set; }

        /// <summary>
        /// Seconds
        /// </summary>
        public int Timeout { get; set; }
    }

    public enum DbServer
    {
        MainDb = 1,
        SecondDb = 2,
        MainMongo = 3,
        GpsLite = 4
    }

    public enum DatabaseType
    {
        SqlServer = 1, 
        MySql = 2
    }
}
