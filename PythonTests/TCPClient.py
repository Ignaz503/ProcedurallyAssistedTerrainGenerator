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

    def sendMsg(self,sock,msg):
        #add msg leng up front
        msg = struct.pack('<I',len(msg))+msg
        sock.sendall(msg)

    def recvMsg(self,sock):
        #get msg leng form first 4 byte
        totalMsgLength = self.recvAll(sock,4)
        if not totalMsgLength:
            return None
        msgLen = struct.unpack('<I',totalMsgLength)[0]
        return self.recvAll(sock,msgLen).decode('utf8')

    def recvAll(self,sock,n):
        data = bytearray()
        while len(data) < n:
            pkg = sock.recv(n - len(data))
            if not pkg:
                return None
            data.extend(pkg)
        return data

    def connectToServer(self):
        self.socket = socket.socket(socket.AF_INET,socket.SOCK_STREAM)
        try:
            self.socket.connect((HOST,PORT))
            self.hasConnection = True
            return
        except:
            self.hasConnection = False
            return

    @classmethod
    def client(self,context):
        self.connectToServer(self)
        if(not self.hasConnection):
            self.endThread(self)
            return
        while((not self.canceled) and self.hasConnection):
            #TODO check if send msg 
            # //check if recieve msg
            msg = self.recvMsg(self,self.socket)
            self.handleMsg(self,msg)
        #end
        self.endThread(self)
        

    def handleMsg(self,msg):
        #TODO make to TCP msg
        tcpMsg = TCPMessage.Deserialize(msg)
        # push into revievedMsgqueue
        recivedLock.acquire()
        try:
            recievedMsgQueue.append(tcpMsg)
        finally:
            recivedLock.release()
        return

    def endThread(self):
        self.threads.remove(threading.current_thread)

    @classmethod        
    def execute(self,context):
        self.threads =[threading.Thread(name="Client",target=self.client)for i in range(1)]
        for t in self.threads:
            t.start()
        return {'RUNNING_MODAL'}

    @classmethod
    def unregister(cls):
        cls.canceled = True
        for t in cls.threads:
            t.join()

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

def msgRecievedCheck():
    recivedLock.acquire()
    msg = None
    try:
        msg = recievedMsgQueue.pop(0)
    finally:
        recivedLock.release()
        if not msg is None:
            handleMsg(msg)
    return .5

def handleMsg(tcpMsg):
    #TODO handle revieved msg
    return

#def register():
#    bpy.app.timers.register(msgCheck)
#    bpy.utils.register_class(TCPClient)

#def unregister():
#    bpy.utils.unregister_class(TCPClient)



