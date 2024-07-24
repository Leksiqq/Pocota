using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddCommandLine(args).Build();

if(!(configuration["socket-path"] is string socketPath && !string.IsNullOrEmpty(socketPath)))
{
    return;
}
AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
if (configuration["title"] is string s && !string.IsNullOrEmpty(s))
{
    Console.Title = s;
}
Socket socket = new(AddressFamily.Unix, SocketType.Stream, ProtocolType.IP);
socket.Bind(new UnixDomainSocketEndPoint(socketPath));
socket.Listen();
try
{
    if (socket.Accept() is Socket accept)
    {
        byte[] buffer = new byte[1024];
        int len;
        MemoryStream ms = new();
        while ((len = accept.Receive(buffer)) > 0)
        {
            int numLines = 0;
            for (int i = 0; i < len; ++i)
            {
                if (buffer[i] == 0)
                {
                    string mess = Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Position);
                    string[] lines = mess.Split('\n');
                    Console.SetCursorPosition(0, 0);
                    foreach(string line in lines)
                    {
                        Console.Write(string.Format("{0,80}\r", string.Empty));
                        Console.Write(line);
                    }
                    Console.WriteLine("Click left mouse button to pause, press ESC to continue.");
                    ms.Position = 0;
                }
                else
                {
                    if (buffer[i] == '\n') 
                    {
                        ++numLines;
                    }
                    ms.WriteByte(buffer[i]);
                }
            }
        }
        accept.Close();
    }
}
catch(Exception ex)
{
    Console.WriteLine(ex.ToString()); 
}
finally 
{
    File.Delete(socketPath);
    int top;
    (_, top) = Console.GetCursorPosition();
    Console.SetCursorPosition(0, top - 1);
    Console.WriteLine("                                                                                            \rPress any key to close.");
    Console.ReadKey();
}

void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    File.Delete(socketPath);
}

