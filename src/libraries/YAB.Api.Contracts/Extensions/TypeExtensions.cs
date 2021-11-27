using System;
using System.Collections.Generic;
using System.Linq;

using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;
using YAB.Core.Annotations;

namespace YAB.Api.Contracts.Extensions
{
    public static class TypeExtensions
    {
        public static object FromStringifiedValue(this Type type, string value)
        {
            if (type == typeof(int))
            {
                return int.Parse(value);
            }
            else if (type == typeof(double))
            {
                return double.Parse(value);
            }
            else if (type == typeof(float))
            {
                return float.Parse(value);
            }
            else if (type == typeof(char))
            {
                return char.Parse(value);
            }
            else if (type == typeof(string))
            {
                return value;
            }
            else
            {
                throw new NotImplementedException($"Could not translate value {value} to {type.FullName}.");
            }
        }

        public static List<PropertyDescriptionDto> GetPropertyDescriptorsForType(this Type configurationType, object? instance = null)
        {
            var propertyDescriptions = new List<PropertyDescriptionDto>();
            var optionProperties = configurationType.GetProperties();

            dynamic instanceCasted = null;
            if (instance != null)
            {
                instanceCasted = Convert.ChangeType(instance, configurationType);
            }

            foreach (var optionProperty in optionProperties)
            {
                var propertyDescriptionAttribute = optionProperty
                    .GetCustomAttributes(typeof(PropertyDescriptionAttribute), true)
                    .Cast<PropertyDescriptionAttribute>()
                    .FirstOrDefault();

                PropertyValueTypeDto? propertyValueType = null;
                try
                {
                    propertyValueType=optionProperty.PropertyType.GetValueType();
                }
                catch (Exception)
                {
                    propertyValueType=null;
                }


                PropertyDescriptionDto descriptor = new PropertyDescriptionDto
                {
                    PropertyName = optionProperty.Name,
                    ValueType = propertyValueType ?? PropertyValueTypeDto.Complex,
                    PropertyDescription = propertyDescriptionAttribute?.Description,
                    IsSecret = propertyDescriptionAttribute?.IsSecret ?? true
                };

                if (instance is not null)
                {
                    descriptor.CurrentValue = optionProperty.GetValue(instanceCasted).ToString();
                }

                propertyDescriptions.Add(descriptor);
            }

            return propertyDescriptions;
        }

        public static PropertyValueTypeDto GetValueType(this Type type)
        {
            if (type == typeof(int) || type == typeof(int?))
            {
                return PropertyValueTypeDto.Int;
            }
            else if (type == typeof(double) || type == typeof(float))
            {
                return PropertyValueTypeDto.FloatingPoint;
            }
            else if (type == typeof(char) || type == typeof(string) || type == typeof(Guid))
            {
                return PropertyValueTypeDto.String;
            }
            else
            {
                throw new NotImplementedException($"Could not translate type {type.FullName} to PropertyValueTypeDto.");
                return PropertyValueTypeDto.Complex;
            }
        }
    }
}