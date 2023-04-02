using System.Net;
using System.Net.Sockets;
 
ServerObject server = new ServerObject();
Console.WriteLine("Сервер включён " + DateTime.Now);
await server.ListenAsync();

class ServerObject
{
    readonly TcpListener _tcpListener = new(IPAddress.Loopback, 8888);
    protected internal async Task ListenAsync()
    { 
        _tcpListener.Start();
        while (true)
        { 
            TcpClient tcpClient = await _tcpListener.AcceptTcpClientAsync(); 
            Console.WriteLine("Клиент соединился " + DateTime.Now + " с адреса " + IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString()) + ":" + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port);
            ClientObject clientObject = new ClientObject(tcpClient); 
            await Task.Run(clientObject.ProcessAsync);
        }
    }
}
class ClientObject
{
    private StreamReader Reader { get; }
    public ClientObject(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        Reader = new StreamReader(stream);
    }
 
    public async Task ProcessAsync()
    {
        while (true) 
        {
            try 
            {
                string? message = await Reader.ReadLineAsync(); 
                if (message == null) continue;
                Console.WriteLine("Сервер получил " + DateTime.Now + " " + message);
            }
            catch 
            { 
                Console.WriteLine("Клиент разорвал соединение"); 
                break;
            }
        }
    }
    
    public async Task SendMessageAsync(StreamWriter streamWriter)
    {
        while (true)
        {
            string? message = Console.ReadLine();
            await streamWriter.WriteLineAsync(message);
            await streamWriter.FlushAsync();
        }
    }
}