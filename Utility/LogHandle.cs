using System;
using System.IO;
using System.Xml.Linq;
using System.Xml;
using System.Threading;
using log4net;
using System.Web;
using System.Text.RegularExpressions;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Text;

namespace Utility
{
    public static class LogWriter
    {
        private static Mutex mtx = new Mutex();
        private static ILog log = LogManager.GetLogger(typeof(LogWriter));
        /// <summary>
        /// 日志
        /// </summary>
        public static ILog Log
        {
            get
            {
                return log;
            }
        }
        static AutoResetEvent Event1 = new AutoResetEvent(false);
        //add by xugh on 20150115--平台日志路径修改，改为读配置文件  --START
        private static string path = System.Configuration.ConfigurationManager.AppSettings["APPLOGPath"].ToString() == "" ? "D:\\onlinelog" : System.Configuration.ConfigurationManager.AppSettings["APPLOGPath"].ToString();
        //private static string path = AppDomain.CurrentDomain.SetupInformation.ApplicationBase + "\\applog\\";
        //add by xugh on 20150115--平台日志路径修改，改为读配置文件  -- END

        /// <summary>
        /// 获取堆栈String
        /// </summary>
        /// <returns></returns>
        public static string GetFramesString()
        {
            StringBuilder frames = new StringBuilder("\r\n-------------------业务堆栈Start----------------\r\n");
            foreach (StackFrame Frame in new StackTrace().GetFrames())
            {
                if (Frame.GetMethod().DeclaringType.ToString().StartsWith("System."))
                {//不打出系统堆栈
                    continue;
                }
                frames.AppendFormat("类：{0},方法：{1}\r\n", Frame.GetMethod().DeclaringType, Frame.GetMethod().Name);
            }
            frames.Append("------------------业务堆栈end------------------\r\n");
            return frames.ToString();
        }
        /// <summary>
        /// 写本地日志
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        public static void Write(string account, string comment)
        {
            WriteLog(account, comment);
        }
        /// <summary>
        /// 写本地日志
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        /// <param name="filename">文件名称</param>
        public static void Write(string account, string comment, string filename)
        {
            WriteLog(account, comment, filename);
        }
        /// <summary>
        /// 写本地日志 add by qinyue
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        /// <param name="filename">文件名称</param>
        /// <param name="moduleName">功能/模块名称</param>
        public static void Write(string account, string comment, string filename, string moduleName)
        {
            WriteLog(account, comment, filename, moduleName);
        }
        private static void WriteLog(string account, string comment)
        {
            try
            {
                log.Info("'" + DateTime.Now.ToString() + account + "执行了" + comment + GetFramesString());
            }
            catch (Exception ee)
            {
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + ee.Message);
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + ee.Source);
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + comment);
            }
        }

        /// <summary>
        /// 写本地日志 update by qinyue
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        /// <param name="filename">文件名称</param>
        private static void WriteLog(string account, string comment, string filename)
        {
            WriteLog(account, comment, filename, "");
        }

        /// <summary>
        /// 写本地日志 add by qinyue
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        /// <param name="filename">文件名称</param>
        /// <param name="moduleName">功能/模块名称</param>
        private static void WriteLog(string account, string comment, string filename, string moduleName)
        {
            try
            {
                string p = path + DateTime.Now.ToString("yyyy-MM-dd");
                if (!string.IsNullOrWhiteSpace(moduleName))
                {
                    p += "\\" + moduleName.Trim();
                }
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                p += "\\" + filename + ".config";
                //Write file here
                mtx.WaitOne();
                FileStream fs = new FileStream(p, FileMode.Append, FileAccess.Write, FileShare.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine("'" + DateTime.Now.ToString() + " " + account + "执行了" + comment);
                sw.Close();
                fs.Close();
                mtx.ReleaseMutex();
                Event1.Reset();

            }
            catch (Exception ee)
            {
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + ee.Message);
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + ee.Source);
                log.Error("'" + DateTime.Now.ToString() + account + "日志错误执行了" + comment);
            }
        }
        /// <summary>
        /// 写本地日志（文件名与目录名不包含日期） add by zhj
        /// </summary>
        /// <param name="account">账号信息/方法名称</param>
        /// <param name="comment">日志内容</param>
        /// <param name="filename">文件名称</param>
        /// <param name="moduleName">功能/模块名称</param>
        public static void WriteLogNoDateName(string account, string comment, string filename, string moduleName)
        {
            try
            {
                string p = path;
                if (!string.IsNullOrWhiteSpace(moduleName))
                {
                    p += "\\" + moduleName.Trim();

                }
                if (!Directory.Exists(p))
                {
                    Directory.CreateDirectory(p);
                }
                p += "\\" + filename + ".config";
                //Write file here
                mtx.WaitOne();
                FileStream fs = new FileStream(p, FileMode.Append, FileAccess.Write, FileShare.Write);
                System.IO.StreamWriter sw = new System.IO.StreamWriter(fs, System.Text.Encoding.Default);
                sw.WriteLine("'" + DateTime.Now.ToString() + " " + account + "执行了" + comment);
                sw.Close();
                fs.Close();
                mtx.ReleaseMutex();
                Event1.Reset();

            }
            catch (Exception ee)
            {
                log.Error("'" + DateTime.Now.ToString() + "'" + account + "'。日志错误消息：" + ee.Message + "。执行了：" + comment + "。跟踪：" + ee.StackTrace);
            }
        }
        public static void _Write(string FileName, string Account)
        {
            try
            {
                string Files = DateTime.Now.ToString("yyyy-MM-dd") + "_" + FileName + ".config";
                if (!Directory.Exists(path)) Directory.CreateDirectory(path);
                mtx.WaitOne();
                FileStream _Stream = new FileStream(path + Files, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (StreamWriter _Writer = new System.IO.StreamWriter(_Stream, System.Text.Encoding.Default))
                {
                    _Writer.WriteLine("'" + DateTime.Now.ToString() + "执行了" + Account);
                    _Writer.Close();
                    _Stream.Close();
                    mtx.ReleaseMutex();
                    Event1.Reset();
                }
            }
            catch (Exception ee)
            {
                log.Error("'" + DateTime.Now.ToString() + Account + "日志错误执行了" + ee.Message + "/" + ee.Source);
            }
        }
        /// <summary>
        /// 描述: 日志写入
        /// 设计: 雷聃
        /// 日期: 2014/04/09
        /// </summary>
        /// <param name="folder">文件夹名字</param>
        /// <param name="_object">错误对象</param>
        /// <param name="otherMessage">临时对象 如果Exception为null 则以otherMessage内容写入</param>
        public static void Import(string folder, Exception _object, string otherMessage)
        {
            if (null == _object && null == otherMessage) return;
            try
            {
                string fPath = path + folder;
                string fAddress = fPath + "\\" + DateTime.Now.ToString("yyyy-MM-dd") + @".config";
                string fContext = string.Empty;
                if (!Directory.Exists(fPath)) Directory.CreateDirectory(fPath);
                mtx.WaitOne();
                FileStream _Stream = new FileStream(fAddress, FileMode.Append, FileAccess.Write, FileShare.Write);
                using (StreamWriter _Writer = new System.IO.StreamWriter(_Stream, System.Text.Encoding.Default))
                {
                    if (_object == null)
                    {
                        _Writer.WriteLine(DateTime.Now.ToString() + ":" + otherMessage);
                        _Writer.WriteLine("");
                    }
                    else
                    {
                        _Writer.WriteLine("时间: " + DateTime.Now.ToString());
                        _Writer.WriteLine("当前异常的消息: " + StringFormat(_object.Message));
                        _Writer.WriteLine("导致错误的应用程序或对象的名称: " + _object.Source);
                        _Writer.WriteLine("调用堆栈的内容: " + StringFormat(_object.StackTrace));
                        _Writer.WriteLine("其他:" + (otherMessage == null ? "" : otherMessage));
                    }
                    _Writer.Close();
                    _Stream.Close();
                    mtx.ReleaseMutex();
                    Event1.Reset();
                }
            }
            catch (Exception ee)
            {
                log.Error("'" + DateTime.Now.ToString() + otherMessage + "日志错误执行了" + ee.Message + "/" + ee.Source);
            }
        }
        /// <summary>
        /// 描述: 格式化内容
        /// </summary>
        /// <param name="_string"></param>
        /// <returns></returns>
        private static string StringFormat(string _string)
        {
            return Regex.Replace(_string, @"[\n\r\{\}\u0022]", "", RegexOptions.Multiline);
        }
        public static void _Write(string Account)
        {
            _Write("G", Account);
        }
    }

    /// <summary>
    /// Cookie操作
    /// </summary>
    public class PCookie
    {
        /// <summary>
        /// 设置Cookie(名字 值)
        /// 默认保存时间是一天 暂时没加参数
        /// </summary>
        /// <param name="name"></param>
        /// <param name="value"></param>
        public static void setCookie(string name, string value)
        {
            HttpCookie cookie = new HttpCookie(name);
            DateTime now = DateTime.Now;
            cookie.Value = value;
            cookie.Expires = now.AddDays(24);
            cookie.HttpOnly = true;
            if (HttpContext.Current.Response.Cookies[name] != null)
            {
                HttpContext.Current.Response.Cookies.Remove(name);
            }
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
        /// <summary>
        /// 获取Cookie 不存在则为null
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static string getCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            cookie.HttpOnly = true;
            cookie = HttpContext.Current.Request.Cookies[name];
            if (cookie != null)
                return cookie.Value;
            else
                return null;
        }
        /// <summary>
        /// 清除Cookie
        /// </summary>
        /// <param name="name"></param>
        public static void clearCookie(string name)
        {
            HttpCookie cookie = new HttpCookie(name);
            DateTime now = DateTime.Now;
            cookie.Domain = //AppConfig.GetAppSetting("Domain");
            cookie.Value = "";
            cookie.Expires = now.AddYears(-2);
            HttpContext.Current.Response.Cookies.Add(cookie);
        }



    }
}
