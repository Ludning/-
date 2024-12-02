using System.Net;
using System.Net.Sockets;
using System.Text;
using ServerCore;

namespace DummyClient;

class Program
{
    static void Main(string[] args)
    {
        //DNS
        string host = Dns.GetHostName();
        var ipHost = Dns.GetHostEntry(host);
        var ipAddress = ipHost.AddressList[0];
        var endPoint = new IPEndPoint(ipAddress, 7777);

        Connector connector = new Connector();
        
        connector.Connect(endPoint, () => new ServerSession());
        
        while (true)
        {
            try
            {
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
            
            Thread.Sleep(100);
        }
    }
}