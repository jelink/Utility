using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;
using System.Threading;
using System.Text.RegularExpressions;

namespace Utility
{
    public class CommCode
    {
        /// <summary>
        /// 是否含有HTML
        /// </summary>
        /// <param name="content"></param>
        /// <returns>true包含，false不包含</returns>
        public static bool IsHtml(string content)
        {
            return Regex.IsMatch(content, "<(.[^>]*)>");
        }
        /// <summary>
        /// 写入文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="ThisUpValue"></param>
        public void WriteFileByStreamReader(string filePath, string ThisUpValue, bool flag)
        {
            try
            {
                //初始化streamreader
                using (StreamWriter sw = new StreamWriter(filePath, flag))
                {
                    //写入一行
                    sw.Write(ThisUpValue);
                    sw.Close();
                }
            }
            catch (Exception)
            {
                using (FileStream NewText = File.Create(filePath))
                {
                    NewText.Close();
                }

                //初始化streamreader
                using (StreamWriter sw = new StreamWriter(filePath))
                {
                    //写入一行
                    sw.WriteLine(ThisUpValue);
                    sw.Close();
                }
            }
        }

        /// <summary>
        /// 读取文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public string ReadFileByStreamReader(string filePath)
        {
            try
            {
                using (StreamReader sr = new StreamReader(filePath))
                {
                    string ThisUpValue = sr.ReadToEnd();
                    return ThisUpValue;
                }
            }
            catch (Exception ee)
            {
                return "";
            }

        }


        /// <summary>
        /// 比较大小
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        /// <returns></returns>
        public static int CompareDinosByLength(double x, double y, double z, double w, double m, double n)
        {
            if (z == w && m == n)
            {
                return x.CompareTo(y);
            }
            else
            {
                return 0;
            }

        }

        /// <summary>
        /// 得到月份的英文检测
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string getEnglishCode(int t)
        {
            if (t != 0)
            {
                switch (t)
                {
                    case 1:
                        return "JAN";
                    case 2:
                        return "FEB";
                    case 3:
                        return "MAR";
                    case 4:
                        return "APR";
                    case 5:
                        return "MAY";
                    case 6:
                        return "JUN";
                    case 7:
                        return "JUL";
                    case 8:
                        return "AUG";
                    case 9:
                        return "SEP";
                    case 10:
                        return "OCT";
                    case 11:
                        return "NOV";
                    case 12:
                        return "DEC";
                    default:
                        return "JAN";
                }
            }
            else
                return "";
        }


    }

    /// <summary>
    /// 月份
    /// </summary>
    public enum Month
    {
        /// <summary>
        /// 一月
        /// </summary>
        JAN = 1,
        /// <summary>
        /// 二月
        /// </summary>
        FEB = 2,
        /// <summary>
        /// 三月
        /// </summary>
        MAR = 3,
        /// <summary>
        /// 四月
        /// </summary>
        APR = 4,
        /// <summary>
        /// 五月
        /// </summary>
        MAY = 5,
        /// <summary>
        /// 六月
        /// </summary>
        JUN = 6,
        /// <summary>
        /// 七月
        /// </summary>
        JUL = 7,
        /// <summary>
        /// 八月
        /// </summary>
        AUG = 8,
        /// <summary>
        /// 九月
        /// </summary>
        SEP = 9,
        /// <summary>
        /// 十月
        /// </summary>
        OCT = 10,
        /// <summary>
        /// 十一月
        /// </summary>
        NOV = 11,
        /// <summary>
        /// 十二月
        /// </summary>
        DEC = 12
    }

    //public class RequestState
    //{
    //    // This class stores the state of the request.
    //    const int BUFFER_SIZE = 1024;
    //    public StringBuilder requestData;
    //    public byte[] bufferRead;
    //    public WebRequest request;
    //    public WebResponse response;
    //    public Stream responseStream;
    //    public RequestState()
    //    {
    //        bufferRead = new byte[BUFFER_SIZE];
    //        requestData = new StringBuilder("");
    //        request = null;
    //        responseStream = null;
    //    }
    //}
    //public class WebRequest_BeginGetResponse
    //{
    //    public static ManualResetEvent allDone = new ManualResetEvent(false);
    //    const int BUFFER_SIZE = 1024;
    //    public static void Main(string Url)
    //    {
    //        try
    //        {
    //            WebRequest myWebRequest=null;
    //            // Create a new webrequest to the mentioned URL.
    //            if (Url=="")
    //            {
    //                myWebRequest = WebRequest.Create("http://air.tenpay.com/pages/international/tejia.php");
    //            }
    //            else
    //            {
    //                myWebRequest = WebRequest.Create(Url);
    //            }
    //            myWebRequest.Method = "Get";
    //            myWebRequest.ContentType = "application/x-www-form-urlencoded;charset=utf-8";
    //            // Please, set the proxy to a correct value.
    //            WebProxy proxy = new WebProxy();

    //            proxy.Credentials = new NetworkCredential();
    //            myWebRequest.Proxy = proxy;
    //            // Create a new instance of the RequestState.
    //            RequestState myRequestState = new RequestState();
    //            // The 'WebRequest' object is associated to the 'RequestState' object.
    //            myRequestState.request = myWebRequest;
    //            // Start the Asynchronous call for response.
    //            IAsyncResult asyncResult = (IAsyncResult)myWebRequest.BeginGetResponse(new AsyncCallback(RespCallback), myRequestState);
    //            allDone.WaitOne();
    //            // Release the WebResponse resource.
    //            myRequestState.response.Close();
    //            Console.Read();
    //        }
    //        catch (WebException e)
    //        {
    //            Console.WriteLine("WebException raised!");
    //            Console.WriteLine("\n{0}", e.Message);
    //            Console.WriteLine("\n{0}", e.Status);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Exception raised!");
    //            Console.WriteLine("Source : " + e.Source);
    //            Console.WriteLine("Message : " + e.Message);
    //        }
    //    }
    //    private static void RespCallback(IAsyncResult asynchronousResult)
    //    {
    //        try
    //        {
    //            // Set the State of request to asynchronous.
    //            RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
    //            WebRequest myWebRequest1 = myRequestState.request;
    //            // End the Asynchronous response.
    //            myRequestState.response = myWebRequest1.EndGetResponse(asynchronousResult);
    //            // Read the response into a 'Stream' object.
    //            Stream responseStream = myRequestState.response.GetResponseStream();
    //            myRequestState.responseStream = responseStream;
    //            // Begin the reading of the contents of the HTML page and print it to the console.
    //            IAsyncResult asynchronousResultRead = responseStream.BeginRead(myRequestState.bufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);

    //        }
    //        catch (WebException e)
    //        {
    //            Console.WriteLine("WebException raised!");
    //            Console.WriteLine("\n{0}", e.Message);
    //            Console.WriteLine("\n{0}", e.Status);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Exception raised!");
    //            Console.WriteLine("Source : " + e.Source);
    //            Console.WriteLine("Message : " + e.Message);
    //        }
    //    }
    //    private static void ReadCallBack(IAsyncResult asyncResult)
    //    {
    //        try
    //        {
    //            // Result state is set to AsyncState.
    //            RequestState myRequestState = (RequestState)asyncResult.AsyncState;
    //            Stream responseStream = myRequestState.responseStream;
    //            int read = responseStream.EndRead(asyncResult);
    //            // Read the contents of the HTML page and then print to the console.
    //            if (read > 0)
    //            {
    //                myRequestState.requestData.Append(Encoding.ASCII.GetString(myRequestState.bufferRead, 0, read));
    //                IAsyncResult asynchronousResult = responseStream.BeginRead(myRequestState.bufferRead, 0, BUFFER_SIZE, new AsyncCallback(ReadCallBack), myRequestState);
    //            }
    //            else
    //            {
    //                Console.WriteLine("\nThe HTML page Contents are:  ");
    //                if (myRequestState.requestData.Length > 1)
    //                {
    //                    string sringContent;
    //                    sringContent = myRequestState.requestData.ToString();
    //                    Console.WriteLine(sringContent);
    //                }
    //                Console.WriteLine("\nPress 'Enter' key to continue........");
    //                responseStream.Close();
    //                allDone.Set();
    //            }
    //        }
    //        catch (WebException e)
    //        {
    //            Console.WriteLine("WebException raised!");
    //            Console.WriteLine("\n{0}", e.Message);
    //            Console.WriteLine("\n{0}", e.Status);
    //        }
    //        catch (Exception e)
    //        {
    //            Console.WriteLine("Exception raised!");
    //            Console.WriteLine("Source : {0}", e.Source);
    //            Console.WriteLine("Message : {0}", e.Message);
    //        }

    //    }


    //}

    public class RequestState
    {
        // This class stores the request state of the request.
        public WebRequest request;
        public RequestState()
        {
            request = null;
        }
    }
    /// <summary>
    /// 抓取页面
    /// </summary>
    public class WebRequest_BeginGetRequeststream
    {
        public static ManualResetEvent allDone = new ManualResetEvent(false);
        public static void Main(string Url)
        {
            WebRequest myWebRequest = null;
            // Create a new webrequest to the mentioned URL.
            if (Url == "")
            {
                myWebRequest = WebRequest.Create("http://air.tenpay.com/pages/international/tejia.php");
            }
            else
            {
                myWebRequest = WebRequest.Create(Url);
            }

            // Create an instance of the RequestState and assign
            // 'myWebRequest' to it's request field.
            RequestState myRequestState = new RequestState();
            myRequestState.request = myWebRequest;
            myWebRequest.ContentType = "application/x-www-form-urlencoded";

            // Set the 'Method' property  to 'POST' to post data to a Uri.
            myRequestState.request.Method = "POST";
            // Start the Asynchronous 'BeginGetRequestStream' method call.
            IAsyncResult r = (IAsyncResult)myWebRequest.BeginGetRequestStream(
                new AsyncCallback(ReadCallback), myRequestState);
            // Pause the current thread until the async operation completes.
            Console.WriteLine("main thread waiting...");
            allDone.WaitOne();
            // Assign the response object of 'WebRequest' to a 'WebResponse' variable.
            WebResponse myWebResponse = myWebRequest.GetResponse();
            Console.WriteLine("The string has been posted.");
            Console.WriteLine("Please wait for the response...");

            Stream streamResponse = myWebResponse.GetResponseStream();
            StreamReader streamRead = new StreamReader(streamResponse);
            Char[] readBuff = new Char[256];
            int count = streamRead.Read(readBuff, 0, 256);
            Console.WriteLine("\nThe contents of the HTML page are ");

            while (count > 0)
            {
                String outputData = new String(readBuff, 0, count);
                Console.Write(outputData);
                count = streamRead.Read(readBuff, 0, 256);
            }

            // Close the Stream Object.
            streamResponse.Close();
            streamRead.Close();


            // Release the HttpWebResponse Resource.
            myWebResponse.Close();
        }

        /// <summary>
        /// 回调方法
        /// </summary>
        /// <param name="asynchronousResult"></param>
        private static void ReadCallback(IAsyncResult asynchronousResult)
        {
            RequestState myRequestState = (RequestState)asynchronousResult.AsyncState;
            WebRequest myWebRequest = myRequestState.request;

            // End the Asynchronus request.
            Stream streamResponse = myWebRequest.EndGetRequestStream(asynchronousResult);

            // Create a string that is to be posted to the uri.
            Console.WriteLine("Please enter a string to be posted:");
            string postData = Console.ReadLine();
            // Convert the string into a byte array.
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);

            // Write the data to the stream.
            streamResponse.Write(byteArray, 0, postData.Length);
            streamResponse.Close();
            allDone.Set();
        }
    }
}
