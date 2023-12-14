using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Saorsa.QueryEngine.API.Controllers;

public class GenericControllerNameConventionAttribute : Attribute, IControllerModelConvention
{
    public void Apply(ControllerModel controller)
    {
        if (controller.ControllerType.GetGenericTypeDefinition() != 
            typeof(GenericController<>))
        {
            // Not a GenericController, ignore.
            return;
        }

        var entityType = controller.ControllerType.GenericTypeArguments[0];
        controller.ControllerName = entityType.Name;
    }
}