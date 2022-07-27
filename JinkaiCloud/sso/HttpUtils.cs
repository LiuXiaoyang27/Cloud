using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace Cloud.sso
{
    public class HttpUtils
    {
        /// <summary>
        /// HTTP Post请求
        /// </summary>
        /// <param name="uri">HTTP接口Url，不带协议和端口，如/artemis/api/resource/v1/org/advance/orgList</param>
        /// <param name="body">请求参数</param>
        /// <param name="timeout">请求超时时间，单位：秒</param>
        /// <return>请求结果</return>
        public static byte[] HttpPost(string uri, string body, int timeout)
        {

            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            GC.WaitForPendingFinalizers();


            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            StreamReader reader = null;
            Stream rspStream = null;
            //byte[] result = null;

            try
            {
                // 创建POST请求
                request = (HttpWebRequest)WebRequest.Create(uri.ToString());
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version11;
                request.AllowAutoRedirect = false;   // 不允许自动重定向
                request.Method = "POST";
                request.Timeout = timeout * 1000;    // 传入是秒，需要转换成毫秒
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded";

                if (!string.IsNullOrWhiteSpace(body))
                {
                    byte[] data = Encoding.UTF8.GetBytes(body);
                    request.ContentLength = data.Length;
                    try
                    {
                        reqStream = request.GetRequestStream();
                        reqStream.Write(data, 0, data.Length);
                        reqStream.Close();
                    }
                    catch (WebException)
                    {
                        if (reqStream != null)
                        {
                            reqStream.Close();
                        }

                        return null;
                    }
                }
                response = (HttpWebResponse)request.GetResponse();
                if (HttpStatusCode.OK == response.StatusCode)
                {
                    rspStream = response.GetResponseStream();
                    reader = new StreamReader(rspStream);
                    string strStream = reader.ReadToEnd();
                    long streamLength = strStream.Length;
                    byte[] result = System.Text.Encoding.UTF8.GetBytes(strStream);
                    response.Close();
                    return result;
                }

                response.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (reqStream != null)
                {
                    reqStream.Dispose();
                    reqStream.Close();
                }
                if (rspStream != null)
                {
                    rspStream.Dispose();
                    rspStream.Close();
                }
                if (reader != null)
                {
                    reader.Dispose();
                    reader.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return null;
        }

        /// <summary>
        /// HTTP Post请求
        /// </summary>
        /// <param name="uri">HTTP接口Url，不带协议和端口，如/artemis/api/resource/v1/org/advance/orgList</param>
        /// <param name="body">请求参数</param>
        /// <param name="timeout">请求超时时间，单位：秒</param>
        /// <return>请求结果</return>
        public static byte[] HttpGet(string uri, int timeout)
        {

            GC.Collect();//垃圾回收，回收没有正常关闭的http连接
            GC.WaitForPendingFinalizers();


            HttpWebRequest request = null;
            HttpWebResponse response = null;
            Stream reqStream = null;

            StreamReader reader = null;
            Stream rspStream = null;
            //byte[] result = null;

            try
            {
                // 创建POST请求
                request = (HttpWebRequest)WebRequest.Create(uri.ToString());
                request.KeepAlive = false;
                request.ProtocolVersion = HttpVersion.Version11;
                request.AllowAutoRedirect = false;   // 不允许自动重定向
                request.Method = "GET";
                request.Timeout = timeout * 1000;    // 传入是秒，需要转换成毫秒
                request.Accept = "*/*";
                request.ContentType = "application/x-www-form-urlencoded";

                response = (HttpWebResponse)request.GetResponse();

                if (response != null && HttpStatusCode.OK == response.StatusCode)
                {
                    rspStream = response.GetResponseStream();
                    reader = new StreamReader(rspStream);
                    string strStream = reader.ReadToEnd();
                    long streamLength = strStream.Length;
                    byte[] result = System.Text.Encoding.UTF8.GetBytes(strStream);
                    response.Close();
                    return result;
                }

                response.Close();
            }
            catch (Exception)
            {
                return null;
            }
            finally
            {
                //关闭连接和流
                if (response != null)
                {
                    response.Close();
                }
                if (reqStream != null)
                {
                    reqStream.Dispose();
                    reqStream.Close();
                }
                if (rspStream != null)
                {
                    rspStream.Dispose();
                    rspStream.Close();
                }
                if (reader != null)
                {
                    reader.Dispose();
                    reader.Close();
                }
                if (request != null)
                {
                    request.Abort();
                }
            }
            return null;
        }


        /// <summary>
        /// 获得所有的GET请求数据
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static string GetRequestBuilder(HttpContext context)
        {
            try
            {
                IDictionary<string, object> dic = context.Request.RequestContext.RouteData.Values;

                StringBuilder builder = new StringBuilder();
                foreach (var item in dic)
                {
                    builder.AppendFormat("key:{0}, value:{1}", item.Key, item.Value);
                    builder.AppendLine();
                }
                return builder.ToString();
            }
            catch (Exception ex)
            {
                return ex.Message.ToString();
            }

        }

        /// <summary>
        /// 正常响应报文JSON样例
        /// </summary>
        /// <returns></returns>
        public static string Success()
        {
            JObject result = new JObject();
            result["code"] = "success";
            return result.ToString();
        }

        /// <summary>
        /// 异常响应报文JSON样例
        /// </summary>
        /// <returns></returns>
        public static string Error(string errMsg)
        {
            JObject result = new JObject();
            result["code"] = "error";
            result["data"] = errMsg;
            return result.ToString();
        }
    }
}