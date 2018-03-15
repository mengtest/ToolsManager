//
//  IAPLib
//
//
//  Created by JanusLau
//
//

#import <Foundation/Foundation.h>
#import <AdSupport/ASIdentifierManager.h>
#include <string>

extern UIViewController *UnityGetGLViewController();

extern "C" {
    
    // Helper method used to convert NSStrings into C-style strings.
    NSString *CreateNSString(const char* string) {
        if (string) {
            return [NSString stringWithUTF8String:string];
        } else {
            return [NSString stringWithUTF8String:""];
        }
    }
    
    char* GetIDFA()
    {
        static std::string str = "";
        if ([[UIDevice currentDevice].systemVersion floatValue] >= 6.0) {
            NSString* adid = [[[ASIdentifierManager sharedManager] advertisingIdentifier] UUIDString];
            str = [adid cStringUsingEncoding: NSASCIIStringEncoding];
        }
        
        char* res = (char*)malloc(strlen(str.c_str()) + 1);
        strcpy(res, str.c_str());
        return res;
    }

    char* GetIDFV()
    {
        std::string str = "";
        NSString *idfv = [[[UIDevice currentDevice] identifierForVendor] UUIDString];
        str = [idfv cStringUsingEncoding: NSASCIIStringEncoding];
        char* res = (char*)malloc(strlen(str.c_str()) + 1);
        strcpy(res, str.c_str());
        return res;
    }
    long GetStorageFreeSize()
    {
        /// Ê£Óà´óÐ¡
        long freesize = -1;
        /// ÊÇ·ñµÇÂ¼
        NSError *error = nil;
        NSArray *paths = NSSearchPathForDirectoriesInDomains(NSDocumentDirectory, NSUserDomainMask, YES);
        NSDictionary *dictionary = [[NSFileManager defaultManager] attributesOfFileSystemForPath:[paths lastObject] error: &error];
        if (dictionary)
        {
            NSNumber *_free = [dictionary objectForKey:NSFileSystemFreeSize];
            freesize = [_free unsignedLongLongValue];
        }
        return freesize;
    }
}
