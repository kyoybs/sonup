using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using System.Configuration;
using Dapper;
using Dapper.Contrib.Extensions;

namespace YangMvc
{
    public class DbModel
    {
        public DbSettings Settings { get; set; } = new DbSettings();

        protected DbModel() { }

        public static DbModel Create(DbServer dbServer = DbServer.BizDb, int timeout = 10)
        {
            DbModel db = new DbModel();
            db.Settings.Server = dbServer;
            db.Settings.Timeout = timeout;
            return db;
        }

        public T FirstOrDefault<T>(string sql, object param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        public List<T> Query<T>(string sql, object param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        public async Task<List<T>> QueryAsync<T>(string sql, object param)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        public void LogError(string sql, object param, Exception ex)
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

        public List<dynamic> Query(string sql, object param, DbServer dbName = DbServer.BizDb)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        protected DataTable QueryDataTable(string sql, object param, DbServer dbName = DbServer.BizDb)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
                {
                    var comm = conn.CreateCommand();
                    comm.CommandText = sql;

                    if (param != null)
                    {
                        Type type = param.GetType();
                        var props = type.GetProperties();
                        foreach (var item in props)
                        {
                            comm.Parameters.AddWithValue(item.Name, item.GetValue(param, null));
                        }
                    }

                    SqlDataAdapter sda = new SqlDataAdapter(comm);
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

        public SqlConnection GetDbConnection(DbServer dbName = DbServer.BizDb)
        {
            return new SqlConnection(GetDbConnectionString());
        }

        public List<T> Query<T>(string sql, object param, Pager pageInfo, DbServer dbName = DbServer.BizDb)
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

        public int Execute(string sql, object param, DbServer dbName = DbServer.BizDb)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        public async Task<int> ExecuteAsync(string sql, object param, DbServer dbName = DbServer.BizDb)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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

        public T ExecuteScalar<T>(string sql, object param, DbServer dbName = DbServer.BizDb)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
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
            if (DbConnetions.ContainsKey(key))
            {
                return DbConnetions[key];
            }
            else
            {
                DbConnetions = new Dictionary<string, string>();

                DbConnetions.Add("BizDb", Config.Get("BizDb"));
                DbConnetions.Add("GpsDb", Config.Get("GpsDb"));

                if (!DbConnetions.ContainsKey(key))
                    throw new ApplicationException($"配置文件中没有{key}");

                return DbConnetions[key];
            }
        }

        public T Get<T>(int id) where T : class
        {
            using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
            {
                return conn.Get<T>(id);
            }
        }

        public void Insert<T>(T entity) where T : class
        {
            using (SqlConnection conn = new SqlConnection(GetDbConnectionString()))
            {
                conn.Insert(entity);
            }
        }

        public bool SaveError { get; set; } = true;

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
        BizDb = 1,
        GpsDb = 2,
        MainMongo = 3,
        GpsLite = 4
    }


}
