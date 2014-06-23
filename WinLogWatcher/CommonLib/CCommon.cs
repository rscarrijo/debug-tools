using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public class CCommon
    {
        public static string GetTempPath()
        {
            try
            {
                if (AppDomain.CurrentDomain.ShadowCopyFiles)
                {
                    return AppDomain.CurrentDomain.DynamicDirectory;
                }
                else
                {
                    return System.IO.Path.GetTempPath();
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                return null;            
            }
        }


        public static string GetApplicationPath()
        {
            try
            {
                if (AppDomain.CurrentDomain.ShadowCopyFiles)
                {
                    return AppDomain.CurrentDomain.RelativeSearchPath;
                }
                else
                {
                    return AppDomain.CurrentDomain.BaseDirectory;
                }
            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                return null;
            }
        }
    }

}
