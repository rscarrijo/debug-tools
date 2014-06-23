using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class CLogHandler
    {
        static bool isAlreadyLoaded;
        private static CUDPLogger oUDPLogger;

        internal static CUDPLogger objUDPLogger
        {
            get
            {
                return oUDPLogger;
            }
        }

        static bool isTraceEnable = true;

        public enum enumAction
        { 
            @IN,
            @OUT
        }

        internal class CUDPLogger
        {
            private System.Net.Sockets.UdpClient oUDPClient;
            private System.Net.IPEndPoint oIPEndPoint;

            public CUDPLogger()
            {
                try
                {
                    oUDPClient = new System.Net.Sockets.UdpClient();
                    oUDPClient.Client.SendBufferSize = Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='UDPBufferSize']"));
                    oIPEndPoint = new System.Net.IPEndPoint(
                        System.Net.IPAddress.Parse(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='MulticastIPAddress']")),
                        Convert.ToInt32(CDLLReadSetting.ReadSetting("//configuration/appSettings/add[@key='MulticastPort']")));
                }
                catch (Exception ex)
                {
                    System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                    CommonLib.CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);                
                }
            }


            public void SendHash(Hashtable oHash)
            {
                try
                {
                    System.IO.MemoryStream oMemoryStream = new System.IO.MemoryStream();
                    System.Runtime.Serialization.Formatters.Binary.BinaryFormatter oBinary = new System.Runtime.Serialization.Formatters.Binary.BinaryFormatter();
                    oBinary.Serialize(oMemoryStream, oHash);
                    oUDPClient.Send(oMemoryStream.GetBuffer(), oMemoryStream.Capacity, oIPEndPoint);

                }
                catch (Exception ex)
                {
                    System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                    CommonLib.CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                }
            
            }

            ~CUDPLogger()
            {
                oUDPClient = null;
            }
        }


        public static void LoadLogDefinitions()
        {       
           try
            {
               if (!isAlreadyLoaded)
               {
                   isAlreadyLoaded = true;
                   oUDPLogger = new CUDPLogger();
               }
               else
               {
               
               }
               return;

            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                CommonLib.CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                return;
            }
        }


        public static void LogTrace(System.Reflection.MethodBase oMethod, string Message)
        {
            try
            {                
                LoadLogDefinitions();

                if (isTraceEnable)
                {
                    Hashtable oHashtable = new Hashtable();

                    oHashtable["DATETIME"] = DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString().PadLeft(3,'0');
                    oHashtable["MESSAGE"] = Message;
                    oHashtable["TYPE"] = "TRACE";
                    oHashtable["METHOD"] = oMethod.Name;
                    oHashtable["MODULE"] = oMethod.DeclaringType.FullName;                    
                    oHashtable["THREADID"] = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    oHashtable["PROCID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
                    oHashtable["PROCNAME"] = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;

                    string LogMethod = String.Format("{0}, MESSAGE={1}, TYPE={2}, METHOD={3}(), MODULE={4}, THREADID={5}, PROCID={6}, PROCNAME{7}",
                        oHashtable["DATETIME"].ToString(), oHashtable["MESSAGE"],
                        oHashtable["TYPE"], oHashtable["METHOD"], oHashtable["MODULE"],
                        oHashtable["THREADID"], oHashtable["PROCID"], oHashtable["PROCNAME"]);

                    oUDPLogger.SendHash(oHashtable);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

        }
    }


}
