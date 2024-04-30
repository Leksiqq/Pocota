using System.Collections;
using System.Text;

namespace Net.Leksi.Pocota.Contract;

public class PocotaRemoteException(string? message) : Exception(message)
{
    private const int _indention = 4;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder();

        sb.Append(GetType().FullName).Append(": ").AppendLine(Message);
        if(Data.Count > 0)
        {
            sb.AppendLine("Data: ---------- begin -------------");
            DataToString(sb, Data, 1);
            sb.AppendLine("---------- end -------------");
        }
        sb.AppendLine(StackTrace);

        return sb.ToString();
    }

    private static void DataToString(StringBuilder sb, IDictionary data, int level)
    {
        int alignment = 0;
        foreach(var key in data.Keys)
        {
            if(key.ToString()!.Length > alignment)
            {
                alignment = key.ToString()!.Length;
            }
        }
        foreach (var key in data.Keys)
        {
            string name = string.Format($"{{0,{level * _indention}}}{{1,-{alignment + _indention}}} ", "", key.ToString() + ':');
            object? value = data[key];
            if(value is null)
            {
                sb.AppendLine($"{name}null");
            }
            else if(value is IDictionary dict)
            {
                if(dict.Count > 0)
                {
                    sb.AppendLine(name);
                    DataToString(sb, dict, level + 1);
                }
            }
            else
            {
                sb.Append(name);
                foreach (string line in value.ToString()!.Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                {
                    sb.AppendFormat($"{{0,{level * (_indention + 1)}}}", "").AppendLine(line);
                }
            }
        }
    }
}
