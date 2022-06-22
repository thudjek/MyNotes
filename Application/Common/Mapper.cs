using System.Reflection;

namespace Application.Common;
public static class Mapper
{
    public static TDestination MapTo<TDestination>(object source)
    {
        var result = (TDestination)Activator.CreateInstance(typeof(TDestination));

        var sourceProperties = source.GetType().GetProperties().ToList();
        var destinationProperties = result.GetType().GetProperties().ToList();

        foreach (var sProp in sourceProperties)
        {
            var dProp = destinationProperties.FirstOrDefault(d => d.Name == sProp.Name);

            if (dProp != null && (AreSameType(sProp, dProp) || AreEnumAndInt(sProp, dProp)))
            {
                dProp.SetValue(result, sProp.GetValue(source));
            }
        }

        return result;
    }

    private static bool AreSameType(PropertyInfo first, PropertyInfo second)
    {
        return first.PropertyType.FullName == second.PropertyType.FullName && 
               first.PropertyType.BaseType.Name == second.PropertyType.BaseType.Name;
    }

    private static bool AreEnumAndInt(PropertyInfo first, PropertyInfo second)
    {
        return (first.PropertyType.BaseType.Name == "Enum" && second.PropertyType.Name.Contains("Int")) || 
               (second.PropertyType.BaseType.Name == "Enum" && first.PropertyType.Name.Contains("Int"));
    }
}