using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using SonupApp.Models;
using SonupApp.Models.Sys;
using YangMvc;

namespace SonupApp.Controllers
{
    public class HomeController : BaseController
    {
        private UserDb UserDb => Db<UserDb>();

        public IActionResult Index()
        {
            string scode = Guid.NewGuid().ToString();
            ViewBag.ServerCode = scode;
            Session().SetString("LoginValidCode", scode);
            return View();
        }

        public IActionResult Login()
        {
            return View();
        }
         
        [HttpPost]
        public async Task<JsonResult> Login(long ticks, string userName, string password, long ccode, string scode,
            [FromServices] UserManager<AppUser> _UserManager)
        {
            //验证ServerCode
            string sessionCode = Session().GetString("LoginValidCode");
            if (sessionCode != scode)
            {
                return AjaxResult.JsonError("无效的请求 - 1");
            }

            //check client code
            long clientCode = userName.Length * password.Length * 3 * DateTime.Today.Day;
            if (ccode != clientCode)
            {
                return AjaxResult.JsonError("无效的请求 - 2");
            }

            ////check time
            //var dt = new DateTime(ticks);
            //if (Math.Abs((dt - DateTime.Now).TotalMinutes) > 30)
            //{
            //    return AjaxResult.JsonError("无效的请求 - 3");
            //}

            var user = UserDb.GetUser(userName);
            if(user == null)
            {
                return AjaxResult.JsonError("用户名不存在.");
            }

            if(user.Password != password)
            {
                return AjaxResult.JsonError("密码错误.");
            }

            Session().SetLoginUser(user);

            await _UserManager.CreateAsync(user);

            return AjaxResult.Json("ok");
        }

        [Authorize]
        public IActionResult Memory()
        {
            return View();
        }

        public IActionResult About()
        {
            ViewData["Message"] = "Your application description page.";

            return View();
        }

        public IActionResult Contact()
        {
            ViewData["Message"] = "Your contact page.";

            return View();
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
