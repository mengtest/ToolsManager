import sys
import json

sdk_name = sys.argv[1]
iosType = 0
	
def ezsdk_proc():
    import ezsdkplatform
    ezsdkplatform.ProcessDifPlatform(sdk_name)
        
def get_iosType():
    global iosType
    if sdk_name == "tiantuo_iOS":
        iosType = 3
        
def store_json(data):
    with open('iosType.json', 'w') as json_file:
        json_file.write(json.dumps(data))		
		
if __name__ == "__main__":
    ezsdk_proc()
    get_iosType()
	
    print("iosType:" + str(iosType))
    data = {}
    data[sdk_name] = iosType
    store_json(data)	