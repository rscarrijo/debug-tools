using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Reflection;

namespace CommonLib
{
    public class CDLLReadSetting
    {

        protected static XmlDocument oDllSettingReader;

        public static XmlDocument GetDLLSettingReader()
        {
            string xmlDLLConfigFile = string.Empty;

            try
            {
                if (oDllSettingReader == null)
                {
                    xmlDLLConfigFile = new StringBuilder(CCommon.GetApplicationPath()).Append("CommonLib.config").ToString();
                    oDllSettingReader = new XmlDocument();
                    oDllSettingReader.Load(xmlDLLConfigFile);
                }

                return oDllSettingReader;

            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                CommonLib.CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                return null;
            }
            
        }

        public static string ReadSetting(string xPath)
        {
            XmlDocument oDllSettingReader = GetDLLSettingReader();
            try
            {
                XmlNode xmlNode = oDllSettingReader.SelectSingleNode(xPath);
                return xmlNode.SelectSingleNode("@value").Value;

            }
            catch (Exception ex)
            {
                System.Reflection.MethodBase oMethod = System.Reflection.MethodBase.GetCurrentMethod();
                CommonLib.CErrorHandler.LogError(ref oMethod, ex, CommonLib.CErrorHandler.enumErrorType.Elevate);
                return null;
            }
        }

    }
}
