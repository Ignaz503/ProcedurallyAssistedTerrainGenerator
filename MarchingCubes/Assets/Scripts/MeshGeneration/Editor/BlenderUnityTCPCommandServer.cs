using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Newtonsoft.Json;

namespace BlenderUnityCommunication
{
    public class BlenderUnityTCPCommandServer : TCPCommandServer
    {
        //TODO msg handling //success handling etc

        Dictionary<string, string> dataToSend;

        bool success;

        public BlenderUnityTCPCommandServer() : base()
        {
            this.success = false;
            dataToSend = new Dictionary<string, string>();
        }

        protected override bool ContinueListenLoop { get { return !success; } }

        public override void OnEndServer()
        {
            if (success)
            {
                BlenderUnityCommunicator.Instance.GiveInstruction(new BlenderUnityCommunicator.FinishedMeshEditingInstruction());
            }
        }

        public void AddDataToSend(string tag, string dataJson)
        {
            if (dataToSend.ContainsKey(tag))
            {
                dataToSend[tag] = dataJson;
            }
            else
            {
                dataToSend.Add(tag, dataJson);
            }
        }

        protected override void OnConnectionForcedClose()
        {
            BlenderUnityCommunicator.Instance.GiveInstruction(new BlenderUnityCommunicator.ConnectionForcedClosedInstruction());
        }

        protected override AfterMsgHandleProcedure HandleTCPMessage(string data, TcpClient client, NetworkStream clientStream)
        {
            var msg = JsonConvert.DeserializeObject<TCPMessage>(data);
            try
            {
                AfterMsgHandleProcedure mode;
                switch (msg.Type)
                {
                    case TCPMessage.MsgType.DataRequest:
                        mode = HandleDataRequestMessage(msg, client, clientStream);
                        break;
                    case TCPMessage.MsgType.Done:
                        mode = HandleDoneMessage(msg, client, clientStream);
                        break;
                    case TCPMessage.MsgType.Error:
                        mode = HandleErrorMessage(msg, client, clientStream);
                        break;
                    case TCPMessage.MsgType.Test:
                        mode = HandleTestMessage(msg, client, clientStream);
                        break;
                    default:
                        mode = AfterMsgHandleProcedure.KeepConnection;
                        break;
                }
                return mode;
            }
            catch (ObjectDisposedException)
            {
                return AfterMsgHandleProcedure.CloseConnection;
            }
        }

        private AfterMsgHandleProcedure HandleErrorMessage(TCPMessage msg, TcpClient client, NetworkStream clientStream)
        {
            //TODO WTF DO I WANT TO KNOW FROM CLIENT ERROR MSGS? 
            // I GUESS NOTHING JUST FUCK OF INTO THE DISTANCE HAPPY?
            throw new NotImplementedException();
        }

        private AfterMsgHandleProcedure HandleDoneMessage(TCPMessage msg, TcpClient client, NetworkStream clientStream)
        {
            success = true;
            try
            {
                SendDoneMessage(clientStream);
                return AfterMsgHandleProcedure.CloseConnection;
            }
            catch (ObjectDisposedException)
            { /*client already closed and we are done so we don't care if we can't send done back really*/
                return AfterMsgHandleProcedure.CloseConnection;
            }
        }

        private AfterMsgHandleProcedure HandleDataRequestMessage(TCPMessage msg, TcpClient client, NetworkStream clientStream)
        {
            try
            {
                if (dataToSend.ContainsKey(msg.Info))
                {
                    SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.Data, Info = msg.Info, PayLoad = dataToSend[msg.Info] }, clientStream);
                }
                else
                {
                    //we don't have any entries for that tag send error back
                    SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.Error, Info = "WrongDataTag", PayLoad = msg.Info }, clientStream);
                }
                return AfterMsgHandleProcedure.KeepConnection;
            }
            catch (ObjectDisposedException)
            {
                //client dead
                return AfterMsgHandleProcedure.CloseConnection;
            }
        }

        private AfterMsgHandleProcedure HandleTestMessage(TCPMessage msg, TcpClient client, NetworkStream clientStream)
        {
            UnityEngine.Debug.Log($"Recieved Message: {msg.Info}");

            ReplyTest(clientStream);

            return AfterMsgHandleProcedure.KeepConnection;
        }

        private void ReplyTest(NetworkStream clientStream)
        {
            SendMessage(new TCPMessage() { Type = TCPMessage.MsgType.Test, Info = "Test Recieved", PayLoad = "" }, clientStream);
        }

    }
}