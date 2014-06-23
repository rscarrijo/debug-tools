using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class CErrorHandler
    {
        public enum enumErrorType
        { 
            @Log,
            @Elevate
        }

        public static void LogError(ref System.Reflection.MethodBase oMethod,
                                    Exception oException, enumErrorType eErrorType)
        {
                Hashtable oHashtable = new Hashtable();

                    oHashtable["DATETIME"] = DateTime.Now.ToString() + "." + DateTime.Now.Millisecond.ToString().PadLeft(3, '0');
                    oHashtable["MESSAGE"] = oException.Message;
                    oHashtable["TYPE"] = "ERROR";
                    oHashtable["METHOD"] = oMethod.Name;
                    oHashtable["MODULE"] = oMethod.DeclaringType.FullName;                    
                    oHashtable["THREADID"] = System.Threading.Thread.CurrentThread.ManagedThreadId;
                    oHashtable["PROCID"] = System.Diagnostics.Process.GetCurrentProcess().Id;
                    oHashtable["PROCNAME"] = System.Diagnostics.Process.GetCurrentProcess().MainModule.ModuleName;

                    string LogMethod = String.Format("{0}, MESSAGE={1}, TYPE={2}, METHOD={3}(), MODULE={4}, THREADID={5}, PROCID={6}, PROCNAME{7}",
                        oHashtable["DATETIME"].ToString(), oHashtable["MESSAGE"],
                        oHashtable["TYPE"], oHashtable["METHOD"], oHashtable["MODULE"],
                        oHashtable["THREADID"], oHashtable["PROCID"], oHashtable["PROCNAME"]);

                CLogHandler.objUDPLogger.SendHash(oHashtable);

                if (eErrorType == enumErrorType.Elevate)
                {
                    throw oException;
                }
         }
    }
}
