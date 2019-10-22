import threading

emptyList = []
lock = threading.Lock()

def randFunc():
    lock.acquire()
    t = None
    try:
        emptyList.pop(0)
    except:
        t = None
    finally:
        print("lock reased")
        lock.release()
    return

print("start")
randFunc()
print("end")
