using System;
using System.ComponentModel;
using System.Reflection;

namespace ArchiveFiles
{
    /// <summary>
    /// Extension pour retourner la description d'une valeur en fonction de la culture
    /// </summary>
    public static class EnumExtension
    {

        public static string GetDescription(this Enum enumValue, string defDesc)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes(typeof(DescriptionAttribute), true);
                if (attrs != null && attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }

        public static string GetDescription(this Enum enumValue)
        {
          return GetCustomDescription<DescriptionAttribute>(enumValue);
        }

        public static string GetCustomDescription<T>(this Enum enumValue)
        {
            FieldInfo fi = enumValue.GetType().GetField(enumValue.ToString());

            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(T), false);

            if (attributes != null &&
                attributes.Length > 0)
                return attributes[0].Description;
            else
                return enumValue.ToString();
        }

    }
}
