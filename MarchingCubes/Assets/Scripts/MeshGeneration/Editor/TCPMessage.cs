using Newtonsoft.Json;

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

        public string Stringify()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static TCPMessage Deserialize(string jString)
        {
            return JsonConvert.DeserializeObject<TCPMessage>(jString);
        }
    }
}