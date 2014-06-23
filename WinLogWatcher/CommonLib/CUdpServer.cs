using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Net;

using System.Text;

namespace CommonLib
{
    public class CUdpServer
    {
        class UdpState
        {
            public UdpClient u;
            public IPEndPoint e;
        }

        public static void receiveCallback(IAsyncResult ar)
        {
            UdpClient u = (UdpClient)((UdpState)(ar.AsyncState)).u;
            IPEndPoint e = (IPEndPoint)((UdpState)(ar.AsyncState)).e;

            Byte[] receiveBytes = u.EndReceive(ar, ref e);
            string receiveString = Encoding.ASCII.GetString(receiveBytes);

            System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
            CommonLib.CLogHandler.LogTrace(oMethod, receiveString);

            UdpState s = new UdpState();
            s.e = e;
            s.u = u;
            u.BeginReceive(new AsyncCallback(receiveCallback), s);
        }

        public static void receiveMessages()
        {
            // Receive a message and write it to the console.
            IPEndPoint e = new System.Net.IPEndPoint(
                System.Net.IPAddress.Parse(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='UDPServerIPAddress']")),
                Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='UDPServerPort']")));            
            
            UdpClient u = new UdpClient(e);

            UdpState s = new UdpState();
            s.e = e;
            s.u = u;
            u.BeginReceive(new AsyncCallback(receiveCallback), s);

            while (true)
            { }

        }
    }
}




