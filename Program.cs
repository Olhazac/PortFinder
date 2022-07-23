using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Port_Checker
{
    class IpPing
    {
        public IpPing(bool piga, PingReply rep, int port)
        {
            pingable = piga;
            reply = rep;
            portt = port;
        }
        public bool pingable;
        public PingReply reply;
        public int portt;

    }

    class Program
    {
        static List<String> ips;

        public static IpPing PingHost(string nameOrAddress, int portCheck)
        {
            bool pingable = false;
            Ping pinger = null;
            int port = portCheck;
            PingReply reply = null;
            TcpClient tcpScanner = new TcpClient();
            tcpScanner.SendTimeout = 3000;
            tcpScanner.ReceiveTimeout = 3000;

            try
            {
                pinger = new Ping();
                reply = pinger.Send(nameOrAddress);
                if(reply.Status == IPStatus.Success)
				{
                    tcpScanner.Connect(IPAddress.Parse(nameOrAddress), port);
                    pingable = true;  
				}          
            }
            catch (PingException)
            {
                return new IpPing(false, reply, port);
            }
            catch (SocketException)
			{
                return new IpPing(false, reply, port);
            }
            finally
            {
                if (pinger != null)
                {
                    pinger.Dispose();
                }
            }

            return new IpPing(pingable, reply, port);
        }

        static void Main(string[] args)
        {
            Console.Clear();
            Console.WriteLine("Start? Type 'y' for yes 'n' for no:");
            string confirm = Console.ReadLine();
            if(confirm != "y")
			{
                return;
			}
            string routerIP = "192.168.1.";
            List<IpPing> ping = new List<IpPing>();
            List<int> ports = new List<int> { 80, 443, 21, 22, 110, 995, 143, 993, 25, 26, 587, 3306, 2082, 2083, 2086, 2087, 2095, 2096, 2077, 2078 };
            IpPing pong;
            for (int u = 0; u < 20; u++)
            {
                for (int i = 0; i < 255; i++)
                {
                    pong = PingHost(routerIP + i, ports[u]);

                    if (pong.pingable)
                    {
                        try
                        {
                            Console.Clear();
                            Console.Write("Searching IP and Port {" + i * u + "/5100} Device Name: " + Dns.GetHostEntry(pong.reply.Address.ToString()).HostName.ToString() + "Port: " + ports[u]);
                            Console.Write("\r");
                        }
                        catch (SocketException)
                        {
                            Console.Clear();
                            Console.Write("Searching IP {" + i * u + "/5100} Device Name: " + pong.reply.Address.ToString() + "Port: " + ports[u]);
                            Console.Write("\r");
                        }
                        ping.Add(pong);
                    }
                    else
                    {
                        Console.Clear();
                        Console.Write("Searching IP {" + i * u + "/5100}");
                        Console.Write("\r");
                    }
                }
            }
            
         
            using (StreamWriter writer = new StreamWriter("./ips.txt"))
            {
				for (int i = 0; i < ping.Count; i++)
				{
                    writer.WriteLine(ping[i].reply.Address.ToString() + ":" + ping[i].portt);
                }            
            }

        }
    }
}
