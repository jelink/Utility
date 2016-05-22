using System;
using System.Linq;
using System.Reflection;
using System.ComponentModel;
using System.Collections.Generic;

namespace Utility
{
    /// <summary>
    /// 获取Enum的描述信息，如果存在DescriptionAttribute,则返回DescriptionAttribute，否则返回Enum.ToString()
    /// </summary>
    public static class EnmuUtility
    {
        /// <summary>
        /// 获取描述信息
        /// </summary>
        /// <param name="en"></param>
        /// <param name="descriptionType">当为null时该值为 typeof(DescriptionAttribute)</param>
        /// <returns></returns>
        public static string GetDescription(this Enum en, Type descriptionType = null)
        {
            if (descriptionType == null)
                descriptionType = typeof(DescriptionAttribute);
            Type type = en.GetType();
            MemberInfo[] memInfo = type.GetMember(en.ToString());
            if (memInfo != null && memInfo.Length > 0)
            {
                object[] attrs = memInfo[0].GetCustomAttributes(descriptionType, false);
                if (attrs != null && attrs.Length > 0)
                {
                    if (descriptionType == typeof(DescriptionAttribute))
                        return ((DescriptionAttribute)attrs[0]).Description;
                    return en.ToString();
                }
            }
            return en.ToString();
        }

        /// <summary>
        /// 获取枚举值和描述的信息
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="descriptionType"></param>
        /// <returns></returns>
        public static List<EnumInfo> GetEnumInfo<T>(Type descriptionType = null)
        {
            if (descriptionType == null)
                descriptionType = typeof(DescriptionAttribute);
            Type enumType = typeof(T);
            List<Enum> list = enumType.GetFields().Where(o => o.IsLiteral).Select(o => (Enum)o.GetValue(enumType)).ToList();
            List<EnumInfo> result = new List<EnumInfo>();
            list.ForEach(o =>
            {
                result.Add(new EnumInfo { IntValue = (int)(object)o, StringValue = o.ToString(), Description = o.GetDescription(descriptionType) });
            });
            return result;
        }

    }

    /// <summary>
    /// 枚举信息
    /// </summary>
    public class EnumInfo
    {
        /// <summary>
        /// int 值
        /// </summary>
        public long IntValue { set; get; }

        /// <summary>
        /// stringValue
        /// </summary>
        public string StringValue { set; get; }


        /// <summary>
        /// 描述
        /// </summary>
        public string Description { set; get; }
    }
}
