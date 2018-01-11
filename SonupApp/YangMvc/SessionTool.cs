 
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;

namespace YangMvc
{
    public class SessionTool
    {
        public static string LoginUserKey = "LoginUser";

        private ISession Session;

        public SessionTool(ISession session)
        {
            this.Session = session;
        }

        public void SetInt(string key, int value)
        {
            Session.SetInt32(key, value);
        }

        public void SetString(string key , string value)
        {
            Session.SetString(key, value);
        }

        public void SetObject<T>(string key ,T value)
        {
            if(value == null)
            {
                Session.Remove(key);
            }
            string json = value.ToJsonBat();
            var bytes = Encoding.UTF8.GetBytes(json);
            Session.Set(key, bytes);
        }

        public int? GetInt(string key)
        {
            return Session.GetInt32(key);
        }

        public string GetString(string key)
        {
            return Session.GetString(key);
        }

        public T GetObject<T>(string key)
        {
            var bytes = Session.Get(key);
            if (bytes == null)
                return default(T);

            string json = Encoding.UTF8.GetString(bytes);
            return json.JsonToObject<T>();
        }

        public AppUser GetLoginUser()
        {
            return GetObject<AppUser>(LoginUserKey);
        }

        public void SetLoginUser(AppUser loginUser)
        {
            SetObject(LoginUserKey, loginUser);
        }
    }
}
