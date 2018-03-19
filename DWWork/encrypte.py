import os
import sys
import google
import zlib
from Crypto.Cipher import AES
import binascii
import struct
import shutil

PRIVATE_KEY="dd7fd4a156d28bade96f816db1d18609".decode("hex")
IV="dd7fd4a156d28bade96f816db1d18609".decode("hex")

def encrypte(data,slen) :
    aes = AES.new(PRIVATE_KEY, AES.MODE_CBC, IV)
    x = len(data) % 16
    if x != 0:
        data = data + '0'*(16 - x)
    edata = aes.encrypt(data)
    print("______________________slen:" + str(slen) + "  dLen:" + str(len(data)))
    return struct.pack("!l",slen) + struct.pack("!l", len(data)) + edata
    #return data

	
xgame_file = open('./Assets/StreamingAssets/xgame.ezfun', "rb")	
try:
	source_data = xgame_file.read()
finally:
	xgame_file.close();
	
zpkg = zlib.compress(source_data)
epkg = encrypte(zpkg, len(source_data))
f = open('./Assets/StreamingAssets/xgame.ezfun', "wb")
f.write(epkg)
f.close()