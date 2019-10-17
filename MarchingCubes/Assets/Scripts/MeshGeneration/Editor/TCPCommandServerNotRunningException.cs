using System;

namespace BlenderUnityCommunication
{
    public class TCPCommandServerNotRunningException : Exception
    {
        public TCPCommandServerNotRunningException() : base("Server Not Running, open server before use")
        { }
    }
}