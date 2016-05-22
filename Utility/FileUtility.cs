using System;
using System.IO;
using System.Collections;
using System.Xml;
using System.Reflection;
using System.ComponentModel;
using System.Text;

namespace Utility
{
    class FileUtility
    {

        /// <summary>
        /// 检测文件是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CK_FilePath(string url)
        {
            if (url == "") return false;
            return File.Exists(url);
        }
        /// <summary>
        /// 检测目录是否存在
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static bool CK_FolderPath(string url)
        {
            if (url == "") return false;
            return Directory.Exists(url);
        }
        //读取文件内容
        public static string Re_FilePath(string url)
        {
            if (!CK_FilePath(url)) return "";
            string str = "";
            using (StreamReader oStream = new StreamReader(url, System.Text.Encoding.GetEncoding("utf-8")))
            {
                str = oStream.ReadToEnd();
                oStream.Close();
                oStream.Dispose();
                return str.Trim();
            }
        }
        /// <summary>
        /// 检测文件是否存在不存在则创建文件并写入
        /// </summary>
        /// <returns></returns>
        public static bool We_FilePath(string file, string fileName, string fix, string value)
        {
            try
            {
                CreateFolder(file);
                bool ow = true;
                string fileNameURL = "";
                fileNameURL = file + "//" + fileName + "." + fix;
                if (CK_FilePath(fileNameURL))
                {
                    return We_FilePath(value, fileNameURL);
                }
                else
                {
                    using (StreamWriter oWrite = File.CreateText(fileNameURL))
                    {
                        oWrite.Write(value.ToCharArray());
                        oWrite.Close();
                        oWrite.Dispose();
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }
        public static void CreateFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
        }
        public static bool We_FilePath(string value, string url)
        {
            try
            {
                using (StreamWriter oWrite = new StreamWriter(url, false, Encoding.GetEncoding("utf-8")))
                {
                    oWrite.Write(value.ToCharArray());
                    oWrite.Close();
                    oWrite.Dispose();
                    return true;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /// <summary>
        /// 获取源文件内容
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public static string ReadFile(string Path)
        {
            string s = "";
            if (!File.Exists(Path))
                s = "不存在相应的目录";
            else
            {
                StreamReader f2 = new StreamReader(Path, System.Text.Encoding.GetEncoding("utf-8"));
                s = f2.ReadToEnd();
                f2.Close();
                f2.Dispose();
            }
            return s;
        }
        /// <summary>
        /// 检查创建目录
        /// </summary>
        public static bool createFolder(string path)
        {
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
                return true;
            }
            return true;
        }
        public static bool createFile(string path)
        {
            if (!File.Exists(path))
            {
                File.CreateText(path);
                return true;
            }
            return true;
        }
        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool deleFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
                if (!File.Exists(path))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
        public string C(Hashtable t, string str)
        {
            string _loop = str;
            foreach (string s in t.Keys)
            {
                _loop = _loop.Replace(s, t[s].ToString());
            }
            return _loop;
        }
    }
}

