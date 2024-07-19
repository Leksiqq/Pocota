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
    while (socket.Accept() is Socket accept)
    {
        byte[] buffer = new byte[1024];
        int len;
        MemoryStream ms = new();
        while ((len = accept.Receive(buffer)) > 0)
        {
            for (int i = 0; i < len; ++i)
            {
                if (buffer[i] == 0)
                {
                    Console.Write("\u00b1[1J");
                    Console.SetCursorPosition(0, 0);
                    Console.Write(Encoding.UTF8.GetString(ms.ToArray(), 0, (int)ms.Position));
                    ms.Position = 0;
                }
                else
                {
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
    Console.WriteLine("Press any key to close.");
    Console.ReadKey();
}

void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    File.Delete(socketPath);
}

