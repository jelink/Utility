using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utility
{
    public static class IEnumerableTools
    {
        /// <summary>
        /// 将数组通过间隔符组合成一个完整的字符串
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="list"></param>
        /// <param name="separator"></param>
        /// <param name="fun"></param>
        /// <returns></returns>
        public static string ToString<T>(this IEnumerable<T> list, string separator, Func<T, string> fun)
        {
            if (separator == null)
            {
                throw new ArgumentNullException();
            }
            StringBuilder tmp = new StringBuilder();
            if (list != null)
            {
                foreach (var l in list)
                {
                    tmp.Append(separator);
                    tmp.Append(fun == null ? l.ToString() : fun(l));
                }
            }
            if (tmp.Length > 0)
            {
                tmp = tmp.Remove(0, separator.Length);
            }
            return tmp.ToString();
        }
    }
}
