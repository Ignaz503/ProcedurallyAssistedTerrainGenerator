using Newtonsoft.Json;
using System;
using UnityEngine;

namespace BlenderUnityCommunication
{
    public class TCPMessage
    {
        public enum MsgType
        {
            DataRequest,
            DirectoryInfo,
            Data,
            Done,
            ConnectionProbe,
            Error,
            Test
        }
        public MsgType Type;
        public string Info;
        public string PayLoad;

        public string Stringify(bool msgLengthHeader = false)
        {
            if (msgLengthHeader)
                return LengthAppendedMsg();
            return JsonConvert.SerializeObject(this);
        }

        string LengthAppendedMsg()
        {
            var msg = JsonConvert.SerializeObject(this);
            int len = msg.Length;
            UnityEngine.Debug.Log(len);

            byte[] lenByte = BitConverter.GetBytes(len);

            if (BitConverter.IsLittleEndian)
            {
                Array.Reverse(lenByte);
            }
            
            string ret = BitConverter.ToString(lenByte);

            string[] sp = ret.Split('-');
            ret = "";
            for (int i = 0; i < sp.Length; i++)
            {
                ret += $"\\x{sp[i]}";
            }

            return ret + msg;
        }

        public static TCPMessage Deserialize(string jString,bool lengthHeader = false)
        {
            if (lengthHeader)
            {
                return DeserializeLengthHeadedMsg(jString);
            }
            return JsonConvert.DeserializeObject<TCPMessage>(jString);
        }

        public void Log(ILogger logger)
        {
            logger.Log(Type);
            logger.Log(Info);
            logger.Log(PayLoad);
        }

        static  TCPMessage DeserializeLengthHeadedMsg(string jString)
        {
            var tempJString = jString.Remove(0, jString.IndexOf('{'));
            UnityEngine.Debug.Log(tempJString);
            return JsonConvert.DeserializeObject<TCPMessage>(tempJString);
        }
    }
}