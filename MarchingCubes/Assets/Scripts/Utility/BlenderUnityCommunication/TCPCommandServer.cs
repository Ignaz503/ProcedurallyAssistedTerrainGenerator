using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace BlenderUnityCommunication
{
    public abstract class TCPCommandServer
    {
        public enum AfterMsgHandleProcedure
        {
            KeepConnection,
            CloseConnection,
        }

        public const int DefaultPort = 9999;

        protected TcpListener server;
        protected CancellationToken token;
        protected bool serverIsStarted;

        bool forceCloseAnyConnection;

        public bool ServerIsRunning { get { return serverIsStarted && ContinueListenLoop; } }

        protected abstract bool ContinueListenLoop { get; }

        public TCPCommandServer()
        {
            serverIsStarted = false;
            forceCloseAnyConnection = false;
        }

        public void SetCancelationToke(CancellationToken t)
        {
            token = t;
        }

        public void OpenServer(string ipAddress = "", int port = DefaultPort)
        {
            IPAddress address = IPAddress.Parse("127.0.0.1");
            server = new TcpListener(address, port);
            server.Start();
            serverIsStarted = true;
        }

        public void TCPLoop()
        {
            if (!serverIsStarted)
                throw new TCPCommandServerNotRunningException();
            //TCP listen Thread
            try
            {
                byte[] buffer = new byte[1024];
                string data = null;
                TcpClient client = null;
                NetworkStream stream = null;

                while (ContinueListenLoop)
                {
                    //canel token check 
                    if (token.IsCancellationRequested)
                    {
                        //cleanup
                        if (client != null)
                        {
                            if (IsClientConnectionOpen(stream))
                            {
                                SendUnityClosedMessage(stream);
                                client.Close();
                            }
                            break;
                        }
                        else
                            break;
                    }

                    //when we have client
                    if (forceCloseAnyConnection)
                    {
                        if (client != null)
                        {
                            //do things
                            if (IsClientConnectionOpen(stream))
                            {
                                client.Close();
                            }
                        }
                        //OnForceClose
                        forceCloseAnyConnection = false;
                        client = null;
                        stream = null;
                        OnConnectionForcedClose();
                    }

                    if (client == null)
                    {
                        client = server.AcceptTcpClient();
                        UnityEngine.Debug.Log("Client Connected");
                        stream = client.GetStream();
                    }
                    else
                    {
                        if (stream == null)
                            stream = client.GetStream();
                        //try send and see if fail to check if closed
                        if (!IsClientConnectionOpen(stream))
                            break;

                    }

                    //after client connected check if cancel request
                    if (token.IsCancellationRequested)
                    {
                        try
                        {
                            SendUnityClosedMessage(stream);
                        }
                        catch (ObjectDisposedException e)
                        {
                            //client already closed
                            UnityEngine.Debug.Log(e);
                            break;
                        }
                        //close client
                        client.Close();
                        break;

                    }

                    StringBuilder dataBuilder = new StringBuilder();
                    int numBytesRead = 0;
                    bool canceld = false;

                    if (stream.CanRead)
                    {
                        do
                        {
                            //whilst reading
                            if (token.IsCancellationRequested)
                            {
                                canceld = true;
                                break;
                            }

                            numBytesRead = stream.Read(buffer, 0, buffer.Length);
                            dataBuilder.AppendFormat("{0}", Encoding.ASCII.GetString(buffer, 0, numBytesRead));

                        } while (stream.DataAvailable);
                    }

                    if (canceld)
                    {
                        //IsOpenCHeck TODO
                        if (IsClientConnectionOpen(stream))
                        {
                            SendUnityClosedMessage(stream);
                            client.Close();
                        }
                        break;
                    }

                    data = dataBuilder.ToString();
                    var procedure = HandleTCPMessage(data, client, stream);
                    switch (procedure)
                    {
                        case AfterMsgHandleProcedure.CloseConnection:
                            client.Close();
                            client = null;
                            break;
                        case AfterMsgHandleProcedure.KeepConnection:
                            //Noting
                            break;
                    }
                }
            }
            catch (SocketException e)
            {
                UnityEngine.Debug.Log(e.Message);
            }
            finally
            {
                server.Stop();
                OnEndServer();
            }

            server.Stop();
            OnEndServer();
        }

        public abstract void OnEndServer();

        protected void SendUnityClosedMessage(NetworkStream stream)
        {
            //inform
            TCPMessage msg = new TCPMessage() { Type = TCPMessage.MsgType.Error, Info = "UnityClosed", PayLoad = "" };

            SendMessage(msg, stream);
        }

        protected void SendConnectionProbeMessage(NetworkStream stream)
        {
            SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.ConnectionProbe, Info = "ConnectionOpenCheck", PayLoad = "" }, stream);
        }

        protected void SendMessage(TCPMessage msg, NetworkStream stream)
        {
            byte[] msgBuffer = Encoding.ASCII.GetBytes(msg.Stringify());
            stream.Write(msgBuffer, 0, msgBuffer.Length);
        }

        protected abstract AfterMsgHandleProcedure HandleTCPMessage(string data, TcpClient client, NetworkStream clientStream);

        protected bool IsClientConnectionOpen(NetworkStream clientStream)
        {
            //canceld/unity cloed whilst reading
            try
            {
                SendConnectionProbeMessage(clientStream);
            }
            catch (ObjectDisposedException e)
            {
                //client was already closed
                UnityEngine.Debug.Log(e);
                return false;
            }
            return true;
        }

        public void ForceAnyConnectionClosed()
        {
            forceCloseAnyConnection = true;
        }

        protected void SendDoneMessage(NetworkStream stream)
        {
            SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.Done, Info = "Done", PayLoad = "" },stream);
        }

        protected abstract void OnConnectionForcedClose();

    }
}