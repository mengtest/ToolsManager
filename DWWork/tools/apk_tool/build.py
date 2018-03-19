import os
import shutil

# 1 ipay 2 kappu 3 yybws 4 yybby
type=4
fileName=""
signApk=""

def DelMeta() :
    os.rename(fileName + ".apk", fileName + ".zip")
    eZip = "\"C:\Program Files (x86)\HaoZip\HaoZipC.exe\" x " + fileName + ".zip -oe:\\" + fileName  
    os.system(eZip)
    os.rename(fileName + ".zip", fileName + ".apk")
    shutil.rmtree("e:/" + fileName + "/META-INF") 
    aZip="\"C:\Program Files (x86)\HaoZip\HaoZipC.exe\" a -tzip " + fileName + "temp.zip " + "e:\\" + fileName + "\\*" 
    os.system(aZip)
    shutil.rmtree("e:/" + fileName)
    try:
        os.remove(fileName + "_unsigned.apk")
    except WindowsError:
        pass
    os.rename(fileName + "temp.zip", fileName + "_unsigned.apk")

def PackApk(index) :
    os.rename(fileName + "_unsigned.apk", fileName + "_unsigned.zip")
    ttFile=open("assets/ttChannel.bytes", 'w')
    ttFile.write(str(index))
    ttFile.close()
    updateZip="\"C:\Program Files (x86)\HaoZip\HaoZipC.exe\" u " + fileName + "_unsigned.zip assets\\"
    os.system(updateZip)
    os.rename(fileName + "_unsigned.zip", fileName + "_unsigned.apk") 
    os.system(signApk)
    os.rename(fileName + "_signed.apk", str(index) + ".apk")


if type == 1 :
    fileName="DFWS_IPay"
    DelMeta()
    signApk="tool\signeIpay.bat"
    for i in range(2001, 2040+1):
        PackApk(i)
    for i in range(2061, 2065+1):
        PackApk(i)
elif type == 2 :
    fileName="DFWS_kaopu"
    DelMeta()
    signApk="tool\signeCps.bat"
    for i in range(2079, 2088+1):
        PackApk(i)
elif type == 3 :
    fileName="DFWS_yingyongbao"
    signApk="tool\signedYybWs.bat"
    DelMeta()
    for i in range(2041, 2045+1):
        PackApk(i)
elif type == 4 :
    fileName="DFBY_YingYongBao"
    signApk="tool\signeYybBy.bat"
    DelMeta()
    for i in range(2122, 2124+1):
        PackApk(i)