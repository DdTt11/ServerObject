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
            Console.WriteLine("Клиент соединился " 
                              + DateTime.Now 
                              + " с адреса " 
                              + IPAddress.Parse(((IPEndPoint)tcpClient.Client.RemoteEndPoint!).Address.ToString()) 
                              + ":" 
                              + ((IPEndPoint)tcpClient.Client.RemoteEndPoint).Port);
            ClientObject clientObject = new ClientObject(tcpClient);
            await Task.Run(clientObject.ProcessAsync);
        }
    }
}
class ClientObject
{
    private StreamReader Reader { get; }
    private StreamWriter Writer { get; }
    protected internal ClientObject(TcpClient tcpClient)
    {
        var stream = tcpClient.GetStream();
        Reader = new StreamReader(stream);
        Writer = new StreamWriter(stream);
    }
 
    public async Task ProcessAsync()
    {
        var th = new Thread(SendMessage);
        th.Start();
        
        while (true) 
        {
            try 
            {
                var message = await Reader.ReadLineAsync(); 
                if (message == null) continue;
                Console.WriteLine("Сервер получил " + DateTime.Now + ": " + message);
            }
            catch 
            { 
                Console.WriteLine("Клиент разорвал соединение");
                break;
            }
        }
    }
    
    private void SendMessage()
    {
        while (true)
        {
            var message = Console.ReadLine();
            Writer.WriteLine(message);
            Writer.Flush();
        }
    }
}