using System;
using System.Collections.Generic;
using System.Linq;
using Google.Protobuf.Collections;
using Google.Protobuf.WellKnownTypes;
using Nanover.Protocol;
using UnityEngine;

namespace Nanover.Grpc
{
    /// <summary>
    /// Utility methods for converting from protobuf data structures to C# objects.
    /// </summary>
    public static class Conversions
    {
        /// <summary>
        /// Convert a protobuf <see cref="ValueArray" /> to the corresponding C# array.
        /// </summary>
        public static object ToArray(this ValueArray valueArray)
        {
            switch (valueArray.ValuesCase)
            {
                case ValueArray.ValuesOneofCase.FloatValues:
                    return valueArray.FloatValues.Values.ToArray();
                case ValueArray.ValuesOneofCase.IndexValues:
                    return valueArray.IndexValues.Values.ToArray();
                case ValueArray.ValuesOneofCase.StringValues:
                    return valueArray.StringValues.Values.ToArray();
                default:
                    throw new ArgumentOutOfRangeException(
                        $"Cannot convert ValueArray of type {valueArray.ValuesCase} to an array.");
            }
        }

        /// <summary>
        /// Convert a protobuf <see cref="ListValue" /> to a list of C# objects.
        /// </summary>
        public static List<object> ToList(this ListValue value)
        {
            var list = new List<object>();
            foreach (var item in value.Values)
            {
                list.Add(item.ToObject());
            }
            return list;
        }

        /// <summary>
        /// Convert a protobuf <see cref="Struct" /> to a C# dictionary.
        /// </summary>
        public static Dictionary<string, object> ToDictionary(this Struct value)
        {
            var list = new Dictionary<string, object>();
            if (value == null)
                return list;
            foreach (var item in value.Fields)
            {
                list.Add(item.Key, item.Value.ToObject());
            }

            return list;
        }

        /// <summary>
        /// Convert a protobuf <see cref="ValueArray" /> to an array of
        /// <see cref="Vector3" />.
        /// </summary>
        public static Vector3[] ToVector3Array(this ValueArray valueArray)
        {
            if (valueArray.ValuesCase != ValueArray.ValuesOneofCase.FloatValues)
                throw new ArgumentException("ValueArray is of wrong type");

            var positionCoordinateArray = valueArray.FloatValues.Values;

            if (positionCoordinateArray.Count % 3 > 0)
                throw new ArgumentException("Array size is not multiple of 3");


            var positionCount = positionCoordinateArray.Count / 3;
            var positionArray = new Vector3[positionCount];

            for (var i = 0; i < positionCount; i++)
            {
                positionArray[i].x = positionCoordinateArray[3 * i];
                positionArray[i].y = positionCoordinateArray[3 * i + 1];
                positionArray[i].z = positionCoordinateArray[3 * i + 2];
            }

            return positionArray;
        }

        /// <summary>
        /// Convert a protobuf <see cref="Value" /> to the corresponding C# type.
        /// </summary>
        public static object ToObject(this Value value)
        {
            switch (value.KindCase)
            {
                case Value.KindOneofCase.NullValue:
                    return null;
                case Value.KindOneofCase.NumberValue:
                    return value.NumberValue;
                case Value.KindOneofCase.StringValue:
                    return value.StringValue;
                case Value.KindOneofCase.BoolValue:
                    return value.BoolValue;
                case Value.KindOneofCase.ListValue:
                    return value.ListValue.ToList();
                case Value.KindOneofCase.StructValue:
                    return value.StructValue.ToDictionary();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Convert a C# dictionary to a protobuf Value (as a Struct).
        /// </summary>
        public static Value ToProtobufValue(this IDictionary<string, object> dictionary)
        {
            return Value.ForStruct(dictionary.ToProtobufStruct());
        }
        
        /// <summary>
        /// Convert a C# dictionary to a protobuf Struct.
        /// </summary>
        public static Struct ToProtobufStruct(this IDictionary<string, object> dictionary)
        {
            var @struct = new Struct();

            foreach (var pair in dictionary)
            {
                @struct.Fields.Add(pair.Key, pair.Value.ToProtobufValue());
            }
            
            return @struct;
        }

        /// <summary>
        /// Convert a C# IEnumerable to a protobuf Value (as a Value list).
        /// </summary>
        public static Value ToProtobufValue(this IEnumerable<object> enumerable)
        {
            var values = enumerable.Select(@object => @object.ToProtobufValue());
            return Value.ForList(values.ToArray());
        }

        /// <summary>
        /// Convert a C# object to a protobuf Value.
        /// </summary>
        public static Value ToProtobufValue(this object @object)
        {
            switch (@object)
            {
                case Value value:
                    return value;
                case null:
                    return Value.ForNull();
                case bool boolean:
                    return Value.ForBool(boolean);
                case int number:
                    return Value.ForNumber(number);
                case float number:
                    return Value.ForNumber(number);
                case double number:
                    return Value.ForNumber(number);
                case string @string:
                    return Value.ForString(@string);
                case IDictionary<string, object> dictionary:
                    return dictionary.ToProtobufValue();
                case IEnumerable<object> enumerable:
                    return enumerable.ToProtobufValue();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// Retrieve a Vector3 from a Protobuf array of floats, optionally
        /// starting from a given offset in the array.
        /// </summary>
        public static Vector3 GetVector3(this RepeatedField<float> values,
                                          int offset = 0)
        {
            return new Vector3(values[0 + offset],
                               values[1 + offset],
                               values[2 + offset]);
        }

        /// <summary>
        /// Retrieve a Quaternion from a Protobuf array of floats, optionally
        /// starting from a given offset in the array.
        /// </summary>
        public static Quaternion GetQuaternion(this RepeatedField<float> values,
                                               int offset = 0)
        {
            return new Quaternion(values[0 + offset],
                                  values[1 + offset],
                                  values[2 + offset],
                                  values[3 + offset]);
        }

        /// <summary>
        /// Append the components of a Vector3 to a Protobuf array of floats.
        /// </summary>
        public static void PutValues(this RepeatedField<float> values,
                                     Vector3 vector)
        {
            values.Add(vector.x);
            values.Add(vector.y);
            values.Add(vector.z);
        }

        /// <summary>
        /// Append the components of a Quaternion to a Protobuf array of floats.
        /// </summary>
        public static void PutValues(this RepeatedField<float> values,
                                     Quaternion quaternion)
        {
            values.Add(quaternion.x);
            values.Add(quaternion.y);
            values.Add(quaternion.z);
            values.Add(quaternion.w);
        }

        /// <summary>
        /// Retrieve a Vector3 from a list of objects, optionally
        /// starting from a given offset in the array.
        /// </summary>
        public static Vector3 GetVector3(this IReadOnlyList<object> values,
                                         int offset = 0)
        {
            return new Vector3(Convert.ToSingle(values[0 + offset]),
                               Convert.ToSingle(values[1 + offset]),
                               Convert.ToSingle(values[2 + offset]));
        }

        /// <summary>
        /// Retrieve a Quaternion from a list of objects, optionally
        /// starting from a given offset in the array.
        /// </summary>
        public static Quaternion GetQuaternion(this IReadOnlyList<object> values,
                                               int offset = 0)
        {
            return new Quaternion(Convert.ToSingle(values[0 + offset]),
                                  Convert.ToSingle(values[1 + offset]),
                                  Convert.ToSingle(values[2 + offset]),
                                  Convert.ToSingle(values[3 + offset]));
        }
        
        /// <summary>
        /// Convert a protobuf <see cref="Value" /> to an integer.
        /// </summary>
        public static int ToInt(this Value value)
        {
            if(value.KindCase != Value.KindOneofCase.NumberValue)
                throw new ArgumentException("Value is not numeric, and cannot be converted to an integer");
            return (int) value.NumberValue;
        }
                
        /// <summary>
        /// Convert a C# dictionary to a protobuf Struct.
        /// </summary>
        public static Struct ToProtobufStruct<T>(this IDictionary<string, T> dictionary)
        {
            var @struct = new Struct();

            foreach (var pair in dictionary)
            {
                @struct.Fields.Add(pair.Key, pair.Value.ToProtobufValue());
            }
            
            return @struct;
        }

    }
}