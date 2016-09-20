using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace JsonParserOverride.Extensions
{
    public static class ObjectMapperExtensions
    {
        public static bool TryMapPremitiveObject(object srcValue, Type destType, out object destValue, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool isSuccess;

            if (destType == typeof(string))
            {
                isSuccess = TryMapString(srcValue, out destValue);
                if (!isSuccess) errorMessage = "is not valid string";
            }
            else if (destType == typeof(bool))
            {
                isSuccess = TryMapBoolean(srcValue, out destValue);
                if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
            }
            else if (destType == typeof(bool?))
            {
                if (srcValue != null)
                {
                    isSuccess = TryMapBoolean(srcValue, out destValue);
                    if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
                }
                else
                {
                    isSuccess = true;
                    destValue = null;
                }
            }
            else if (destType == typeof(int))
            {
                isSuccess = TryMapInteger(srcValue, out destValue);
                if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
            }
            else if (destType == typeof(int?))
            {
                if (srcValue != null)
                {
                    isSuccess = TryMapInteger(srcValue, out destValue);
                    if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
                }
                else
                {
                    isSuccess = true;
                    destValue = null;
                }

            }
            else if (destType == typeof(double))
            {
                isSuccess = TryMapDouble(srcValue, out destValue);
                if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
            }
            else if (destType == typeof(double?))
            {
                if (srcValue != null)
                {
                    isSuccess = TryMapDouble(srcValue, out destValue);
                    if (!isSuccess) errorMessage = $"{srcValue} is not valid format";
                }
                else
                {
                    isSuccess = true;
                    destValue = null;
                }
            }
            else if (destType == typeof(decimal))
            {
                object doubleValue;
                isSuccess = TryMapDouble(srcValue, out doubleValue);
                if (isSuccess)
                {
                    destValue = Convert.ToDecimal(doubleValue);
                }
                else
                {
                    destValue = 0;
                    errorMessage = $"{srcValue} is not valid format";
                }
            }
            else if (destType == typeof(decimal?))
            {
                if (srcValue != null)
                {
                    object doubleValue;
                    isSuccess = TryMapDouble(srcValue, out doubleValue);
                    if (isSuccess)
                    {
                        destValue = Convert.ToDecimal(doubleValue);
                    }
                    else
                    {
                        destValue = null;
                        errorMessage = $"{srcValue} is not valid format";
                    }
                }
                else
                {
                    isSuccess = true;
                    destValue = null;
                }
            }
            else if (destType.GetTypeInfo().IsEnum || (Nullable.GetUnderlyingType(destType)?.GetTypeInfo().IsEnum == true))
            {
                destValue = Activator.CreateInstance(destType);

                if (srcValue == null)
                {
                    isSuccess = !destType.GetTypeInfo().IsEnum;
                }
                else
                {
                    isSuccess = TryMapEnum(srcValue, destType, out destValue);
                }
                if (!isSuccess) errorMessage = $"{srcValue} is not defined";
            }
            else if (destType == typeof(DateTime))
            {
                isSuccess = TryMapDateTime(srcValue, out destValue);
                if (!isSuccess) errorMessage = $"{srcValue} is not valid format. Pleaser refer ISO 8601";
            }
            else if (destType == typeof(DateTime?))
            {
                if (srcValue != null)
                {
                    isSuccess = TryMapDateTime(srcValue, out destValue);
                    if (!isSuccess) errorMessage = $"{srcValue} is not valid format. Pleaser refer ISO 8601";
                }
                else
                {
                    isSuccess = true;
                    destValue = null;
                }
            }
            else
            {
                destValue = null;
                isSuccess = false;
                errorMessage = $"{srcValue} for data type {destType} is not implemented";
            }

            return isSuccess;
        }

        public static bool TryMapString(object src, out object dst)
        {
            dst = src == null ? null : Convert.ToString(src).Trim();
            return true;
        }

        public static bool TryMapInteger(object src, out object dst)
        {
            dst = 0;
            if (src == null)
            {
                return false;
            }

            var type = src.GetType();

            if (type == typeof(long) || type == typeof(double))
            {
                dst = Convert.ToInt32(src);
                return true;
            }

            if (type != typeof(string)) return false;

            var strValue = (string)src;
            strValue = strValue.Trim();
            int iVal;
            var result = int.TryParse(strValue, out iVal);
            if (result) dst = iVal;
            return result;
        }

        public static bool TryMapBoolean(object src, out object dst)
        {
            dst = 0;
            if (src == null)
            {
                return false;
            }

            var type = src.GetType();

            if (type == typeof(long) || type == typeof(double) || type == typeof(bool))
            {
                var intValue = Convert.ToInt32(src);
                if (intValue < 0 || intValue > 1) return false;
                dst = intValue == 1;
                return true;
            }

            if (type != typeof(string)) return false;

            var strValue = ((string)src).Trim();

            if (strValue.Equals("true", StringComparison.OrdinalIgnoreCase))
            {
                dst = true;
                return true;
            }

            if (strValue.Equals("false", StringComparison.OrdinalIgnoreCase))
            {
                dst = false;
                return true;
            }

            return false;
        }

        public static bool TryMapEnum(object src, Type enumType, out object dst)
        {
            var isNullable = false;

            if (Nullable.GetUnderlyingType(enumType)?.GetTypeInfo().IsEnum == true)
            {
                isNullable = true;
                dst = null;
                enumType = enumType.GetGenericArguments()[0];
            }
            else
            {
                dst = Enum.ToObject(enumType, 0);
            }

            if (!enumType.GetTypeInfo().IsEnum)
            {
                return false;
            }

            if (src == null)
            {
                return isNullable;
            }

            if (!new[] { typeof(string), typeof(int), typeof(long) }.Contains(src.GetType()))
            {
                return false;
            }

            var s = src as string;
            if (s != null)
            {
                if (enumType.GetTypeInfo().IsEnumDefined(s))
                {
                    dst = Enum.Parse(enumType, s);
                    return true;
                }

                int intValue;

                if (!int.TryParse(s, out intValue)) return false;

                src = intValue;
            }

            if (src is long || src is int)
            {
                dst = Convert.ToInt32(src);
            }

            if (!(dst is int) || !enumType.GetTypeInfo().IsEnumDefined(int.Parse(dst.ToString()))) return false;

            dst = Enum.ToObject(enumType, src);

            return true;
        }

        public static bool TryMapDouble(object src, out object dst)
        {
            dst = 0;
            if (src == null)
            {
                return false;
            }

            var type = src.GetType();

            if (type == typeof(long) || type == typeof(double))
            {
                dst = Convert.ToDouble(src);
                return true;
            }

            if (type != typeof(string)) return false;

            var strValue = (string)src;
            strValue = strValue.Trim();
            double iVal;
            var result = double.TryParse(strValue, out iVal);
            if (result) dst = iVal;
            return result;
        }

        public static bool TryMapDateTime(object src, out object dst)
        {
            dst = new DateTime();

            if (src == null) return false;

            var success = true;

            if (src is DateTime)
            {
                dst = src;
            }
            else if (src is string)
            {
                var strDate = ((string)src).Trim().ToLower();
                switch (strDate)
                {
                    case "now":
                        dst = DateTime.Now;
                        break;
                    case "today":
                        dst = DateTime.Today;
                        break;
                    case "yesterday":
                        dst = DateTime.Today.AddDays(-1);
                        break;
                    case "tomorrow":
                        dst = DateTime.Today.AddDays(1);
                        break;
                    default:
                        success = false;
                        break;
                }
            }
            else
            {
                success = false;
            }

            return success;
        }

        public static bool IsList(object o)
        {
            if (o == null) return false;
            return o is IList &&
                   o.GetType().GetTypeInfo().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>));
        }

        public static bool IsDictionary(object o)
        {
            if (o == null) return false;
            return o is IDictionary &&
                   o.GetType().GetTypeInfo().IsGenericType &&
                   o.GetType().GetGenericTypeDefinition().IsAssignableFrom(typeof(Dictionary<,>));
        }
    }
}