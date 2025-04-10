from ac_interface import AssettoCorsaData
import socket

rpm = 0
MAX_RPM = 1
assettoReader = AssettoCorsaData()
assettoReader.start()
data = assettoReader.getData()

data = assettoReader.getData()

# Connect to the server
client_socket = socket.socket(socket.AF_INET, socket.SOCK_STREAM)
client_socket.connect(('localhost', 12345))  # Server address and port

while True:
    data = assettoReader.getData()
    rpm = int(data['rpm'])
    print(rpm)
    if(rpm > MAX_RPM):
        MAX_RPM = rpm
    socketData = f"{rpm},{MAX_RPM}"
    client_socket.sendall(socketData.encode())
    
