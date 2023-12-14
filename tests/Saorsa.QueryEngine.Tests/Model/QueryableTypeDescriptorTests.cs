namespace Saorsa.QueryEngine.Tests.Model;

public class QueryableTypeDescriptorTests
{
    [Test]
    public void TestTypeName()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.TypeName, Is.Not.Null);

        var newName = Guid.NewGuid().ToString("N");
        typeDef.TypeName = newName;
        
        Assert.That(newName, Is.EqualTo(typeDef.TypeName));
    }
    
    [Test]
    public void TestType()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.Type, Is.Null);

        var type = Guid.NewGuid().ToString("N");
        typeDef.Type = type;
        
        Assert.That(type, Is.EqualTo(typeDef.Type));
    }
    
    [Test]
    public void TestNullable()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.Nullable, Is.False);

        typeDef.Nullable = true;
        
        Assert.That(typeDef.Nullable, Is.EqualTo(true));
    }
    
    [Test]
    public void TestEnumValues()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.EnumValues, Is.Null);

        typeDef.EnumValues = new[] {string.Empty};
        
        Assert.That(typeDef.EnumValues, Is.Not.Null);
        Assert.That(typeDef.EnumValues, Is.Not.Empty);
    }
    
    [Test]
    public void TestArrayElement()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.ArrayElement, Is.Null);

        typeDef.ArrayElement = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.ArrayElement, Is.Not.Null);
    }
    
    [Test]
    public void TestAllowedFilters()
    {
        var typeDef = new QueryableTypeDescriptor();
        
        Assert.That(typeDef.AllowedFilters, Is.Empty);

        typeDef.AllowedFilters = FilterDescriptor.ArrayFilters;
        
        Assert.That(typeDef.AllowedFilters, Is.Not.Null);
        Assert.That(typeDef.AllowedFilters, Is.Not.Empty);
        Assert.That(typeDef.AllowedFilters, Has.Length.EqualTo(FilterDescriptor.ArrayFilters.Length));
    }
    
    [Test]
    public void TestToString()
    {
        var typeDef = new QueryableTypeDescriptor();
        var stringRep = typeDef.ToString();
        Assert.That(stringRep, Does.Contain(typeDef.TypeName));
    }
}
