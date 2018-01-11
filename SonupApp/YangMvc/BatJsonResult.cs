using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace YangMvc
{
    //public class BatJsonResult : JsonResult
    //{
    //    public BatJsonResult(object value) : base(value) { }
    //    public BatJsonResult(object value, JsonSerializerSettings serializerSettings) : base(value, serializerSettings) { }
         
    //    private const string _dateFormat = "yyyy-MM-dd HH:mm";
          
    //    public async override Task ExecuteResultAsync(ActionContext context)
    //    {
    //        if (context == null)
    //        {
    //            throw new ArgumentNullException("context");
    //        }

    //        HttpResponse response = context.HttpContext.Response;

    //        if (!String.IsNullOrEmpty(ContentType))
    //        {
    //            response.ContentType = ContentType;
    //        }
    //        else
    //        {
    //            response.ContentType = "application/json";
    //        }
    //        //if (ContentEncoding != null)
    //        //{
    //        //    response.ContentEncoding = ContentEncoding;
    //        //}
          
    //        if (Value != null)
    //        {
    //            // Using Json.NET serializer
    //            var isoConvert = new IsoDateTimeConverter();
    //            DBNullCreationConverter nullConverter = new DBNullCreationConverter();

    //            isoConvert.DateTimeFormat = _dateFormat;

    //            JsonSerializerSettings settings = new JsonSerializerSettings { ContractResolver = new SpecialContractResolver() };
    //            settings.Converters.Add(isoConvert);
    //            settings.Converters.Add(nullConverter);

    //            string json = JsonConvert.SerializeObject(Value, Formatting.Indented, settings);
    //            await response.WriteAsync(json);
    //        } 
    //    }
    //}

    //public class DBNullCreationConverter : JsonConverter
    //{
    //    /// <summary>
    //    /// 是否允许转换
    //    /// </summary>
    //    public override bool CanConvert(Type objectType)
    //    {
    //        bool canConvert = false;
    //        switch (objectType.FullName)
    //        {
    //            case "System.DBNull":

    //                canConvert = true;
    //                break;
    //        }
    //        return canConvert;
    //    }

    //    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    //    {
    //        return existingValue;
    //    }

    //    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    //    {
    //        writer.WriteValue(string.Empty);
    //    }

    //    public override bool CanRead
    //    {
    //        get
    //        {
    //            return false;
    //        }
    //    }
    //    /// <summary>
    //    /// 是否允许转换JSON字符串时调用
    //    /// </summary>
    //    public override bool CanWrite
    //    {
    //        get
    //        {
    //            return true;
    //        }
    //    }
    //}


    //public class NullableValueProvider : Newtonsoft.Json.Serialization.IValueProvider
    //{
    //    private readonly object _defaultValue;
    //    private readonly Newtonsoft.Json.Serialization.IValueProvider _underlyingValueProvider;


    //    public NullableValueProvider(MemberInfo memberInfo, Type underlyingType)
    //    {
    //        _underlyingValueProvider = new DynamicValueProvider(memberInfo);
    //        if (underlyingType.FullName == typeof(string).FullName)
    //        {
    //            _defaultValue = "";
    //        }
    //        else if (underlyingType.FullName == typeof(DateTime).FullName)
    //        {
    //            _defaultValue = null;
    //        }
    //        else
    //        {
    //            _defaultValue = Activator.CreateInstance(underlyingType);
    //        }
    //    }

    //    public void SetValue(object target, object value)
    //    {
    //        _underlyingValueProvider.SetValue(target, value);
    //    }

    //    public object GetValue(object target)
    //    {
    //        return _underlyingValueProvider.GetValue(target) ?? _defaultValue;
    //    }

    //    public bool ContainsPrefix(string prefix)
    //    {
    //        return false;
    //    }

    //    public ValueProviderResult GetValue(string key)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
     
    //public class SpecialContractResolver : DefaultContractResolver
    //{
    //    protected override Newtonsoft.Json.Serialization.IValueProvider CreateMemberValueProvider(MemberInfo member)
    //    {
    //        Type st = typeof(string);
    //        if (member.MemberType == MemberTypes.Property)
    //        {
    //            var pi = (PropertyInfo)member;
    //            // 处理所有可空泛型
    //            //if (pi.PropertyType.IsGenericType && pi.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //            //{
    //            //    return new NullableValueProvider(member, pi.PropertyType.GetGenericArguments().First());
    //            //}
    //            if (pi.PropertyType.FullName == st.FullName)
    //            {
    //                return new NullableValueProvider(member, st);
    //            }
    //        }
    //        else if (member.MemberType == MemberTypes.Field)
    //        {
    //            var fi = (FieldInfo)member;

    //            // 处理所有可空泛型
    //            //if (fi.FieldType.IsGenericType && fi.FieldType.GetGenericTypeDefinition() == typeof(Nullable<>))
    //            //{
    //            //    return new NullableValueProvider(member, fi.FieldType.GetGenericArguments().First());
    //            //}

    //            if (fi.FieldType.FullName == st.FullName)
    //            {
    //                return new NullableValueProvider(member, st);
    //            }
    //        }

    //        return base.CreateMemberValueProvider(member);
    //    }
    //}
}
