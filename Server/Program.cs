using System.Net;
using System.Text;
using ServerCore;

namespace Server;

class Program
{
    static Listener _listener = new Listener();

    static void Main(string[] args)
    {
        //DNS
        string host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ipAddress = ipHost.AddressList[0];
        var endPoint = new IPEndPoint(ipAddress, 7777);

        //소켓
        _listener.Init(endPoint, () => new ClientSession());
        Console.WriteLine("Listening...");
        
        while (true)
        {
        }
    }
}