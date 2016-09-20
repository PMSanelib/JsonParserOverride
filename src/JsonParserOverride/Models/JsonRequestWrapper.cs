using System;
using System.Collections;
using System.Collections.Generic;
using System.Dynamic;
using System.Reflection;
using JsonParserOverride.Extensions;
using Newtonsoft.Json.Linq;

namespace JsonParserOverride.Models
{
    public class JsonRequestWrapper<T> : DynamicObject, IJsonRequestWrapper where T : DtoBase, new()
    {
        public T MappedObj { get; } = new T();

        public ValidationResult ValidationResult { get; } = new ValidationResult();

        private readonly Dictionary<string, PropertyInfo> _propertyList = new Dictionary<string, PropertyInfo>();

        public JsonRequestWrapper()
        {
            ExtractProperties();
        }

        private void ExtractProperties()
        {
            var objProperties = MappedObj.GetType().GetProperties();

            foreach (var info in objProperties)
            {
                _propertyList.Add(info.Name.ToLower(), info);
            }
        }

        public override bool TryGetMember(GetMemberBinder binder, out object value)
        {
            throw new NotImplementedException();
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {
            var propertyName = binder.Name.ToLower();

            if (_propertyList.ContainsKey(propertyName))
            {
                var info = _propertyList[propertyName];

                object mappedValue;

                var isSuccess = TryMapDataType(binder.Name, value, info.PropertyType, out mappedValue, ValidationResult);

                if (!isSuccess) mappedValue = Activator.CreateInstance(info.PropertyType);

                info.SetValue(MappedObj, mappedValue);
            }
            else
            {
                ValidationResult.AddError(binder.Name, "Unexpected field");
            }

            return true;
        }

        private static bool TryMapDataType(string propertyName, object srcValue, Type destType, out object destValue, ValidationResult validationResult)
        {
            destValue = null;
            var isSuccess = true;

            if (PrimitiveTypes.Test(destType))
            {
                string errorMessage;
                isSuccess = ObjectMapperExtensions.TryMapPremitiveObject(srcValue, destType, out destValue, out errorMessage);
                if (!isSuccess) validationResult.AddError(propertyName, errorMessage);
            }
            else if (destType.GetTypeInfo().IsGenericType && destType.GetGenericTypeDefinition().IsAssignableFrom(typeof(List<>)))
            {
                var listElementType = destType.GetTypeInfo().GenericTypeArguments[0];
                var listType = typeof(List<>).MakeGenericType(listElementType);
                var list = (IList)Activator.CreateInstance(listType);
                var isPremitiveElement = PrimitiveTypes.Test(listElementType);
                if (srcValue.GetType() != typeof(JArray))
                {
                    validationResult.AddError(propertyName, $"{srcValue} is not valid array");
                }
                else
                {
                    destValue = list;
                    var index = 0;
                    foreach (var token in (JArray) srcValue)
                    {
                        var propertyIndexName = $"{propertyName}[{index}]";
                        var destElm = Activator.CreateInstance(listElementType);
                        if (isPremitiveElement)
                        {
                            if (token.GetType() == typeof(JValue))
                            {
                                string errorMessage;
                                isSuccess = ObjectMapperExtensions.TryMapPremitiveObject(((JValue)token).Value, listElementType, out destElm, out errorMessage);
                                if (!isSuccess) validationResult.AddError(propertyIndexName, errorMessage);
                            }
                            else
                            {
                                validationResult.AddError(propertyIndexName, "is not valid format");
                            }
                            
                        }
                        else
                        {
                            if (token.GetType() == typeof(JObject))
                            {
                                isSuccess = TryMapJObject(propertyIndexName, token, listElementType, out destElm, validationResult);
                            }
                            else
                            {
                                validationResult.AddError(propertyIndexName, "is not valid format");
                            }
                        }

                        list.Add(destElm);

                        index++;
                    }
                }
            }
            else
            {
                isSuccess = TryMapJObject(propertyName, srcValue, destType, out destValue, validationResult);
            }

            return isSuccess;
        }

        private static bool TryMapJObject(string propertyName, object srcValue, Type destType, out object destValue, ValidationResult validationResult)
        {
            destValue = null;
            var isSuccess = true;

            if (srcValue == null)
            {
                return true;
            }

            if (srcValue.GetType() != typeof(JObject))
            {
                validationResult.AddError(propertyName, $"{srcValue} is not valid format");
                isSuccess = false;
            }
            else
            {
                var subDtoType = typeof(JsonRequestWrapper<>).MakeGenericType(destType);
                dynamic v = ((JObject) srcValue).ToObject(subDtoType);
                if (v.ValidationResult.IsValid)
                {
                    destValue = v.MappedObj;
                    isSuccess = false;
                }
                else
                {
                    validationResult.MergeValidations(v.ValidationResult, propertyName);
                }
            }

            return isSuccess;
        }
    }
}