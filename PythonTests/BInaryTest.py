import struct

a = "hello this is my dick"
b = bytearray()

b.extend(map(ord,a))

l = len(a)

p = struct.pack('>I',l) + b

print(l)
print(p)

siz = struct.calcsize('>I')
print(siz)

v = struct.unpack('>I',p[:siz])
print(v)
print(p[siz:])


u = "\x00\x00\x00\x2C{\"Type\":6,\"Info\":\"info\",\"PayLoad\":\"payload\"}"

ub = bytearray()
ub.extend(map(ord,u))

g = struct.unpack('>I',ub[:siz])

print(g)
