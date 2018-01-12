 
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json;
using ShareCarApi.Common;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Http.Extensions;
using System.Web;
using System.Collections.Concurrent;

namespace YangMvc
{
    public class BaseController:Controller
    {
        private SessionTool _Session;

        public SessionTool Session()
        {
            if (_Session == null)
                _Session = new SessionTool(HttpContext.Session);
            return _Session;
        }

        private ConcurrentDictionary<string, DbBase> Dbs = new ConcurrentDictionary<string, DbBase>();

        public T Db<T>() where T : DbBase, new()
        {
            string type = typeof(T).FullName;
            if (!Dbs.ContainsKey(type))
            {
                T db = new T();
                db.MvcContext = this.HttpContext;
                Dbs.TryAdd(type, db);
                return db;
            }
            return Dbs[type] as T;
        }
        
        public CacheHelper Cache()
        {
            CacheHelper ch = new CacheHelper(HttpContext);
            return ch;
        }
  
        protected string GetUrl()
        {
            return this.Request.GetDisplayUrl();
        }
         
    }

    public class BasePageModel : PageModel
    {
        private SessionTool _Session;

        public SessionTool Session()
        {
            if (_Session == null)
                _Session = new SessionTool(HttpContext.Session);
            return _Session;
        }

        public T Db<T>() where T : DbBase, new()
        {
            T model = new T();
            model.MvcContext = this.HttpContext;
            return model;
        }

        protected string GetUrl()
        {
            return this.Request.GetDisplayUrl();
        }
    }

    public class AuthController: BaseController
    {
        public AppUser LoginUser
        {
            get
            {
                var user = Session().GetLoginUser();
                return user;
            }
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if(LoginUser == null)
            {
                context.Result = Redirect("~/Acc/Login");
            }
            base.OnActionExecuting(context);
        }
         
    }
    
    [AuthPageFilter]
    public class AuthPageModel : BasePageModel
    {
        public AppUser LoginUser
        {
            get
            {
                var user = Session().GetLoginUser();
                return user;
            }
        } 
    }

    public class AuthPageFilter : Attribute, IPageFilter
    {
        public void OnPageHandlerExecuted(PageHandlerExecutedContext context)
        { 
        }

        public void OnPageHandlerExecuting(PageHandlerExecutingContext context)
        {
            SessionTool st = new SessionTool(context.HttpContext.Session);
            if(st.GetLoginUser() == null)
            {
                context.Result = new RedirectResult("~/Acc/Login");
            }
        }

        public void OnPageHandlerSelected(PageHandlerSelectedContext context)
        { 
        }
    }
}
