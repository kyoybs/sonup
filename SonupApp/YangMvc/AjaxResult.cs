using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;

namespace YangMvc
{
    public class AjaxResult
    {
        public bool IsSuccess { get; set; }

        public object Entity { get; set; }

        public string Message { get; set; }

        public string MoreInfo { get; set; }

        public AjaxResult() { }

        public AjaxResult(object entity)
        {
            if (entity != null)
            {
                this.Entity = entity;
                IsSuccess = true;
            }
            else
            {
                IsSuccess = false;
            }
        }

        public AjaxResult(bool isSuccess, string message = "")
        {
            this.IsSuccess = isSuccess;
            this.Message = message;
        }

        public static JsonResult Json(object obj, string moreInfo = null)
        { 
            AjaxResult result = new AjaxResult(obj);
            if (moreInfo != null)
                result.MoreInfo = moreInfo;
            JsonResult jr = new JsonResult(result);
            return jr;
        }

        public static JsonResult JsonError(string errorMsg)
        { 
            AjaxResult result = new AjaxResult(false, errorMsg);
            JsonResult jr = new JsonResult(result);
            return jr;
        }
    }
}
