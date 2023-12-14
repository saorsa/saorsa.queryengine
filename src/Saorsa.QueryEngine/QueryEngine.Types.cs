using System.Globalization;
using System.Text.Json;
using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    public static class AtomicTypeStringKeys
    {
        public const string Char = "char";
        public const string Boolean = "boolean";
        public const string Byte = "byte";
        public const string SByte = "sbyte";
        public const string Int16 = "int16";
        public const string Int32 = "integer";
        public const string Int64 = "int64";
        public const string UInt16 = "uint16";
        public const string UInt32 = "uint32";
        public const string UInt64 = "uint64";
        public const string Float = "float";
        public const string Double = "double";

        public const string String = "string";
        public const string Decimal = "decimal";
        public const string Guid = "uuid";
        public const string Date = "date";
        public const string DateTime = "dateTime";
        public const string DateTimeOffset = "dateTimeOffset";
        public const string Time = "time";
        public const string TimeSpan = "timeSpan";
    }

    public static class SpecialTypeStringKeys
    {
        public const string Enumeration = "enum";
        public const string ArrayOrList = "array";
        public const string Object = "object";
    }

    public static readonly Dictionary<Type, string> AtomicTypesStringMap = new()
    {
        { typeof(char), AtomicTypeStringKeys.Char },
        { typeof(string), AtomicTypeStringKeys.String },
        { typeof(bool), AtomicTypeStringKeys.Boolean },
        { typeof(byte), AtomicTypeStringKeys.Byte},
        { typeof(sbyte), AtomicTypeStringKeys.SByte },
        { typeof(short), AtomicTypeStringKeys.Int16 },
        { typeof(int), AtomicTypeStringKeys.Int32 },
        { typeof(long), AtomicTypeStringKeys.Int64 }, 
        { typeof(ushort), AtomicTypeStringKeys.UInt16 },
        { typeof(uint), AtomicTypeStringKeys.UInt32 },
        { typeof(ulong), AtomicTypeStringKeys.UInt64 },
        { typeof(float), AtomicTypeStringKeys.Float  },
        { typeof(double), AtomicTypeStringKeys.Double  },
        
        { typeof(decimal), AtomicTypeStringKeys.Decimal },
        { typeof(Guid), AtomicTypeStringKeys.Guid },
        { typeof(DateOnly), AtomicTypeStringKeys.Date },
        { typeof(DateTime), AtomicTypeStringKeys.DateTime },
        { typeof(DateTimeOffset), AtomicTypeStringKeys.DateTimeOffset },
        { typeof(TimeSpan), AtomicTypeStringKeys.TimeSpan },
        { typeof(TimeOnly), AtomicTypeStringKeys.Time },
    };
    
    public static readonly Dictionary<Type, FilterMetaData[]> AtomicTypesFiltersMap = new()
    {
        { typeof(char), FilterMetaData.NumericFilters },
        { typeof(string), FilterMetaData.TextFilters  },
        { typeof(bool), FilterMetaData.NumericFilters  },
        { typeof(byte), FilterMetaData.NumericFilters  },
        { typeof(sbyte), FilterMetaData.NumericFilters  },
        { typeof(short), FilterMetaData.NumericFilters  },
        { typeof(int), FilterMetaData.NumericFilters  },
        { typeof(long), FilterMetaData.NumericFilters  }, 
        { typeof(ushort), FilterMetaData.NumericFilters  },
        { typeof(uint), FilterMetaData.NumericFilters  },
        { typeof(ulong), FilterMetaData.NumericFilters  },
        { typeof(float), FilterMetaData.NumericFilters  },
        { typeof(double), FilterMetaData.NumericFilters  },
        
        { typeof(decimal), FilterMetaData.NumericFilters  },
        { typeof(Guid), FilterMetaData.NumericFilters  },
        { typeof(DateOnly), FilterMetaData.NumericFilters  },
        { typeof(DateTime), FilterMetaData.NumericFilters  },
        { typeof(DateTimeOffset), FilterMetaData.NumericFilters  },
        { typeof(TimeSpan), FilterMetaData.NumericFilters  },
        { typeof(TimeOnly), FilterMetaData.NumericFilters  },
    };
    
    public static readonly IEnumerable<Type> AtomicTypes = AtomicTypesStringMap.Keys;

    public static bool IsAtomicType(Type type)
    {
        return AtomicTypes.Contains(type);
    }

    public static string GetStringRepresentation(Type type)
    {
        if (IsAtomicType(type))
        {
            return AtomicTypesStringMap[type];
        }

        if (type.IsEnum)
        {
            return SpecialTypeStringKeys.Enumeration;
        }

        return type.IsSingleElementTypeEnumeration()
            ? SpecialTypeStringKeys.ArrayOrList
            : SpecialTypeStringKeys.Object;
    }

    public static FilterMetaData[] GetFilterDefinitions(Type type)
    {
        if (IsAtomicType(type))
        {
            return AtomicTypesFiltersMap[type];
        }

        if (type.IsEnum)
        {
            return FilterMetaData.EnumFilters;
        }

        return type.IsSingleElementTypeEnumeration()
            ? FilterMetaData.ArrayFilters
            : FilterMetaData.ReferenceFilters;
    }
    
    public static TAtom? ConvertToAtom<TAtom>(object? source)
    {
        var result = ConvertToAtom(source, typeof(TAtom));
        if (result != null)
        {
            return (TAtom)result;
        }

        return default;
    }
    
    public static object? ConvertToAtom(
        object? source,
        Type targetType)
    {
        var isValueType = targetType.IsValueType;
        var isNullable = targetType.IsNullable();
        var required = isValueType && !isNullable;
        
        if (source == null)
        {
            if (required)
            {
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Conversion to '{targetType.Name}' is required, but the source value is null.");
            }

            return targetType.GetDefaultValue();
        }
        
        if (source is JsonElement json)
        {
            return ConvertToAtom(json, targetType);
        }

        var underlyingType = targetType.GetUnderlyingTypeIfNullable();

        if (underlyingType.IsEnum)
        {
            var sourceString = source as string ?? Convert.ToString(source);
            return (int)Enum.Parse(underlyingType, sourceString!);
        }
        if (underlyingType == typeof(char))
        {
            return source is char v ? v : Convert.ToChar(source);
        }
        if (underlyingType == typeof(string))
        {
            return source as string ?? Convert.ToString(source);
        }
        if (underlyingType == typeof(bool))
        {
            return source is bool v ? v : Convert.ToBoolean(source);
        }
        if (underlyingType == typeof(byte))
        {
            return source is byte v ? v : Convert.ToByte(source);
        }
        if (underlyingType == typeof(sbyte))
        {
            return source is sbyte v ? v : Convert.ToSByte(source);
        }
        if (underlyingType == typeof(short))
        {
            return source is short v ? v : Convert.ToInt16(source);
        }
        if (underlyingType == typeof(int))
        {
            return source is int v ? v : Convert.ToInt32(source);
        }
        if (underlyingType == typeof(long))
        {
            return source is int v ? v : Convert.ToInt64(source);
        }
        if (underlyingType == typeof(ushort))
        {
            return source is ushort v ? v : Convert.ToUInt16(source);
        }
        if (underlyingType == typeof(uint))
        {
            return source is uint v ? v : Convert.ToUInt32(source);
        }
        if (underlyingType == typeof(ulong))
        {
            return source is ulong v ? v : Convert.ToUInt64(source);
        }
        if (underlyingType == typeof(float))
        {
            return source is float v ? v : Convert.ToSingle(source);
        }
        if (underlyingType == typeof(double))
        {
            return source is double v ? v : Convert.ToDouble(source);
        }
        if (underlyingType == typeof(decimal))
        {
            return source is decimal v ? v : Convert.ToDecimal(source);
        }
        if (underlyingType == typeof(Guid))
        {
            return source is Guid v ? v : Guid.Parse(source.ToString()!);
        }
        if (underlyingType == typeof(DateTime))
        {
            return source is DateTime v ? v : DateTime.Parse(source.ToString()!);
        }
        if (underlyingType == typeof(DateOnly))
        {
            return source is DateOnly v ? v : DateOnly.Parse(source.ToString()!);
        }
        if (underlyingType == typeof(DateTimeOffset))
        {
            return source is DateTimeOffset v ? v : DateTimeOffset.Parse(source.ToString()!);
        }
        if (underlyingType == typeof(TimeSpan))
        {
            return source is TimeSpan v ? v : TimeSpan.Parse(source.ToString()!);
        }
        if (underlyingType == typeof(TimeOnly))
        {
            return source is TimeOnly v ? v : TimeOnly.Parse(source.ToString()!);
        }
        
        throw new QueryEngineException(
            ErrorCodes.TypeConversionError,
            $"Value '{source}' cannot be represented as '{targetType.Name}/{underlyingType.Name}'");
    }

    public static TAtom? ConvertToAtom<TAtom>(JsonElement jsonRef)
    {
        var result = ConvertToAtom(jsonRef, typeof(TAtom));
        if (result != null)
        {
            return (TAtom)result;
        }

        return default;
    }

    public static object? ConvertToAtom(JsonElement jsonRef, Type targetType)
    {
        var isValueType = targetType.IsValueType;
        var isNullable = targetType.IsNullable();
        var required = isValueType && !isNullable;
        var underlyingType = targetType.GetUnderlyingTypeIfNullable();

        switch (jsonRef.ValueKind)
        {
            case JsonValueKind.Undefined:
                if (required)
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionError,
                        $"Conversion to '{targetType.Name}' is required, but the source JsonElement " +
                        $"value is undefined.");
                }

                return targetType.GetDefaultValue();

            case JsonValueKind.Object:
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionAtomValueExpectedError,
                    $"Conversion to '{targetType.Name}' is not possible, because the source JsonElement " +
                    $"'{jsonRef.ValueKind}' / '{jsonRef.ToString()}' is not an atom.");

            case JsonValueKind.Array:
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionAtomValueExpectedError,
                    $"Conversion to '{targetType.Name}' is not possible, because the source JsonElement " +
                    $"'{jsonRef.ValueKind}' / '{jsonRef.ToString()}' is not an atom.");

            case JsonValueKind.String:
            {
                var stringVal = jsonRef.GetString()!;
                if (underlyingType == typeof(char))
                {
                    return Convert.ToChar(stringVal);
                }
                if (underlyingType == typeof(string))
                {
                    return stringVal;
                }
                if (underlyingType == typeof(bool))
                {
                    return Convert.ToBoolean(stringVal);
                }
                if (underlyingType == typeof(byte))
                {
                    return Convert.ToByte(stringVal);
                }
                if (underlyingType == typeof(sbyte))
                {
                    return Convert.ToSByte(stringVal);
                }
                if (underlyingType == typeof(short))
                {
                    return Convert.ToInt16(stringVal);
                }
                if (underlyingType == typeof(int))
                {
                    return Convert.ToInt32(stringVal);
                }
                if (underlyingType == typeof(long))
                {
                    return Convert.ToInt64(stringVal);
                }
                if (underlyingType == typeof(ushort))
                {
                    return Convert.ToInt16(stringVal);
                }
                if (underlyingType == typeof(uint))
                {
                    return Convert.ToUInt32(stringVal);
                }
                if (underlyingType == typeof(ulong))
                {
                    return Convert.ToUInt64(stringVal);
                }
                if (underlyingType == typeof(float))
                {
                    return Convert.ToSingle(stringVal);
                }
                if (underlyingType == typeof(double))
                {
                    return Convert.ToDouble(stringVal);
                }
                if (underlyingType == typeof(decimal))
                {
                    return Convert.ToDecimal(stringVal);
                }
                if (underlyingType == typeof(Guid))
                {
                    return Guid.Parse(stringVal);
                }
                if (underlyingType == typeof(DateTime))
                {
                    return DateTime.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (underlyingType == typeof(DateOnly))
                {
                    return DateOnly.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (underlyingType == typeof(DateTimeOffset))
                {
                    return DateTimeOffset.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (underlyingType == typeof(TimeSpan))
                {
                    return TimeSpan.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                if (underlyingType == typeof(TimeOnly))
                {
                    return TimeOnly.Parse(stringVal, CultureInfo.InvariantCulture);
                }
                
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"String value '{jsonRef.ToString()}' cannot be represented an atom of type " +
                    $"'{targetType.Name}'.");
            }

            case JsonValueKind.Number:
                if (underlyingType == typeof(char))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                if (underlyingType == typeof(string))
                {
                    var val = jsonRef.GetInt32();
                    return val.ToString();
                }
                if (underlyingType == typeof(bool))
                {
                    var val = jsonRef.GetInt32();
                    return val != 0;
                }
                if (underlyingType == typeof(byte))
                {
                    return jsonRef.GetByte();
                }
                if (underlyingType == typeof(sbyte))
                {
                    return jsonRef.GetSByte();
                }
                if (underlyingType == typeof(short))
                {
                    return jsonRef.GetInt16();
                }
                if (underlyingType == typeof(int))
                {
                    return jsonRef.GetInt32();
                }
                if (underlyingType == typeof(long))
                {
                    return jsonRef.GetInt64();
                }
                if (underlyingType == typeof(ushort))
                {
                    return jsonRef.GetUInt16();
                }
                if (underlyingType == typeof(uint))
                {
                    return jsonRef.GetUInt32();
                }
                if (underlyingType == typeof(ulong))
                {
                    return jsonRef.GetUInt64();
                }
                if (underlyingType == typeof(float))
                {
                    return jsonRef.GetSingle();
                }
                if (underlyingType == typeof(double))
                {
                    return jsonRef.GetDouble();
                }
                if (underlyingType == typeof(decimal))
                {
                    return jsonRef.GetDecimal();
                }
                if (underlyingType == typeof(Guid))
                {
                    return jsonRef.GetGuid();
                }
                if (underlyingType == typeof(DateTime))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                if (underlyingType == typeof(DateOnly))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                if (underlyingType == typeof(DateTimeOffset))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                if (underlyingType == typeof(TimeSpan))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                if (underlyingType == typeof(TimeOnly))
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionNotSupportedError,
                        $"Json number '{jsonRef.ToString()}' conversion to '{targetType.Name}' is " +
                        $"not supported.");
                }
                
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{jsonRef.ToString()}' cannot be represented as '{targetType.Name}'");
                
            case JsonValueKind.True:
                if (underlyingType == typeof(bool))
                {
                    return jsonRef.GetBoolean();
                }
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{jsonRef.ToString()}' cannot be represented as '{targetType.Name}'");

            case JsonValueKind.False:
                if (underlyingType == typeof(bool))
                {
                    return jsonRef.GetBoolean();
                }
                throw new QueryEngineException(
                    ErrorCodes.TypeConversionError,
                    $"Value '{jsonRef.ToString()}' cannot be represented as '{targetType.Name}'");

            case JsonValueKind.Null:
                if (required)
                {
                    throw new QueryEngineException(
                        ErrorCodes.TypeConversionError,
                        $"Conversion to '{targetType.Name}' is required, but the source JsonElement " +
                        $"value is null.");
                }

                return targetType.GetDefaultValue();

            default:
                throw new NotSupportedException($"JsonElement '{jsonRef.ValueKind}' is not supported.");
        }
    }
}
