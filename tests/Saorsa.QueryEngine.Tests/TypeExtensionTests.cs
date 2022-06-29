namespace Saorsa.QueryEngine.Tests;

public class TypeExtensionTests
{
    [Test]
    public void TestTypeDefaultValueForNullables()
    {
        foreach (var nullableAtomType in QueryEngineTypeSystemTests.NullableAtomTypes)
        {
            var defaultVal = nullableAtomType.GetDefaultValue();
            Assert.That(defaultVal, Is.Null);

            var nonNullableType = nullableAtomType.GetUnderlyingTypeIfNullable();
            var defaultNonNullableVal = nonNullableType.GetDefaultValue();
            Assert.That(defaultNonNullableVal, Is.Not.Null);
        }
    }
}
