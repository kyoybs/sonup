 
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
using BatCommon.Models.Apis;

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

        public T Model<T>() where T : BaseModel, new()
        {
            T model = new T();
            model.MvcContext = this.HttpContext;
            return model;
        }
        
        public CacheHelper Cache()
        {
            CacheHelper ch = new CacheHelper(HttpContext);
            return ch;
        }

        protected string CreateSn(int partnerId , string url)
        {
            var ptn = Model<PartnerModel>().GetPartner(partnerId);
            string pk = ptn.PartnerSecret;
            int pos = url.IndexOf("&sn=");
            if (pos > 0)
            {
                url = url.Substring(0, pos);
            }
            return HttpUtility.UrlEncode( url.ToLower().EncryBat(pk));
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

        public T Model<T>() where T : BaseModel, new()
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
        public SysUser LoginUser
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
        public SysUser LoginUser
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
