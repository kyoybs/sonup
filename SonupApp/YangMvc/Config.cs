using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using ShareCarApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Serialization;
using System.Data.SqlClient;
using Dapper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;

namespace YangMvc
{
    public static class Config
    {
        /// <summary>
        /// 在 Startup 构造方法中调用
        /// </summary>
        /// <param name="config"></param>
        public static void InitConfig(IConfiguration config)
        {
            Config.Configuration = config; 
        }

        /// <summary>
        /// 在 Startup.ConfigureServices() 方法中调用
        /// </summary>
        /// <param name="services"></param>
        public static void InitServices(IServiceCollection services, string cacheConfigKeyMssql = "AppSettings:MainDb")
        {
            services.AddIdentity<AppUser, AppRole>()
                .AddDefaultTokenProviders();

            services.AddTransient(typeof(IUserStore<AppUser>), typeof( YangUserStore));

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme) 
                .AddCookie(options =>
                {
                    options.LoginPath = "/Home/Index";
                });

            services.AddMvc().AddJsonOptions(opts => {
                opts.SerializerSettings.ContractResolver = new DefaultContractResolver();
            });

            services.AddSession();

            if (string.IsNullOrEmpty(cacheConfigKeyMssql))
            {
                services.AddDistributedMemoryCache();
            }
            else
            {
                string sqlConnStr = Configuration[cacheConfigKeyMssql];
                InitSessionDB(sqlConnStr);
                services.AddDistributedSqlServerCache(options =>
                {
                    options.ConnectionString = sqlConnStr;
                    options.SchemaName = "dbo";
                    options.TableName = "Sessions";
                });
            } 
        }

        public static void InitMvc(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseAuthentication();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();

            app.UseSession();

            app.UseMvc(routes =>
            {
                routes.MapRoute(name: "adminArea",
                                template: "{area:exists}/{controller=Admin}/{action=Index}");

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }

        private static void InitSessionDB(string dbConnection)
        {
            string sql = @"
IF OBJECT_ID('Sessions') IS NULL
CREATE TABLE [dbo].[Sessions](
	[Id] [nvarchar](900) NOT NULL,
	[Value] [varbinary](max) NOT NULL,
	[ExpiresAtTime] [datetimeoffset](7) NOT NULL,
	[SlidingExpirationInSeconds] [bigint] NULL,
	[AbsoluteExpiration] [datetimeoffset](7) NULL,
 CONSTRAINT [pk_Id] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY] ";
            using (SqlConnection conn = new SqlConnection())
            {
                conn.Execute(sql);
            }
        }
         
        private static IConfiguration Configuration { get; set; }

        public static string Get(string key, string section = "AppSettings")
        {
            return Configuration[$"{section}:{key}"];
        }
    }
}
