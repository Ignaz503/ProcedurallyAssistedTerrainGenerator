import socket
import threading
import struct
import enum
import json
#import bpy

#TODO add host as $$ and port $$ or ?? change them in file
#before copy
HOST = '127.0.0.1'
PORT = 9999

recievedMsgQueue = [] 
recivedLock = threading.Lock()

sendMsgQueue = []
sendLock = threading.Lock()

#class TCPClient(bpy.types.Addon):
class TCPClient():
    threads = []
    canceled = False

    socket = None
    dataJson = ""
    hasConnection = False

    def sendMsg(self,sock:socket.socket,msg) -> None:
        #add msg leng up front
        msg = struct.pack('<I',len(msg))+msg
        sock.sendall(msg)

    def recvMsg(self,sock: socket.socket) -> str:
        #get msg leng form first 4 byte
        totalMsgLength = self.recvAll(sock,4)
        if not totalMsgLength:
            return None
        msgLen = struct.unpack('<I',totalMsgLength)[0]
        rec = self.recvAll(sock,msgLen)
        if not rec is None:
            return rec.decode('utf8')
        else:
            return None

    def recvAll(self,sock: socket.socket,n: int) -> bytearray:
        data = bytearray()
        while len(data) < n:
            pkg = sock.recv(n - len(data))
            if not pkg:
                return None
            data.extend(pkg)
        return data

    def connectToServer(self) -> None:
        self.socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        try:
            self.socket.connect((HOST,PORT))
            self.hasConnection = True
            return
        except:
            self.hasConnection = False
            return

    def TrySendMsg(self) -> None:
        msg = None
        sendLock.acquire()
        try:
            msg = sendMsgQueue.pop(0)
        except:
            msg = None
        finally:
            sendLock.release()
        if not msg is None:
            self.sendMsg(self.socket,msg.Serialize())
        return

    @classmethod
    def clientLoop(self,context) -> None:
        self.connectToServer(self)
        if(not self.hasConnection):
            self.endThread(self)
            return
        while((not self.canceled) and self.hasConnection):
            #try send msg
            self.TrySendMsg(self)
            # //check if recieve msg
            msg = self.recvMsg(self,self.socket)
            if not msg is None:
                self.handleMsg(self,msg)
        #end
        self.endThread(self)
        return
        

    def handleMsg(self,msg: str) -> None:
        #make to TCP msg
        tcpMsg = TCPMessage.Deserialize(msg)
        # push into revievedMsgqueue
        recivedLock.acquire()
        try:
            recievedMsgQueue.append(tcpMsg)
        except:
            pass
        finally:
            recivedLock.release()
        return

    def endThread(self) -> None:
        self.threads.remove(threading.current_thread)

    @classmethod        
    def execute(self,context):
        self.threads =[threading.Thread(name="Client",target=self.clientLoop)for i in range(1)]
        for t in self.threads:
            t.start()
        return {'RUNNING_MODAL'}

    @classmethod
    def unregister(cls) -> None:
        cls.canceled = True
        for t in cls.threads:
            t.join()
        print('EXIT')

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

    def __init__(self,type: int,info: str,load: str):
        self.Type = TCPMessageType(type)
        self.Info = info
        self.PayLoad = load

    def Print(self) -> None:
        print(self.Type.name)
        print(self.Info)
        print(self.PayLoad)

    @staticmethod
    def Deserialize(jsonString: str) -> TCPMessage:
        return json.loads(jsonString,object_hook=TCPMessage.Create)

    @staticmethod
    def Create(jD: dict) -> TCPMessage:
        return TCPMessage(jD['Type'],jD['Info'],jD['PayLoad'])

    def Serialize(self) -> str:
        obj = {}
        obj['Type'] = self.Type.value
        obj['Info'] = self.Info
        obj['PayLoad'] = self.PayLoad
        return json.dumps(obj)

def msgRecievedCheck() -> float:
    recivedLock.acquire()
    msg = None
    try:
        msg = recievedMsgQueue.pop(0)
    except:
        msg = None
    finally:
        recivedLock.release()
        if not msg is None:
            handleMsg(msg)
    return .5

def handleMsg(tcpMsg: TCPMessage) -> None:
    #TODO handle revieved msg
    print(tcpMsg.Info)
    return

def EnqueueMsgToSend(tcpMessage: TCPMessage) -> None:
    sendLock.acquire()
    try:
        sendMsgQueue.append(tcpMessage)
    except:
        pass
    finally:
        sendLock.release()
    return

def SendTestMsg() -> None:
    msg = TCPMessage(TCPMessageType.Test.value,"Test","test payload")
    EnqueueMsgToSend(msg)
    return

#def register():
#    bpy.app.timers.register(msgCheck)
#    bpy.utils.register_class(TCPClient)

#def unregister():
#    bpy.utils.unregister_class(TCPClient)


def mainTesting():
    client = TCPClient()

    client.execute(None)

    #TODO fuck shit up horribly
    print('Press Any Key To Exit')

    input()

    client.unregister()

    print('We are Done, thx for your patience')




#mainTesting()



