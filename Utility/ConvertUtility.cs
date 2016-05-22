/*--------------------------------------------------
 * 作用：对数据进行验证，对数据进行转换。
 *       转换之前都先做一下数据验证，
 *       做到前端方法不用管数据是否符合类型，
 *       也不用管是什么类型，只要使用本类的方法，一律可以转换为想要的数据。
 -------------------------------------------------*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.ComponentModel;
using System.Reflection;

namespace Utility
{
    /// <summary>
    /// 数据验证或转换
    /// </summary>
    public static class DataValidateOrConvert
    {
        #region 常量
        /// <summary>
        /// 密码
        /// </summary>
        private const string pwd = "^[0-9a-zA-Z_]{4,25}$";
        #endregion

        #region 验证
        /// <summary>
        /// 指示指定的字符串是 null 还是 System.String.Empty 字符串。
        /// </summary>
        /// <param name="str">要测试的字符串。</param>
        /// <returns>如果 str 为 null 或空字符串 ("")，则为 true；否则为 false。</returns>
        public static bool IsNullOrEmpty(this string str)
        {
            return string.IsNullOrEmpty(str) || str.Trim().Length == 0;
        }
        /// <summary>
        /// 指示指定的对象是 null 还是 DBNull。
        /// </summary>
        /// <param name="obj">要测试的对象</param>
        /// <returns>如果 obj 为 null 或DBNull，则为 true；否则为 false。</returns>
        public static bool IsNullOrDBNull(this object obj)
        {
            return obj == null || obj == DBNull.Value;
        }
        /// <summary>
        /// 指示指定的字符串是否是合法的密码
        /// </summary>
        /// <param name="str">要测试的字符串。</param>
        /// <returns>如果 str 合法，则为 true；否则为 false。</returns>
        public static bool IsLegalPWD(this string str)
        {
            return Regex.IsMatch(str, pwd);
        }
        /// <summary>
        /// 指示指定的对象是否为decimal类型。(obj为空返回false)
        /// </summary>
        /// <param name="obj">要测试的对象</param>
        /// <returns>如果 obj 为decimal，返回true；否则为false。</returns>
        public static bool IsDecimal(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return false;
            }

            return obj is decimal;
        }
        /// <summary>
        /// 指示指定的对象是否为int类型。(obj为空返回false)
        /// </summary>
        /// <param name="obj">要测试的对象</param>
        /// <returns>如果 obj 为int，返回true；否则为false。</returns>
        public static bool IsInt(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return false;
            }

            return obj is int;
        }
        /// <summary>
        /// 指示指定的字符串是否为DateTime类型。(str为空返回false)
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static bool IsDateTime(this string str)
        {
            if (str.IsNullOrDBNull())
            {
                return false;
            }
            try
            {
                DateTime dt = DateTime.Parse(str);
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static bool IsNumber(decimal s, int precision, int scale)
        {
            return IsNumber(s.ToString(), precision, scale);
        }
        /// <summary>
        /// 判断一个字符串是否为合法数字(指定整数位数和小数位数)
        /// </summary>
        /// <param name="s">字符串</param>
        /// <param name="precision">整数位数</param>
        /// <param name="scale">小数位数</param>
        /// <returns></returns>
        public static bool IsNumber(string s, int precision, int scale)
        {
            if ((precision == 0) && (scale == 0))
            {
                return false;
            }
            string pattern = @"(^\d{1," + precision + "}";
            if (scale > 0)
            {
                pattern += @"\.\d{0," + scale + "}$)|" + pattern;
            }
            pattern += "$)";
            return Regex.IsMatch(s, pattern);
        }
        #endregion

        #region 转换
        /// <summary>
        /// Object对象转换为String字符串
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns>转换后的字符串</returns>
        public static string ObjectToString(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return string.Empty;
            }
            return obj.ToString().Trim();
        }
        /// <summary>
        /// 验证Object对象是否是合理的日期，并转换为格式化的String字符串。
        /// 若非合理的日期返回空字符串。
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <param name="format">字符串格式</param>
        /// <returns>转换后的字符串</returns>
        public static string DateTimeValidate(this object obj, string format)
        {
            string str = obj.ObjectToString();

            try
            {
                DateTime dt = DateTime.Parse(str);
                return dt.ToString(format);
            }
            catch
            {
                return string.Empty;
            }
        }
        /// <summary>
        /// Object对象转换为日期类型字符串
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <param name="format">格式字符串</param>
        /// <returns>转换后的日期字符串</returns>
        public static string ObjectToDateTimeString(this object obj, string format)
        {
            return obj.DateTimeValidate(format);
        }
        /// <summary>
        /// Object对象转换为日期类型
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <param name="format">格式字符串</param>
        /// <returns>转换后的日期</returns>
        public static DateTime ObjectToDateTime(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return DateTime.MinValue;
            }

            try
            {
                DateTime dt = Convert.ToDateTime(obj);
                return dt;
            }
            catch
            {
                return DateTime.MinValue;
            }
        }
        /// <summary>
        /// Object对象转换为decimal类型
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns>转换后的数据</returns>
        public static decimal ObjectToDecimal(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return 0;
            }

            decimal value;
            if (decimal.TryParse(obj.ObjectToString(), out value))
            {//转换成功
                return value;
            }
            return 0;
        }
        /// <summary>
        /// Object对象转换为int类型
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns>转换后的数据</returns>
        public static int ObjectToInt(this object obj)
        {
            if (obj.IsNullOrDBNull())
            {
                return 0;
            }

            int value;
            if (int.TryParse(obj.ObjectToString(), out value))
            {//转换成功
                return value;
            }
            return 0;
        }
        /// <summary>
        /// Object对象转换为布尔类型
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns>转换后的数据</returns>
        public static byte ObjectToByte(this object obj)
        {
            byte value;
            if (byte.TryParse(obj.ObjectToString(), out value))
            {//转换成功
                return value;
            }
            return 0;
        }
        /// <summary>
        /// Object对象转换为布尔类型
        /// </summary>
        /// <param name="obj">Object对象</param>
        /// <returns>转换后的数据</returns>
        public static bool ObjectToBoolean(this object obj)
        {
            bool value;
            if (bool.TryParse(obj.ObjectToString(), out value))
            {//转换成功
                return value;
            }
            return false;
        }

        /// <summary>
        /// int转布尔类型（小于等于0的都是false，大于0的都是true）
        /// </summary>
        /// <param name="val">int数据</param>
        /// <returns>转换后的数据</returns>
        public static bool IntToBoolean(this int val)
        {
            if (val < 0)
            {
                return false;
            }
            return Convert.ToBoolean(val);
        }
        /// <summary>
        /// 布尔转int类型
        /// </summary>
        /// <param name="bl">布尔值</param>
        /// <returns>转换后的数据</returns>
        public static int BooleanToInt(this bool bl)
        {
            return Convert.ToInt32(bl);
        }

        #region 将字符串翻译成字节数组
        /// <summary>
        /// 将字符串翻译成字节数组
        /// </summary>
        /// <param name="src">字符串源串</param>
        /// <returns>字节数组</returns>
        public static byte[] GetByteArray(this string src)
        {
            byte[] byteArray = new byte[src.Length];

            for (int i = 0; i < src.Length; i++)
            {
                byteArray[i] = Convert.ToByte(src[i]);
            }

            return byteArray;
        }
        #endregion

        #endregion

        #region 枚举
        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string ToDescription(this Enum obj)
        {
            return ToDescription(obj, false);
        }

        /// <summary>
        /// 获取枚举变量值的 Description 属性
        /// </summary>
        /// <param name="obj">枚举变量</param>
        /// <param name="isTop">是否改变为返回该类、枚举类型的头 Description 属性，而不是当前的属性或枚举变量值的 Description 属性</param>
        /// <returns>如果包含 Description 属性，则返回 Description 属性的值，否则返回枚举变量值的名称</returns>
        public static string ToDescription(this Enum obj, bool isTop)
        {
            if (obj == null)
            {
                return string.Empty;
            }
            try
            {
                Type _enumType = obj.GetType();//获取对象的枚举类型
                if (!_enumType.IsEnum)
                {
                    return string.Empty;
                }
                DescriptionAttribute dna = null;
                if (isTop)
                {//获取枚举类型头Description
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(_enumType, typeof(DescriptionAttribute));
                }
                else
                {
                    FieldInfo fi = _enumType.GetField(Enum.GetName(_enumType, obj));//获取枚举字段
                    dna = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));//利用反射获取字段的描述属性
                }
                if (dna != null && string.IsNullOrEmpty(dna.Description) == false)
                {
                    return dna.Description;
                }
            }
            catch
            {
            }
            return obj.ToString();
        }
        /// <summary>
        /// 根据enum的name获取description
        /// </summary>
        /// <param name="enumItemName">enum名字</param>
        /// <returns></returns>
        public static string NameToDescription<T>(this string enumItemName)
        {
            Type _enumType = typeof(T);//获取对象的枚举类型
            try
            {
                if (!_enumType.IsEnum)
                {
                    return string.Empty;
                }
                FieldInfo fi = _enumType.GetField(enumItemName.ToString());//获取枚举字段
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);//获取字段的所有描述属性

                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }
            catch
            {
            }
            return enumItemName.ToString();
        }
        /// <summary>
        /// 根据enum的value获取description
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItemValue">enum值</param>
        /// <returns></returns>
        public static string ValueToDescription<T>(this int enumItemValue)
        {
            Type _enumType = typeof(T);//获取对象的枚举类型
            try
            {
                if (!_enumType.IsEnum)
                {
                    return string.Empty;
                }
                string enumName = Enum.GetName(_enumType, enumItemValue);
                FieldInfo fi = _enumType.GetField(enumName);//获取枚举字段
                DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);//获取字段的所有描述属性

                if (attributes != null && attributes.Length > 0)
                {
                    return attributes[0].Description;
                }
            }
            catch
            {
            }
            return enumItemValue.ToString();
        }
        /// <summary>
        /// 根据enum的value获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="enumItemValue">enum值</param>
        /// <returns></returns>
        public static T ValueToEnum<T>(this int enumItemValue)
        {
            Type _enumType = typeof(T);//获取对象的枚举类型
            try
            {
                if (!_enumType.IsEnum)
                {
                    return default(T);
                }
                return (T)Enum.Parse(_enumType, enumItemValue.ToString(), true);
            }
            catch
            {
            }
            return default(T);
        }
        /// <summary>
        /// 根据Description获取枚举对象
        /// </summary>
        /// <param name="desc">Description</param>
        /// <returns>枚举对象</returns>
        public static T DescriptionToEnum<T>(this string desc)
        {
            if (desc.IsNullOrEmpty())
            {
                return default(T);
            }
            Type _enumType = typeof(T);//获取对象的枚举类型
            try
            {
                if (!_enumType.IsEnum)
                {
                    return default(T);
                }
                string[] strs = Enum.GetNames(_enumType);
                foreach (string str in strs)
                {//遍历枚举类型所有值进行匹配
                    FieldInfo fi = _enumType.GetField(str);//获取枚举字段
                    DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);//获取字段的所有描述属性

                    if (attributes != null && attributes.Length > 0 && attributes[0].Description == desc)
                    {
                        return (T)Enum.Parse(_enumType, str, true);
                    }
                }
            }
            catch
            { }
            return default(T);
        }
        /// <summary>
        /// 根据Name获取枚举对象
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="name">Name</param>
        /// <returns>枚举对象</returns>
        public static T NameToEnum<T>(this string name)
        {
            if (name.IsNullOrEmpty())
            {
                return default(T);
            }
            Type _enumType = typeof(T);//获取对象的枚举类型
            try
            {
                if (!_enumType.IsEnum)
                {
                    return default(T);
                }
                return (T)Enum.Parse(_enumType, name, true);
            }
            catch
            { }
            return default(T);
        }
        #endregion
    }
}
