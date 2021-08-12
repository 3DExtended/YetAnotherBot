﻿using System;

using YAB.Api.Contracts.Models.Plugins.OptionDescriptions;

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

        public static PropertyValueTypeDto GetValueType(this Type type)
        {
            if (type == typeof(int))
            {
                return PropertyValueTypeDto.Int;
            }
            else if (type == typeof(double) || type == typeof(float))
            {
                return PropertyValueTypeDto.FloatingPoint;
            }
            else if (type == typeof(char) || type == typeof(string))
            {
                return PropertyValueTypeDto.String;
            }
            else
            {
                throw new NotImplementedException($"Could not translate type {type.FullName} to PropertyValueTypeDto.");
            }
        }
    }
}