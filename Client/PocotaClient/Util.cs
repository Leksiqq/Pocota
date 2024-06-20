namespace Net.Leksi.Pocota.Client;

public static class Util
{
    private const string s_void = "void";
    public static string BuildTypeName(Type type)
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
        return string.Concat(
            type.GetGenericTypeDefinition().Name.AsSpan(0, type.GetGenericTypeDefinition().Name.IndexOf('`')),
            "<",
            String.Join(',', type.GetGenericArguments().Select(v => BuildTypeName(v))),
            ">"
        );
    }
    public static string BuildTypeFullName(Type type)
    {
        if (type == typeof(void))
        {
            return s_void;
        }
        if (!type.IsGenericType)
        {
            return type.FullName!;
        }
        if (type.GetGenericTypeDefinition() == typeof(Nullable<>))
        {
            return BuildTypeName(type.GetGenericArguments()[0]);
        }
        return string.Concat(
            type.GetGenericTypeDefinition().FullName.AsSpan(0, type.GetGenericTypeDefinition().FullName!.IndexOf('`')),
            "<",
            String.Join(',', type.GetGenericArguments().Select(v => BuildTypeFullName(v))),
            ">"
        );
    }
}
