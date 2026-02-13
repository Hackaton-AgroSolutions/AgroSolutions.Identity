using System.ComponentModel;
using System.Reflection;

namespace AgroSolutions.Identity.Infrastructure.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum @enum)
    {
        FieldInfo? field = @enum.GetType().GetField(@enum.ToString()!);
        DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
        return attribute?.Description ?? @enum.ToString();
    }
}
