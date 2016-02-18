using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using FP.Radius;

namespace auth
{
    class Program
    {
        static int Main()
        {

            var username = Environment.GetEnvironmentVariable("username");
            var password = Environment.GetEnvironmentVariable("password");

            if (String.IsNullOrEmpty(username) || String.IsNullOrEmpty(password))
            {
                Console.WriteLine("environment variables username or password undefined");
                return 1;
            }

            if(Config.Settings == null)
            {
                Console.WriteLine("Config is empty/unreadable");
                return 1;
            }

            if (Config.Settings.Servers == null || Config.Settings.Servers.Count == 0)
            {
                Console.WriteLine("No servers found in config");
                return 1;
            }

            var res = Parallel.ForEach(Config.Settings.Servers.Cast<ServerElement>(), (server, state) =>
            {
                    // Console.WriteLine("server.name = {0}, sharedsecret={1}, retries={2}, wait={3}, authport={4}", server.Name, server.sharedsecret, server.retries, server.wait, server.authport);

                    var rc = new RadiusClient(server.Name, server.sharedsecret, server.wait * 1000, server.authport);
                    try
                    {
                        var authPacket = rc.Authenticate(username, password);
                        if (Config.Settings.NAS_IDENTIFIER != null)
                            authPacket.SetAttribute(new RadiusAttribute(RadiusAttributeType.NAS_IDENTIFIER, Encoding.ASCII.GetBytes(Config.Settings.NAS_IDENTIFIER)));

                        authPacket.SetAttribute(new RadiusAttribute(RadiusAttributeType.NAS_PORT_TYPE, BitConverter.GetBytes((int)NasPortType.ASYNC)));

                        var receivedPacket = rc.SendAndReceivePacket(authPacket, server.retries).Result;

                        if (receivedPacket != null && receivedPacket.PacketType == RadiusCode.ACCESS_ACCEPT)
                            state.Stop();

                    }catch(Exception){}
                          
            });

            if (res.IsCompleted) 
            { 
                Console.WriteLine("Auth failed for: '{0}'", username);
                return 1;
            }
            else 
            {
                Console.WriteLine("Auth Ok");
                return 0;
            }

        }
    }
}
