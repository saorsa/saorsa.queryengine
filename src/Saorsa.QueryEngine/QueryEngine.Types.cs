using Saorsa.QueryEngine.Model;

namespace Saorsa.QueryEngine;

public static partial class QueryEngine
{
    public static class SimpleTypeStringKeys
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

    public static readonly Dictionary<Type, string> SimpleTypesStringMap = new()
    {
        { typeof(char), SimpleTypeStringKeys.Char },
        { typeof(string), SimpleTypeStringKeys.String },
        { typeof(bool), SimpleTypeStringKeys.Boolean },
        { typeof(byte), SimpleTypeStringKeys.Byte},
        { typeof(sbyte), SimpleTypeStringKeys.SByte },
        { typeof(short), SimpleTypeStringKeys.Int16 },
        { typeof(int), SimpleTypeStringKeys.Int32 },
        { typeof(long), SimpleTypeStringKeys.Int64 }, 
        { typeof(ushort), SimpleTypeStringKeys.UInt16 },
        { typeof(uint), SimpleTypeStringKeys.UInt32 },
        { typeof(ulong), SimpleTypeStringKeys.UInt64 },
        { typeof(float), SimpleTypeStringKeys.Float  },
        { typeof(double), SimpleTypeStringKeys.Double  },
        
        { typeof(decimal), SimpleTypeStringKeys.Decimal },
        { typeof(Guid), SimpleTypeStringKeys.Guid },
        { typeof(DateOnly), SimpleTypeStringKeys.Date },
        { typeof(DateTime), SimpleTypeStringKeys.DateTime },
        { typeof(DateTimeOffset), SimpleTypeStringKeys.DateTimeOffset },
        { typeof(TimeSpan), SimpleTypeStringKeys.TimeSpan },
        { typeof(TimeOnly), SimpleTypeStringKeys.Time },
    };
    
    public static readonly Dictionary<Type, FilterDefinition[]> SimpleTypesFiltersMap = new()
    {
        { typeof(char), FilterDefinition.NumericFilters },
        { typeof(string), FilterDefinition.TextFilters  },
        { typeof(bool), FilterDefinition.NumericFilters  },
        { typeof(byte), FilterDefinition.NumericFilters  },
        { typeof(sbyte), FilterDefinition.NumericFilters  },
        { typeof(short), FilterDefinition.NumericFilters  },
        { typeof(int), FilterDefinition.NumericFilters  },
        { typeof(long), FilterDefinition.NumericFilters  }, 
        { typeof(ushort), FilterDefinition.NumericFilters  },
        { typeof(uint), FilterDefinition.NumericFilters  },
        { typeof(ulong), FilterDefinition.NumericFilters  },
        { typeof(float), FilterDefinition.NumericFilters  },
        { typeof(double), FilterDefinition.NumericFilters  },
        
        { typeof(decimal), FilterDefinition.NumericFilters  },
        { typeof(Guid), FilterDefinition.NumericFilters  },
        { typeof(DateOnly), FilterDefinition.NumericFilters  },
        { typeof(DateTime), FilterDefinition.NumericFilters  },
        { typeof(DateTimeOffset), FilterDefinition.NumericFilters  },
        { typeof(TimeSpan), FilterDefinition.NumericFilters  },
        { typeof(TimeOnly), FilterDefinition.NumericFilters  },
    };
    
    public static readonly IEnumerable<Type> SimpleTypes = SimpleTypesStringMap.Keys;

    public static bool IsSimpleType(Type type)
    {
        return SimpleTypes.Contains(type);
    }

    public static string GetStringRepresentation(Type type)
    {
        if (IsSimpleType(type))
        {
            return SimpleTypesStringMap[type];
        }

        if (type.IsEnum)
        {
            return SpecialTypeStringKeys.Enumeration;
        }

        return type.IsSingleElementTypeEnumeration()
            ? SpecialTypeStringKeys.ArrayOrList
            : SpecialTypeStringKeys.Object;
    }

    public static FilterDefinition[] GetFilterDefinitions(Type type)
    {
        if (IsSimpleType(type))
        {
            return SimpleTypesFiltersMap[type];
        }

        if (type.IsEnum)
        {
            return FilterDefinition.EnumFilters;
        }

        return type.IsSingleElementTypeEnumeration()
            ? FilterDefinition.ArrayFilters
            : FilterDefinition.ReferenceFilters;
    }
}
