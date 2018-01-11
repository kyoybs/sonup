using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ShareCarApi.Common
{
    public class ApiResult 
    {
        public string ErrMsg { get; set; }

        public static ApiResult Error(string error)
        {
            ApiResult ar = new ApiResult();
            ar.ErrMsg = error;
            return ar;
        }
    }

    public class ApiResult<T>: ApiResult
    { 
        public T Entity { get; set; }

        public static ApiResult<T> Create(T entity)
        {
            ApiResult<T> ar = new ApiResult<T>();
            ar.Entity = entity;
            return ar;
        }
    }
}
