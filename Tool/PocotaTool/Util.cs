using System.Reflection;
using System.Text;
using static Net.Leksi.Pocota.Tool.Constants;

namespace Net.Leksi.Pocota.Tool;

internal class Util
{
    internal static string BuildTypeName(Type type)
    {
        if (type == typeof(void))
        {
            return s_void;
        }
        if (!type.IsGenericType)
        {
            return type.Name;
        }
        if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return BuildTypeName(type.GetGenericArguments()[0]);
        }
        return type.GetGenericTypeDefinition().Name.Substring(0, type.GetGenericTypeDefinition().Name.IndexOf('`'))
            + '<' + String.Join(',', type.GetGenericArguments().Select(v => BuildTypeName(v))) + '>';
    }
    internal static string BuildTypeFullName(Type type)
    {
        if (!string.IsNullOrEmpty(type.Namespace))
        {
            return $"{type.Namespace}.{Util.BuildTypeName(type)}";
        }
        return Util.BuildTypeName(type);
    }
    internal static string GetTypeName(string fullName)
    {
        if (fullName.LastIndexOf('.') is int index && index >= 0)
        {
            return fullName[(index + 1)..];
        }
        return fullName;
    }
    internal static string GetNamespace(string fullName)
    {
        if (fullName.LastIndexOf('.') is int index && index >= 0)
        {
            return fullName[0..index];
        }
        return string.Empty;
    }
    internal static string BuildAttribute(Attribute attribute, HashSet<Type> usings)
    {
        StringBuilder sb = new();

        sb.Append('[').Append(BuildTypeName(attribute.GetType()));
        sb.Remove(sb.Length - "Attribute".Length, "Attribute".Length);
        bool hasParens = false;
        ConstructorInfo? usedConstructor = null;
        Dictionary<string,object?> constructedProperties = [];
        foreach(var constr in attribute.GetType().GetConstructors())
        {
            if(constr.GetParameters().Length == 0)
            {
                if(usedConstructor is null)
                {
                    usedConstructor = constr;
                }
            }
            else
            {
                bool canUseConstructor = true;
                Dictionary<string, object?> supposedProperties = [];
                foreach (ParameterInfo par in constr.GetParameters())
                {
                    if (
                        attribute.GetType().GetProperties()
                            .Where(p => string.Equals(p.Name, par.Name, StringComparison.OrdinalIgnoreCase)).FirstOrDefault() is PropertyInfo supposedProperty
                        && !supposedProperty.CanWrite
                    )
                    {
                        supposedProperties.Add(supposedProperty.Name.ToLower(), supposedProperty.GetValue(attribute));
                    }
                    else
                    {
                        canUseConstructor = false;
                        break;
                    }
                }
                if(canUseConstructor)
                {
                    usedConstructor = constr;
                    constructedProperties.Clear();
                    foreach(KeyValuePair<string, object?> item in supposedProperties)
                    {
                        constructedProperties.Add(item.Key, item.Value);
                    }
                }
            }
        }
        object defaultObject = Activator.CreateInstance(attribute.GetType())!;
        if (usedConstructor is { })
        {
            List<object?> parameters = [];
            foreach (ParameterInfo par in usedConstructor.GetParameters())
            {
                if (!hasParens)
                {
                    hasParens = true;
                    sb.Append('(');
                }
                else
                {
                    sb.Append(',');
                }
                sb.Append(constructedProperties[par.Name!.ToLower()]);
                parameters.Add(constructedProperties[par.Name!.ToLower()]);
                if(constructedProperties[par.Name!.ToLower()] is object value)
                {
                    usings.Add(value.GetType());
                }
            }
            defaultObject = usedConstructor.Invoke([.. parameters]);
        }
        else
        {
            defaultObject = Activator.CreateInstance(attribute.GetType())!;
        }
        foreach (PropertyInfo pi in attribute.GetType().GetProperties())
        {
            if (pi.Name != "TypeId" && !constructedProperties.ContainsKey(pi.Name.ToLower()) && pi.CanWrite && pi.GetValue(attribute) != pi.GetValue(defaultObject))
            {
                string value = RenderValue(pi, attribute);
                if(!hasParens)
                {
                    hasParens = true;
                    sb.Append('(');
                }
                else
                {
                    sb.Append(',');
                }
                sb.Append(pi.Name).Append('=').Append(value);
                usings.Add(pi.PropertyType);
            }
        }
        if (hasParens)
        {
            sb.Append(')');
        }
        sb.Append(']');
        return sb.ToString();
    }

    private static string RenderValue(PropertyInfo pi, object target)
    {
        if (pi.PropertyType == typeof(bool))
        {
            if (pi.GetValue(target) is bool b)
            {
                return b ? "true" : "false";
            }
        }
        else if (pi.PropertyType == typeof(string))
        {
            if (pi.GetValue(target) is string s)
            {
                return $"""{s}""";
            }
        }
        else if (pi.PropertyType.IsEnum)
        {
            if (pi.GetValue(target) is object obj)
            {
                return $"{pi.PropertyType.Name}.{obj}";
            }
        }
        else if(pi.GetValue(target) is object obj)
        {
            return obj.ToString()!;
        }
        return "null";
    }
}
