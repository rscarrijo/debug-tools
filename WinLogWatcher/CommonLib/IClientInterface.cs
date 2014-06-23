using System;
using System.Collections.Generic;
using System.Text;

namespace CommonLib
{
    public enum enumClientState
    {
        @Disconnected,
        @Connected,
        @Connecting,
        @Disconnecting,
        @Busy,
        @Unknown
    }

    public interface IClientInterface
    {
        void Initialize();
        void Terminate();

        void Config(string XmlConfig);

        void Connect();
        void Disconnect();

        enumClientState CurrentState();

    }
}
