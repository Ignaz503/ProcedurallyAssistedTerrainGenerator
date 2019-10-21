import socket
import threading
import struct
#import bpy

#TODO add host as $$ and port $$ or ?? change them in file
#before copy
HOST = '127.0.0.1'
PORT = 9999

#class TCPClient(bpy.types.Addon):
class TCPClient():
    threads = []
    canceled = False

    recievedMSGQueue = []

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
            #TODO
            msg = self.recvMsg(self,self.socket)
            self.handleMsg(self,msg)
        #end
        self.endThread(self)
        

    def handleMsg(self,msg):
        #TODO
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


#ef register():
#    bpy.utils.register_class(TCPClient)

#def unregister():
#    bpy.utils.unregister_class(TCPClient)

