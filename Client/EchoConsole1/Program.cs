using Microsoft.Extensions.Configuration;
using System.Net.Sockets;
using System.Text;

IConfiguration configuration = new ConfigurationBuilder()
    .AddCommandLine(args).Build();

if(!(configuration["socket-path"] is string socketPath && !string.IsNullOrEmpty(socketPath)))
{
    Console.ReadKey();
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
while(socket.Accept() is Socket accept)
{
    byte[] buffer = new byte[1024];
    int len;
    while((len = accept.Receive(buffer)) > 0)
    {
        Console.Write(Encoding.UTF8.GetString(buffer, 0, len));
    }
    accept.Close();
    break;
}
File.Delete(socketPath);
Console.WriteLine("Press any key to close.");
Console.ReadKey();

void CurrentDomain_ProcessExit(object? sender, EventArgs e)
{
    File.Delete(socketPath);
}

