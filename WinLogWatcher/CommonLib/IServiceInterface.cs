using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public enum enumServiceState
    { 
        @Stopped,
        @Running,
        @Starting,
        @Stopping,
        @Busy,
        @Unknown
    }
    public interface IServiceInterface
    {
        void Initialize();
        void Terminate();

        void Config(string XmlConfig);

        void StartService();
        void StopService();

        enumServiceState CurrentState();

    }
}
