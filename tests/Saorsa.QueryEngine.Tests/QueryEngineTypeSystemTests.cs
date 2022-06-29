namespace Saorsa.QueryEngine.Tests;

public class QueryEngineTypeSystemTests
{
    private readonly Type[] NullableAtomTypes =
    {
        typeof(char?),
        typeof(byte?),
        typeof(sbyte?),
        typeof(short?),
        typeof(int?),
        typeof(long?),
        typeof(ushort?),
        typeof(uint?),
        typeof(ulong?),
        typeof(bool?),
        typeof(decimal?),
        typeof(float?),
        typeof(double?),
        typeof(Guid?),
        typeof(DateTime?),
        typeof(DateTimeOffset?),
        typeof(DateOnly?),
        typeof(TimeOnly?),
    };
    
    [Test]
    public void TestSimpleTypesMapsIntegrity()
    {
        QueryEngine.AtomicTypes.ToList().ForEach(type => {
            Assert.Multiple(() =>
            {
                Assert.That(QueryEngine.AtomicTypesStringMap.ContainsKey(type), Is.True,
                    $"Type '{type}' must be registered in the {nameof(QueryEngine.AtomicTypesStringMap)} map.");
                Assert.That(QueryEngine.AtomicTypesFiltersMap.ContainsKey(type), Is.True,
                    $"Type '{type}' must be registered in the {nameof(QueryEngine.AtomicTypesFiltersMap)} map.");
                Assert.That(QueryEngine.AtomicTypes.Contains(type), Is.True,
                    $"Type '{type}' must be available via the {nameof(QueryEngine.AtomicTypes)} enumeration.");
            });
        });
    }

    [Test]
    public void TestSimpleTypesIntegrity()
    {
        QueryEngine.AtomicTypes.ToList().ForEach(type => {
            Assert.Multiple(() =>
            {
                Assert.That(QueryEngine.IsAtomicType(type));
                Assert.That(type.IsQueryEngineSimpleType(), Is.True);
            });
        });
    }
    
    [TestCase(42)]
    [TestCase(short.MinValue)]
    [TestCase(ushort.MinValue)]
    [TestCase(byte.MinValue)]
    [TestCase(sbyte.MinValue)]
    [TestCase(true)]
    [TestCase('c')]
    [TestCase("string")]
    [TestCase(1.2d)]
    [TestCase(1u)]
    [TestCase(1.12f)]
    [TestCase(1L)]
    [TestCase(1UL)]
    [TestCase(1.0121)]
    [TestCase(0x1)]
    [TestCase(0x00000012)]
    [TestCase((sbyte)0x00000012)]
    public void TestQueryEngineSimpleTypeObjectCase(object candidate)
    {
        var type = candidate.GetType();
        Assert.That(type, Is.Not.Null);
        Assert.Multiple(() =>
        {
            Assert.That(QueryEngine.IsAtomicType(type),
                $"Type '{type}' is a simple type.");
            Assert.That(type.IsQueryEngineSimpleType(), Is.True,
                $"Type '{type}' is a simple type.");
        });
    }

    [Test]
    public void TestQueryEngineSimpleTypeStructureObjects()
    {
        new List<object>
        {
            decimal.MinusOne,
            DateTime.Now,
            DateOnly.MinValue,
            TimeSpan.Zero,
            TimeOnly.MinValue,
            DateTimeOffset.Now,
        }.ForEach(TestQueryEngineSimpleTypeObjectCase);
    }

    [Test]
    public void TestStringRepresentationOnEnums()
    {
        AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => t.IsEnum)
            .ToList()
            .ForEach(enumType =>
            {
                var stringKey = QueryEngine.GetStringRepresentation(enumType);
                Assert.That(stringKey, Is.EqualTo(QueryEngine.SpecialTypeStringKeys.Enumeration),
                    $"Type '{enumType}' is enumeration and its string representation must be " +
                    $"'{QueryEngine.SpecialTypeStringKeys.Enumeration}'.");
            });
    }
    
    [Test]
    public void TestStringRepresentationOnSimpleTypes()
    {
       QueryEngine.AtomicTypes
            .ToList()
            .ForEach(simpleType =>
            {
                var stringKey = QueryEngine.GetStringRepresentation(simpleType);
                Assert.That(stringKey, Is.EqualTo(QueryEngine.AtomicTypesStringMap[simpleType]));
            });
    }

    [Test]
    public void TestIsAtomicTypes()
    {
        QueryEngine.AtomicTypes
            .ToList()
            .ForEach(simpleType =>
            {
                Assert.That(QueryEngine.IsAtomicType(simpleType), Is.True,
                    $"Type '{simpleType}' is expected to be an atomic type for query engine runtime.");
            });
    }

    [TestCase(typeof(int), default(int))]
    [TestCase(typeof(short), default(short))]
    [TestCase(typeof(long), default(long))]
    [TestCase(typeof(ushort), default(ushort))]
    [TestCase(typeof(uint), default(uint))]
    [TestCase(typeof(ulong), default(ulong))]
    [TestCase(typeof(char), '\0')]
    [TestCase(typeof(float), default(float))]
    [TestCase(typeof(double), default(double))]
    [TestCase(typeof(int?), null)]
    [TestCase(typeof(short?), null)]
    [TestCase(typeof(long?), null)]
    [TestCase(typeof(ushort?), null)]
    [TestCase(typeof(uint?), null)]
    [TestCase(typeof(ulong?), null)]
    [TestCase(typeof(char?), null)]
    [TestCase(typeof(decimal?), null)]
    [TestCase(typeof(float?), null)]
    [TestCase(typeof(double?), null)]
    public void TestTypeDefaultsForNonStructs(Type type, object? expected)
    {
        var result = type.GetDefaultValue();
        if (result != null)
        {
            Assert.NotNull(expected);
            Assert.That(result.Equals(expected!), Is.True);
        }
        else
        {
            Assert.IsNull(expected);
        }
    }

    [Test]
    public void TestTypeDefaultsForStructDecimal()
    {
        var result = typeof(decimal).GetDefaultValue();
        var expected = decimal.Zero;
        Assert.NotNull(result);
        Assert.That(result is decimal);
        Assert.That((decimal)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }
    
    [Test]
    public void TestTypeDefaultsForStructDateTime()
    {
        var result = typeof(DateTime).GetDefaultValue();
        var expected = new DateTime();
        Assert.NotNull(result);
        Assert.That(result is DateTime);
        Assert.That((DateTime)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }
    
    [Test]
    public void TestTypeDefaultsForStructDateOnly()
    {
        var result = typeof(DateOnly).GetDefaultValue();
        var expected = new DateOnly();
        Assert.NotNull(result);
        Assert.That(result is DateOnly);
        Assert.That((DateOnly)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }
    
    [Test]
    public void TestTypeDefaultsForStructDateTimeOffset()
    {
        var result = typeof(DateTimeOffset).GetDefaultValue();
        var expected = new DateTimeOffset();
        Assert.NotNull(result);
        Assert.That(result is DateTimeOffset);
        Assert.That((DateTimeOffset)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }
    
    [Test]
    public void TestTypeDefaultsForStructTimeSpan()
    {
        var result = typeof(TimeSpan).GetDefaultValue();
        var expected = new TimeSpan();
        Assert.NotNull(result);
        Assert.That(result is TimeSpan);
        Assert.That((TimeSpan)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }
    
    [Test]
    public void TestTypeDefaultsForStructTimeOnly()
    {
        var result = typeof(TimeOnly).GetDefaultValue();
        var expected = new TimeOnly();
        Assert.NotNull(result);
        Assert.That(result is TimeOnly);
        Assert.That((TimeOnly)(result ?? throw new InvalidOperationException()) == expected, Is.True);
    }

    [Test]
    public void TestConvertToAtomFromNullString()
    {
        string? nullString = default;

        foreach (var nullableAtomType in NullableAtomTypes)
        {
            Assert.That(nullableAtomType.IsNullable,
                $"Type '{nullableAtomType}' is expected to be a nullable type.");
            
            var underlyingType = nullableAtomType.GetUnderlyingTypeIfNullable();
            Assert.That(underlyingType, Is.Not.EqualTo(nullableAtomType));
            
            var expectedNullResult = QueryEngine.ConvertToAtom(nullString, nullableAtomType);
            Assert.That(expectedNullResult, Is.Null,
                $"Type '{nullableAtomType}' should be null when converted from a null string.");

            var underlyingDefaultValue = underlyingType.GetDefaultValue();
            Assert.That(underlyingDefaultValue, Is.Not.Null);
            Assert.Throws<QueryEngineException>(() => QueryEngine.ConvertToAtom(nullString, underlyingType));
        }
    }

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("-42")]
    [TestCase("16")]
    public void TestConvertToInt16AtomFromNonNullString(string stringValue)
    {
        var intValue = QueryEngine.ConvertToAtom<short>(stringValue);
        Assert.IsNotNull(intValue);
        Assert.That(short.Parse(stringValue), Is.EqualTo(intValue));
        
        var nullableIntValue = QueryEngine.ConvertToAtom<short?>(stringValue);
        Assert.That(nullableIntValue, Is.Not.Null);
        Assert.That(nullableIntValue.HasValue);
        Assert.That(short.Parse(stringValue), Is.EqualTo(nullableIntValue));
    }

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("-42")]
    [TestCase("16")]
    public void TestConvertToInt32AtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<int>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(int.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<int?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(int.Parse(stringValue), Is.EqualTo(nullableVal));
    }

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("-42")]
    [TestCase("16452246982746")]
    public void TestConvertToInt64AtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<long>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(long.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<long?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(long.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("42")]
    [TestCase("16")]
    public void TestConvertToUInt16AtomFromNonNullString(string stringValue)
    {
        var intValue = QueryEngine.ConvertToAtom<ushort>(stringValue);
        Assert.IsNotNull(intValue);
        Assert.That(ushort.Parse(stringValue), Is.EqualTo(intValue));
        
        var nullableIntValue = QueryEngine.ConvertToAtom<ushort?>(stringValue);
        Assert.That(nullableIntValue, Is.Not.Null);
        Assert.That(nullableIntValue.HasValue);
        Assert.That(ushort.Parse(stringValue), Is.EqualTo(nullableIntValue));
    }

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("42")]
    [TestCase("16")]
    public void TestConvertToUInt32AtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<uint>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(uint.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<uint?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(uint.Parse(stringValue), Is.EqualTo(nullableVal));
    }

    [TestCase("0")]
    [TestCase("1000")]
    [TestCase("42")]
    [TestCase("16452246982746")]
    public void TestConvertToUInt64AtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<ulong>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(ulong.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<ulong?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(ulong.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    
    [TestCase("3.1557262")]
    [TestCase("1000.0")]
    [TestCase("-42.06138123")]
    [TestCase("16.18623")]
    public void TestConvertToDecimalAtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<decimal>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(decimal.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<decimal?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(decimal.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    
    [TestCase("3.1557262")]
    [TestCase("1000.0")]
    [TestCase("-42.06138123")]
    [TestCase("16.18623")]
    public void TestConvertToFloatAtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<float>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(float.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<float?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(float.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    
    [TestCase("3.1557262")]
    [TestCase("1000.0")]
    [TestCase("-42.06138123")]
    [TestCase("16.18623")]
    public void TestConvertToDoubleAtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<double>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(double.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<double?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(double.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    
    [TestCase("2001-07-24")]
    [TestCase("2001.07.24")]
    [TestCase("2001/07/24")]
    [TestCase("2001/07/24 12:45")]
    [TestCase("2001/07/24 12:45:12.4")]
    [TestCase("2001/07/24T12:45:12.4")]
    [TestCase("2001/07/24T12:45Z")]
    public void TestConvertToDateTimeAtomFromNonNullString(string stringValue)
    {
        var val = QueryEngine.ConvertToAtom<DateTime>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(DateTime.Parse(stringValue), Is.EqualTo(val));
        
        var nullableVal = QueryEngine.ConvertToAtom<DateTime?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(DateTime.Parse(stringValue), Is.EqualTo(nullableVal));
    }
    
    [TestCase("2001-07-24")]
    [TestCase("2001.07.24")]
    [TestCase("2001/07/24")]
    [TestCase("2001/07/24 12:45")]
    [TestCase("2001/07/24 12:45:12.4")]
    [TestCase("2001/07/24T12:45:12.4")]
    [TestCase("2001/07/24T12:45Z")]
    public void TestConvertToDateTimeOffsetAtomFromNonNullString(string stringValue)
    {
        var objectVal = QueryEngine.ConvertToAtom(stringValue, typeof(DateTimeOffset));
        Assert.IsNotNull(objectVal);
        
        var val = QueryEngine.ConvertToAtom<DateTimeOffset>(stringValue);
        Assert.IsNotNull(val);
        Assert.That(DateTimeOffset.Parse(stringValue), Is.EqualTo(val));
        Assert.That(val, Is.EqualTo(objectVal));
        
        var nullableVal = QueryEngine.ConvertToAtom<DateTimeOffset?>(stringValue);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        Assert.That(DateTimeOffset.Parse(stringValue), Is.EqualTo(nullableVal));
    }

    [TestCase('c', 'c')]
    [TestCase(25u, '\u0019')]
    [TestCase(42, '*')]
    [TestCase("s", 's')]
    public void TestConvertToCharAtom(object candidate, char expected)
    {
        var objectVal = QueryEngine.ConvertToAtom(candidate, typeof(char));
        Assert.IsNotNull(objectVal);
        
        var val = QueryEngine.ConvertToAtom<char>(candidate);
        Assert.IsNotNull(val);
        
        var nullableVal = QueryEngine.ConvertToAtom<char?>(candidate);
        Assert.That(nullableVal, Is.Not.Null);
        Assert.That(nullableVal.HasValue);
        
        Assert.That(objectVal, Is.EqualTo(val));
        Assert.That(objectVal, Is.EqualTo(nullableVal));
        Assert.That(val, Is.EqualTo(nullableVal));
        
        Assert.That(val, Is.EqualTo(expected));
    }
    
    [TestCase('c', "c")]
    [TestCase(25u, "25")]
    [TestCase(25.3, "25.3")]
    [TestCase(25.3d, "25.3")]
    [TestCase(42, "42")]
    [TestCase("s", "s")]
    public void TestConvertToStringAtom(object candidate, string expected)
    {
        var objectVal = QueryEngine.ConvertToAtom(candidate, typeof(string));
        Assert.IsNotNull(objectVal);
        
        var val = QueryEngine.ConvertToAtom<string>(candidate);
        Assert.IsNotNull(val);
        
        var nullableVal = QueryEngine.ConvertToAtom<string?>(candidate);
        Assert.That(nullableVal, Is.Not.Null);
        
        Assert.That(objectVal, Is.EqualTo(val));
        Assert.That(objectVal, Is.EqualTo(nullableVal));
        Assert.That(val, Is.EqualTo(nullableVal));
        
        Assert.That(val, Is.EqualTo(expected));
    }
}
