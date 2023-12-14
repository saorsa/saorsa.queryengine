using System.Reflection;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using Saorsa.QueryEngine.Tests.EFCore.Entities;

namespace Saorsa.QueryEngine.API.Controllers;

public class GenericControllerFeatureProvider: IApplicationFeatureProvider<ControllerFeature>
{
    public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
    {
        var types = new []
        {
            typeof(User).GetTypeInfo(),
            typeof(Tag).GetTypeInfo(),
            typeof(Department).GetTypeInfo(),
        };
        // This is designed to run after the default ControllerTypeProvider, 
        // so the list of 'real' controllers has already been populated.
        foreach (var entityType in types)
        {
            var typeName = entityType.Name + "Controller";
            if (feature.Controllers.All(t => t.Name != typeName))
            {
                // There's no 'real' controller for this entity, so add the generic version.
                var controllerType = typeof(GenericController<>)
                    .MakeGenericType(entityType.AsType()).GetTypeInfo();
                feature.Controllers.Add(controllerType);
            }
        }
    }
}
