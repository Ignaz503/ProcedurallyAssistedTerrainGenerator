using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;
using UnityEditor.Formats.Fbx.Exporter;
using Newtonsoft.Json;
using UnityEditor;
public class BlenderUnityCommunicator
{
    protected class Message
    {
        //TODO: inform unity:
        //eg message error -> don't reload meshes or anything else from main thread
        //eg message done -> mainthread we can reload our meshes
    }

    static BlenderUnityCommunicator instance;
    public static BlenderUnityCommunicator Instance
    {
        get
        {
            if (instance == null)
            {
                CreateInstance(TCPCommandServer.DefaultPort);
            }
            return instance;
        }
    }

    public static void CreateInstance(int port)
    {
        instance = new BlenderUnityCommunicator(port);
    }

    public string PathToBlender;

    int port;

    TCPCommandServer server;

    CancellationTokenSource cts;

    Queue<Message> messageQueue;

    private BlenderUnityCommunicator(int port)
    {
        this.port = port;
        messageQueue = new Queue<Message>();
    }

    void EditorUpdate()
    {
        if (messageQueue.Count != 0)
        {
            HandleMessages();
        }
    }

    private void HandleMessages()
    {
        Message m;
        for (int i = 0; i < messageQueue.Count; i++)
        {
            lock (messageQueue)
            {
                m = messageQueue.Dequeue();
            }
            HandleMessage(m);
        }
    }

    private void HandleMessage(Message m)
    {
        throw new NotImplementedException();
    }

    private void MakeUseableInEditor()
    {
        EditorApplication.update -= EditorUpdate;
        EditorApplication.update += EditorUpdate;

        EditorApplication.quitting -= OnQuit;
        EditorApplication.quitting += OnQuit;
    }

    void OnQuit()
    {
        EditorApplication.update -= EditorUpdate;
        //TODO inform tcp thread of quit?
        if (cts != null)
            cts.Cancel();
    }

    public void ExportChunksForEdit(GameObject[] toEdit, string filePath)
    {
        for (int i = 0; i < toEdit.Length; i++)
        {
            ModelExporter.ExportObject(filePath + $"/chunk{i}.fbx", toEdit[i]);
        }
    }

    public void StartBlender()
    {
        Process.Start(PathToBlender);
    }

    public void StartTCPServer()
    {
        cts = new CancellationTokenSource();
        //TODO start server and so on
    }
}

public class TCPCommandServerNotRunningException : Exception
{
    public TCPCommandServerNotRunningException(): base("Server Not Running, open server before use")
    {}
}

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

    public bool ServerIsRunning { get { return serverIsStarted && ContinueListenLoop; } }

    protected abstract bool ContinueListenLoop { get; }

    public TCPCommandServer()
    {
        serverIsStarted = false;
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

    protected void TCPLoop(object cancelToken)
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
                        try
                        {
                            SendConnectionProbeMessage(stream);
                        }
                        catch (ObjectDisposedException e)
                        {
                            //client closed
                            UnityEngine.Debug.Log(e);
                            break;
                        }
                        client.Close();
                        break;
                    }
                    else
                        break;
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
                    try
                    {
                        SendConnectionProbeMessage(stream);
                    }
                    catch (ObjectDisposedException e)
                    {
                        //Connection closed
                        UnityEngine.Debug.Log(e);
                        break;
                    }

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
                    //canceld/unity cloed whilst reading
                    try
                    {
                        SendUnityClosedMessage(stream);
                    }
                    catch (ObjectDisposedException e)
                    {
                        //client was already closed
                        UnityEngine.Debug.Log(e);
                        break;
                    }
                    client.Close();
                    break;
                }

                data = dataBuilder.ToString();
                var procedure  = HandleTCPMessage(data, client, stream);
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
        }

        server.Stop();
        OnEndServer();
    }

    public abstract void OnEndServer();

    protected void SendUnityClosedMessage(NetworkStream stream)
    {
        //inform
        TCPMessage msg = new TCPMessage() { Type = TCPMessage.MsgType.Error, Info = "UnityClosed", JsonLoad = "" };

        SendMessage(msg,stream);
    }

    protected void SendConnectionProbeMessage(NetworkStream stream)
    {
        SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.ConnectionProbe, Info = "ConnectionOpenCheck", JsonLoad = "" }, stream);
    }

    protected void SendMessage(TCPMessage msg, NetworkStream stream)
    {
        byte[] msgBuffer = Encoding.ASCII.GetBytes(msg.Stringify());
        stream.Write(msgBuffer, 0, msgBuffer.Length);
    }

    protected abstract AfterMsgHandleProcedure HandleTCPMessage(string data, TcpClient client, NetworkStream clientStream);

    public void ForceAnyConnectionClosed()
    {
        throw new NotImplementedException();
    }

}

public class BlenderUnityTCPCommandServer : TCPCommandServer
{
   //TODO msg handling //success handling etc


    protected override bool ContinueListenLoop => throw new NotImplementedException();

    public override void OnEndServer()
    {
        throw new NotImplementedException();
    }

    protected override AfterMsgHandleProcedure HandleTCPMessage(string data, TcpClient client, NetworkStream clientStream)
    {
        throw new NotImplementedException();
    }
}

public class TCPMessage
{
    public enum MsgType
    {
        Okay,
        DirectoryInfo,
        PositionInfo,
        Done,
        ConnectionProbe,
        Error
    }
    public MsgType Type;
    public string Info;
    public string JsonLoad;

    public string Stringify()
    {
        return JsonConvert.SerializeObject(this);
    }

    public static TCPMessage Deserialize(string jString)
    {
        return JsonConvert.DeserializeObject<TCPMessage>(jString);
    }
}