﻿using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace BLAZAM.Common.Helpers
{
    public class NetworkTools
    {

        public static bool PingHost(string hostNameOrAddress)
        {
            bool pingable = false;
            Ping pinger = new Ping();
            try
            {
                PingReply reply = pinger.Send(hostNameOrAddress, 1000, new byte[32]);
                pingable = reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                // Ignore exception and return false
            }
            return pingable;
        }

        public static bool IsPortOpen(string hostNameOrAddress, int port)
        {
            return IsAnyPortOpen(hostNameOrAddress,new int[] {port});
        }
        public static bool IsAnyPortOpen(string hostNameOrAddress, int[] ports)
        {
            bool portOpen = false;

            foreach (int port in ports)
            {
                using (TcpClient client = new TcpClient())
                {
                    try
                    {
                        client.Connect(hostNameOrAddress, port);
                        portOpen = true;
                        break;
                    }
                    catch (SocketException)
                    {
                        // Ignore exception and try the next port or return false
                    }
                    finally
                    {
                        client.Close();
                    }
                }
            }

            return portOpen;
        }
    }
}
