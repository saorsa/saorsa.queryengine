namespace Saorsa.QueryEngine.Model;

public static class RefType
{
    public static readonly string Char = "char";
    public static readonly string Boolean = "boolean";
    public static readonly string Byte = "byte";
    public static readonly string SByte = "sbyte";
    public static readonly string Int16 = "int16";
    public static readonly string Int32 = "integer";
    public static readonly string Int64 = "int64";
    public static readonly string UInt16 = "uint16";
    public static readonly string UInt32 = "uint32";
    public static readonly string UInt64 = "uint64";
    
    public static readonly string String = "string";
    public static readonly string Decimal = "decimal";
    public static readonly string UUID = "uuid";
    public static readonly string Date = "date";
    public static readonly string DateTime = "dateTime";
    public static readonly string DateTimeOffset = "dateTimeOffset";
    public static readonly string Time = "time";
    public static readonly string TimeSpan = "timeSpan";
    
    public static readonly Dictionary<Type, string> SimpleTypeStringMap = new()
    {
        { typeof(char), Char },
        { typeof(string), String },
        { typeof(bool), Boolean },
        { typeof(byte), Byte},
        { typeof(sbyte), SByte },
        { typeof(short), "int16" },
        { typeof(int), "integer" },
        { typeof(long), "int64" }, 
        { typeof(ushort), "uint16" },
        { typeof(uint), "uint32" },
        { typeof(ulong), "uint64" },
        
        { typeof(decimal), "decimal" },
        { typeof(Guid), "uuid" },
        { typeof(DateOnly), "date" },
        { typeof(DateTime), "dateTime" },
        { typeof(DateTimeOffset), "dateTimeOffset" },
        { typeof(TimeSpan), "timeSpan" },
        { typeof(TimeOnly), "time" },
    };
}
