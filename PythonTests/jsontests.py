import enum
import json
import inspect

class TCPMessageType(enum.IntEnum):
    DataRequest = 0
    DirectoryInfo = 1
    Data = 2
    Done = 3
    ConnectionProbe = 4
    Error = 5
    Test = 6

class TCPMessage:
    Type = TCPMessageType.Error
    Info = ""
    PayLoad = ""

    def __init__(self,type,info,load):
        self.Type = TCPMessageType(type)
        self.Info = info
        self.PayLoad = load

    def Print(self):
        print(self.Type.name)
        print(self.Info)
        print(self.PayLoad)

    @staticmethod
    def Deserialize(jsonString):
        return json.loads(jsonString,object_hook=TCPMessage.Create)

    @staticmethod
    def Create(jD):
        return TCPMessage(jD['Type'],jD['Info'],jD['PayLoad'])

    def Serialize(self):
        obj = {}
        obj['Type'] = self.Type.value
        obj['Info'] = self.Info
        obj['PayLoad'] = self.PayLoad
        return json.dumps(obj)

data = '{"Type":6,"Info":"info","PayLoad":"payload"}'

print("Start NEW")

msg = TCPMessage.Deserialize(data)
msg.Print()

print(msg.Serialize())