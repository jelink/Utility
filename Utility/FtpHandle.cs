using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;

namespace Utility
{
    /// <summary>
    /// FTP管理类
    /// </summary>
    public class FtpHandle
    {
        /// <summary>
        /// 最大文件夹和文件名长度
        /// </summary>
        public const int MAX_NAME_LENGTH = 256;
        /// <summary>
        /// 最大目录深度
        /// </summary>
        public const int MAX_DIR_DEPTH = 256;
        /// <summary>
        /// 目录分割符号
        /// </summary>
        public const char DIR_SEPARATOR = '/';
        /// <summary>
        /// 上传文件
        /// </summary>
        /// <param name="fileinfo">需要上传的文件</param>
        /// <param name="targetDir">目标路径</param>
        /// <param name="hostname">ftp地址</param>
        /// <param name="username">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        public static void UploadFile(FileInfo fileinfo, string targetDir, string hostname, string username, string password)
        {
            try
            {
                LogWriter.Write("Ftp上传文件开始:2" + fileinfo, "目标路径：" + targetDir + ",ftp地址" + hostname, "FileUrl");
                //1. check target
                string target;
                if (targetDir.Trim() == "")
                {
                    return;
                }
                target = Guid.NewGuid().ToString();  //使用临时文件名

                if (ftpIsExistsPath(targetDir, hostname, username, password) == false)
                {
                    MakeDir(targetDir, hostname, username, password);
                }

                string URI = "FTP://" + hostname + "/" + targetDir + "/" + target;
                ///WebClient webcl = new WebClient();
                System.Net.FtpWebRequest ftp = GetRequest(URI, username, password);

                //设置FTP命令 设置所要执行的FTP命令，
                //ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectoryDetails;//假设此处为显示指定路径下的文件列表
                ftp.Method = System.Net.WebRequestMethods.Ftp.UploadFile;
                //指定文件传输的数据类型
                ftp.UseBinary = true;
                ftp.UsePassive = true;

                //告诉ftp文件大小
                ftp.ContentLength = fileinfo.Length;
                //缓冲大小设置为2KB
                const int BufferSize = 2048;
                byte[] content = new byte[BufferSize - 1 + 1];
                int dataRead;

                //打开一个文件流 (System.IO.FileStream) 去读上传的文件
                using (FileStream fs = fileinfo.OpenRead())
                {
                    try
                    {
                        LogWriter.Write("Ftp上传的文件写入流开始:3", ",文件" + fileinfo, "FileUrl");
                        //把上传的文件写入流
                        using (Stream rs = ftp.GetRequestStream())
                        {
                            do
                            {
                                //每次读文件流的2KB
                                dataRead = fs.Read(content, 0, BufferSize);
                                rs.Write(content, 0, dataRead);
                            } while (!(dataRead < BufferSize));
                            rs.Close();
                        }

                    }
                    catch (Exception ex)
                    {
                        LogWriter.Write("Ftp上传的文件写入流失败:error:", ex.Message, "FileUrl");
                    }
                    finally
                    {
                        fs.Close();
                    }

                }

                ftp = null;
                //设置FTP命令
                ftp = GetRequest(URI, username, password);
                ftp.Method = System.Net.WebRequestMethods.Ftp.Rename; //改名
                ftp.RenameTo = fileinfo.Name;
                try
                {
                    ftp.GetResponse();
                }
                catch (Exception ex)
                {
                    ftp = GetRequest(URI, username, password);
                    ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile; //删除
                    ftp.GetResponse();
                    throw ex;
                }
                finally
                {
                    //fileinfo.Delete();
                }
                LogWriter.Write("Ftp上传文件结束7:" + fileinfo, "URI：" + URI + ",ftp地址" + hostname, "FileUrl");
                // 可以记录一个日志  "上传" + fileinfo.FullName + "上传到" + "FTP://" + hostname + "/" + targetDir + "/" + fileinfo.Name + "成功." );
                ftp = null;
            }
            catch (Exception ex)
            {
                LogWriter.Write("ftp上传日志", "上传" + fileinfo.FullName + "上传到" + "FTP://" + hostname + "/" + targetDir + "/" + fileinfo.Name + "失败，失败原因：" + ex.Message + "，StackTrace" + ex.StackTrace, "FtpClientNew");
            }

            #region
            /*****
             *FtpWebResponse
             * ****/
            //FtpWebResponse ftpWebResponse = (FtpWebResponse)ftp.GetResponse();
            #endregion
        }

        /// <summary>
        /// 下载文件
        /// </summary>
        /// <param name="localDir">下载至本地路径</param>
        /// <param name="FtpDir">ftp目标文件路径</param>
        /// <param name="FtpFile">从ftp要下载的文件名</param>
        /// <param name="hostname">ftp地址即IP</param>
        /// <param name="username">ftp用户名</param>
        /// <param name="password">ftp密码</param>
        public static void DownloadFile(string localDir, string FtpDir, string FtpFile, string hostname, string username, string password)
        {
            string URI = "FTP://" + hostname + "/" + FtpDir + "/" + FtpFile;
            string tmpname = Guid.NewGuid().ToString();
            string localfile = localDir + @"\" + tmpname;

            System.Net.FtpWebRequest ftp = GetRequest(URI, username, password);
            ftp.Method = System.Net.WebRequestMethods.Ftp.DownloadFile;
            ftp.UseBinary = true;
            ftp.UsePassive = false;

            using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                using (Stream responseStream = response.GetResponseStream())
                {
                    //loop to read & write to file
                    using (FileStream fs = new FileStream(localfile, FileMode.CreateNew))
                    {
                        try
                        {
                            byte[] buffer = new byte[2048];
                            int read = 0;
                            do
                            {
                                read = responseStream.Read(buffer, 0, buffer.Length);
                                fs.Write(buffer, 0, read);
                            } while (!(read == 0));
                            responseStream.Close();
                            fs.Flush();
                            fs.Close();
                        }
                        catch (Exception)
                        {
                            //catch error and delete file only partially downloaded
                            fs.Close();
                            //delete target file as it's incomplete
                            File.Delete(localfile);
                            throw;
                        }
                    }

                    responseStream.Close();
                }

                response.Close();
            }



            try
            {
                File.Delete(localDir + @"\" + FtpFile);
                File.Move(localfile, localDir + @"\" + FtpFile);


                ftp = null;
                ftp = GetRequest(URI, username, password);
                ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile;
                ftp.GetResponse();

            }
            catch (Exception ex)
            {
                File.Delete(localfile);
                throw ex;
            }

            // 记录日志 "从" + URI.ToString() + "下载到" + localDir + @"\" + FtpFile + "成功." );
            ftp = null;
        }

        /// <summary>
        /// 搜索远程文件
        /// </summary>
        /// <param name="targetDir"></param>
        /// <param name="hostname"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <param name="SearchPattern"></param>
        /// <returns></returns>
        public static List<string> ListDirectory(string targetDir, string hostname, string username, string password, string SearchPattern)
        {
            List<string> result = new List<string>();
            try
            {
                string URI = "FTP://" + hostname + "/" + targetDir + "/" + SearchPattern;

                System.Net.FtpWebRequest ftp = GetRequest(URI, username, password);
                ftp.Method = System.Net.WebRequestMethods.Ftp.ListDirectory;
                ftp.UsePassive = true;
                ftp.UseBinary = true;


                string str = GetStringResponse(ftp);
                str = str.Replace("\r\n", "\r").TrimEnd('\r');
                str = str.Replace("\n", "\r");
                if (str != string.Empty)
                    result.AddRange(str.Split('\r'));

                return result;
            }
            catch { }
            return null;
        }

        private static string GetStringResponse(FtpWebRequest ftp)
        {
            //Get the result, streaming to a string
            string result = "";
            using (FtpWebResponse response = (FtpWebResponse)ftp.GetResponse())
            {
                long size = response.ContentLength;
                using (Stream datastream = response.GetResponseStream())
                {
                    using (StreamReader sr = new StreamReader(datastream, System.Text.Encoding.Default))
                    {
                        result = sr.ReadToEnd();
                        sr.Close();
                    }

                    datastream.Close();
                }

                response.Close();
            }

            return result;
        }

        /// 在ftp服务器上创建目录
        /// </summary>
        /// <param name="dirName">创建的目录名称</param>
        /// <param name="ftpHostIP">ftp地址</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public static void MakeDir(string dirName, string ftpHostIP, string username, string password)
        {
            try
            {
                LogWriter.Write("ftp服务器上创建目录开始:3 创建的目录名称" + dirName, "，ftp地址：" + ftpHostIP, "FileUrl");
                string[] _dirs = dirName.Split(DIR_SEPARATOR);
                if (_dirs.Length > MAX_DIR_DEPTH)
                    throw new ArgumentException("dirName");
                foreach (string s in _dirs)
                {
                    if (s.Length > MAX_NAME_LENGTH)
                        throw new ArgumentException("dirName");
                }
                int i = 0;
                string uri = "ftp://" + ftpHostIP + "/", _dir = string.Empty;
                System.Net.FtpWebRequest ftp;
                FtpWebResponse response;
                while (i < _dirs.Length)
                {
                    if (i > 0)
                        _dir += "/";
                    _dir += _dirs[i];
                    if (!ftpIsExistsPath(_dir, ftpHostIP, username, password))
                    {
                        ftp = GetRequest(uri + _dir, username, password);
                        ftp.Method = WebRequestMethods.Ftp.MakeDirectory;
                        response = (FtpWebResponse)ftp.GetResponse();
                        response.Close();
                    }
                    i++;
                }
                LogWriter.Write("ftp服务器上创建目录结束:4 创建的目录名称" + dirName, "，uri：" + uri, "FileUrl");
            }
            catch (Exception ex)
            {
                LogWriter.Write("在ftp服务器上创建目录", ex.Message, "FileUrl_EXT");
            }
        }

        /// <summary>
        /// 删除目录
        /// </summary>
        /// <param name="dirName">创建的目录名称</param>
        /// <param name="ftpHostIP">ftp地址</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public void delDir(string dirName, string ftpHostIP, string username, string password)
        {
            try
            {
                string uri = "ftp://" + ftpHostIP + "/" + dirName;
                System.Net.FtpWebRequest ftp = GetRequest(uri, username, password);
                ftp.Method = WebRequestMethods.Ftp.RemoveDirectory;
                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                LogWriter.Write("删除目录", ex.Message, "FileUrl_EXT");
            }
        }
        public static void DeleteFile(string dirName, string ftpHostIP, string username, string password)
        {
            try
            {
                string uri = "ftp://" + ftpHostIP + "/" + dirName;
                System.Net.FtpWebRequest ftp = GetRequest(uri, username, password);
                ftp = GetRequest(uri, username, password);
                ftp.Method = System.Net.WebRequestMethods.Ftp.DeleteFile; //删除
                ftp.GetResponse().Close();
            }
            catch (Exception ex)
            {
                LogWriter.Write("删除文件", ex.Message, "FileUrl_EXT");
            }
        }

        /// <summary>
        /// 文件重命名
        /// </summary>
        /// <param name="currentFilename">当前目录名称</param>
        /// <param name="newFilename">重命名目录名称</param>
        /// <param name="ftpServerIP">ftp地址</param>
        /// <param name="username">用户名</param>
        /// <param name="password">密码</param>
        public void Rename(string currentFilename, string newFilename, string ftpServerIP, string username, string password)
        {
            try
            {

                FileInfo fileInf = new FileInfo(currentFilename);
                string uri = "ftp://" + ftpServerIP + "/" + fileInf.Name;
                System.Net.FtpWebRequest ftp = GetRequest(uri, username, password);
                ftp.Method = WebRequestMethods.Ftp.Rename;

                ftp.RenameTo = newFilename;
                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();

                response.Close();
            }
            catch (Exception ex)
            {
            }
        }

        private static FtpWebRequest GetRequest(string URI, string username, string password)
        {
            LogWriter.Write("服务器信息FtpWebRequest创建类的对象", URI, "FileUrl_EXT");
            //根据服务器信息FtpWebRequest创建类的对象
            FtpWebRequest result = (FtpWebRequest)FtpWebRequest.Create(URI);
            //提供身份验证信息
            result.Credentials = new System.Net.NetworkCredential(username, password);
            //设置请求完成之后是否保持到FTP服务器的控制连接，默认值为true
            result.KeepAlive = false;
            return result;
        }

        /// <summary>
        /// 判断ftp服务器上该目录是否存在
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="ftpHostIP"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        private static bool ftpIsExistsPath(string dirName, string ftpHostIP, string username, string password)
        {
            bool flag = true;
            try
            {
                string uri = "ftp://" + ftpHostIP + "/" + dirName;
                if (!uri.EndsWith("/"))
                {
                    uri = uri + "/";
                }
                System.Net.FtpWebRequest ftp = GetRequest(uri, username, password);
                ftp.Credentials = new NetworkCredential(username, password);
                ftp.Method = WebRequestMethods.Ftp.ListDirectory;
                ftp.UseBinary = true;
                ftp.UsePassive = true;

                FtpWebResponse response = (FtpWebResponse)ftp.GetResponse();
                response.Close();
            }
            catch (Exception ex)
            {
                flag = false;
                LogWriter.Write("判断ftp服务器上该目录是否存在", ex.Message, "FileUrl_EXT");
            }
            return flag;
        }
    }
}
