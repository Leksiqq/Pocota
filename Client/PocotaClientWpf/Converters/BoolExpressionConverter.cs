using System.Globalization;
using System.Net.NetworkInformation;
using System.Windows.Data;

namespace Net.Leksi.Pocota.Client;

public class BoolExpressionConverter : IMultiValueConverter
{
    public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
    {
        if(parameter is string expression)
        {
            return Evaluate(values.Select(v => v is bool b && b).ToArray(), expression);  
        }
        if(values.Length == 1)
        {
            return values[0] is bool b && b;
        }
        return false;
    }
    public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
    private bool Evaluate(bool[] values, string expression)
    {
        Stack<bool> operands = [];
        Stack<char> operations = [];
        int pos = -1;

        foreach (char ch in expression)
        {
            switch(ch)
            {
                case '!' or '(':
                    operations.Push(ch);
                    break;
                case '|':
                    while (operations.TryPeek(out char next) && next == ch)
                    {
                        Apply(operands, next);
                        operations.Pop();
                    }
                    operations.Push(ch);
                    break;
                case '&':
                    while (operations.TryPeek(out char next) && (next == '|' || next == ch))
                    {
                        Apply(operands, next);
                        operations.Pop();
                    }
                    operations.Push(ch);
                    break;
                case ')':
                    while (operations.TryPeek(out char next) && next != '(')
                    {
                        Apply(operands, next);
                        operations.Pop();
                    }
                    if (!operations.TryPop(out char par) || par != '(')
                    {
                        throw new Exception();
                    }
                    while(operations.TryPeek(out char next) && next == '!')
                    {
                        Apply(operands, next);
                        operations.Pop();
                    }
                    break;
                case '@':
                    pos = 0;
                    break;
                case '0' or '1' or '2' or '3' or '4' or '5' or '6' or '7' or '8' or '9':
                    if(pos < 0)
                    {
                        throw new Exception();
                    }
                    pos *= 10;
                    pos += ch - '0';
                    break;
                case ' ' or '\t' or '\n' or '\r':
                    if(pos >= 0)
                    {
                        operands.Push(values[pos]);
                        pos = -1;
                    }
                    break;
            }
        }
        return operands.Pop();
    }

    private void Apply(Stack<bool> operands, char next)
    {
        bool arg1;
        bool arg2;
        switch (next)
        {
            case '!':
                arg1 = PopOrThrow(operands);
                operands.Push(!arg1);
                break;
            case '|':
                arg2 = PopOrThrow(operands);
                arg1 = PopOrThrow(operands);
                operands.Push(arg2 || arg1);
                break;
            case '&':
                arg2 = PopOrThrow(operands);
                arg1 = PopOrThrow(operands);
                operands.Push(arg2 && arg1);
                break;
        }
    }

    private bool PopOrThrow(Stack<bool> operands)
    {
        if(operands.TryPop(out bool b))
        {
            return b;
        }
        throw new Exception();
    }
}
