using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace DncZeus.Api.Utils
{
    /// <summary>
    /// 网络操作
    /// </summary>
    public class Net
    {
        #region Ip(获取Ip)

        public static string Ip
        {
            get
            {
                string result = String.Empty;
                var httpContext = AutofacContainer.Resolve<IHttpContextAccessor>();
                if (httpContext != null)
                {
                    result = httpContext.HttpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
                    if (string.IsNullOrEmpty(result))
                    {
                        result = httpContext.HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString();
                    }

                    if (!string.IsNullOrEmpty(result))
                    {
                        //可能有代理
                        if (result.IndexOf(".") == -1)    //没有“.”肯定是非IPv4格式
                            result = null;
                        else
                        {
                            if (result.IndexOf(",") != -1)
                            {
                                //有“,”，估计多个代理。取第一个不是内网的IP。
                                result = result.Replace(" ", "").Replace("'", "");
                                string[] temparyip = result.Split(",;".ToCharArray());
                                for (int i = 0; i < temparyip.Length; i++)
                                {
                                    if (IsIpAddress(temparyip[i])
                                        && temparyip[i].Substring(0, 3) != "10."
                                        && temparyip[i].Substring(0, 7) != "192.168"
                                        && temparyip[i].Substring(0, 7) != "172.16.")
                                    {
                                        return temparyip[i];    //找到不是内网的地址
                                    }
                                }
                            }
                            else if (IsIpAddress(result)) //代理即是IP格式 ,IsIPAddress判断是否是IP的方法,
                                return result;
                            else
                                result = null;    //代理中的内容 非IP，取IP
                        }
                    }

                    if (string.IsNullOrEmpty(result))
                        result = httpContext.HttpContext.Request.Headers["X-Real-IP"].FirstOrDefault();

                    if (string.IsNullOrEmpty(result))
                        result = httpContext.HttpContext.Request.Host.Host;
                }
                return result;
            }
        }
        public static string LWIp
        {
            get
            {
                return System.Net.NetworkInformation.NetworkInterface.GetAllNetworkInterfaces()
  .Select(p => p.GetIPProperties())
  .SelectMany(p => p.UnicastAddresses)
  .Where(p => p.Address.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork && !System.Net.IPAddress.IsLoopback(p.Address))
  .FirstOrDefault()?.Address.ToString();
            }
        }


        public static bool IsIpAddress(string str)
        {
            if (string.IsNullOrWhiteSpace(str) || str.Length < 7 || str.Length > 15)
                return false;

            IPAddress ip;
            if (IPAddress.TryParse(str, out ip))
                return true;
            return false;
        }

        #endregion

        /// <summary>
        /// 获取IP地址信息
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static string GetLocation(string ip)
        {
            if (string.IsNullOrEmpty(ip) || ip.Equals("0.0.0.0") || ip.Equals("localhost") || ip.Equals("127.0.0.1"))
            {
                return string.Empty;
            }
            string res = "";
            try
            {
                string url = "http://apis.juhe.cn/ip/ip2addr?ip=" + ip + "&dtype=json&key=a86451534a6e72728b8cea430dabc633";
                res = HttpHelper.HttpGet(url);
                var resjson = res.ToObject<objex>();
                res = resjson.result.area + " " + resjson.result.location;
            }
            catch
            {
                res = "";
            }
            if (!string.IsNullOrEmpty(res))
            {
                return res;
            }
            try
            {
                string url = "https://sp0.baidu.com/8aQDcjqpAAV3otqbppnN2DJv/api.php?query=" + ip + "&resource_id=6006&ie=utf8&oe=gbk&format=json";
                res = HttpHelper.HttpGet(url);
                var resjson = res.ToObject<obj>();
                res = resjson.data[0].location;
            }
            catch
            {
                res = "";
            }
            return res;
        }

        #region Ip城市(获取Ip城市)

        /// <summary>
        /// 百度接口
        /// </summary>
        public class obj
        {
            public List<dataone> data { get; set; }
        }
        public class dataone
        {
            public string location { get; set; }
        }
        /// <summary>
        /// 聚合数据
        /// </summary>
        public class objex
        {
            public string resultcode { get; set; }
            public dataoneex result { get; set; }
            public string reason { get; set; }
            public string error_code { get; set; }
        }
        public class dataoneex
        {
            public string area { get; set; }
            public string location { get; set; }
        }
        #endregion

        #region Browser(获取浏览器信息)
        /// <summary>
        /// 获取浏览器信息
        /// </summary>
        public static string Browser
        {
            get
            {
                var httpContext = AutofacContainer.Resolve<IHttpContextAccessor>();
                if (httpContext == null)
                    return string.Empty;
                var browser = httpContext.HttpContext.Request.Headers["User-Agent"];
                return browser;
            }
        }

        #endregion

        /// <summary>
        /// 获取 请求域名
        /// </summary>
        public static string GetWww
        {
            get
            {
                var httpContext = AutofacContainer.Resolve<IHttpContextAccessor>();
                if (httpContext != null)
                {
                    return httpContext.HttpContext.Request.Host.Host;
                }

                return string.Empty;
            }

        }

        /// <summary>
        /// 获取 请求域名
        /// </summary>
        public static string GetOrigin
        {
            get
            {
                var httpContext = AutofacContainer.Resolve<IHttpContextAccessor>();
                if (httpContext != null)
                {
                    return httpContext.HttpContext.Request.Headers["Origin"];
                }

                return string.Empty;
            }

        }
    }
}
