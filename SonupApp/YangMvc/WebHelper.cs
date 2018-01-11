using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;

namespace YangMvc
{
    public class WebHelper
    {
        public static string HttpGetJson(string url)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(url);

                if (url.StartsWith("https", StringComparison.CurrentCultureIgnoreCase))
                {
                    request.ProtocolVersion = HttpVersion.Version10;
                }

                request.ContentType = "application/json";
                request.Method = "GET";
                request.Timeout = 20000;

                HttpWebResponse httpWebResponse = (HttpWebResponse)request.GetResponse();
                StreamReader streamReader = new StreamReader(httpWebResponse.GetResponseStream());
                string responseContent = streamReader.ReadToEnd();
                httpWebResponse.Close();
                streamReader.Close();
                return responseContent;
            }
            catch (Exception ex)
            {
                throw new ApplicationException("Web Error: " + url, ex);
            }

        }

        public static string HttpPostJson(string Url, string strJson)
        {
            try
            {
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Url);

                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = Encoding.UTF8.GetByteCount(strJson);
                using (Stream myRequestStream = request.GetRequestStream())
                {
                    var bytes = Encoding.UTF8.GetBytes(strJson);
                    myRequestStream.Write(bytes, 0, bytes.Length); 
                }
                 
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();
                using (Stream myResponseStream = response.GetResponseStream())
                {
                    using (StreamReader myStreamReader = new StreamReader(myResponseStream, Encoding.GetEncoding("utf-8")))
                    {
                        string retString = myStreamReader.ReadToEnd(); 
                        return retString;
                    }
                } 
            }
            catch (Exception ex)
            {
                throw new ApplicationException("HttpPostJson Error:" + Url + " -- " + strJson, ex);
            }
        }
    }
}
