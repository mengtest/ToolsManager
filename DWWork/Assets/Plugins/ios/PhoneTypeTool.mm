
#import <sys/utsname.h>
#import <Foundation/Foundation.h>

extern "C" {
	
	char* MakeStringCopy (const char* string)
    {
        
        if (string == NULL)
            return NULL;
        
        char* res = (char*)malloc(strlen(string) + 1);
        
        strcpy(res, string);
        
        return res;
    }
	
	char* GetIphoneType()
	{

		struct utsname systemInfo;

		uname(&systemInfo);

		NSString *platform = [NSString stringWithCString:systemInfo.machine encoding:NSASCIIStringEncoding];
		
		NSString *platformName = platform;

		if ([platform isEqualToString:@"iPhone1,1"])
			platformName = @"iPhone_2G";

		else if ([platform isEqualToString:@"iPhone1,2"])
			platformName = @"iPhone_3G";

		else if ([platform isEqualToString:@"iPhone2,1"])
			platformName = @"iPhone_3GS";

		else if ([platform isEqualToString:@"iPhone3,1"])
			platformName = @"iPhone_4";

		else if ([platform isEqualToString:@"iPhone3,2"])
			platformName =  @"iPhone_4";

		else if ([platform isEqualToString:@"iPhone3,3"])
			platformName =  @"iPhone_4";

		else if ([platform isEqualToString:@"iPhone4,1"])
			platformName =  @"iPhone_4S";

		else if ([platform isEqualToString:@"iPhone5,1"])
			platformName =  @"iPhone_5";

		else if ([platform isEqualToString:@"iPhone5,2"])
			platformName =  @"iPhone_5";

		else if ([platform isEqualToString:@"iPhone5,3"])
			platformName =  @"iPhone_5c";

		else if ([platform isEqualToString:@"iPhone5,4"])
			platformName =  @"iPhone_5c";

		else if ([platform isEqualToString:@"iPhone6,1"])
			platformName =  @"iPhone_5s";

		else if ([platform isEqualToString:@"iPhone6,2"])
			platformName =  @"iPhone_5s";

		else if ([platform isEqualToString:@"iPhone7,1"])
			platformName =  @"iPhone_6Plus";

		else if ([platform isEqualToString:@"iPhone7,2"])
			platformName =  @"iPhone_6";

		else if ([platform isEqualToString:@"iPhone8,1"])
			platformName =  @"iPhone_6s";

		else if ([platform isEqualToString:@"iPhone8,2"])
			platformName =  @"iPhone_6sPlus";

		else if ([platform isEqualToString:@"iPhone8,4"])
			platformName =  @"iPhone_SE";

		else if ([platform isEqualToString:@"iPhone9,1"])
			platformName =  @"iPhone_7";

		else if ([platform isEqualToString:@"iPhone9,2"])
			platformName =  @"iPhone_7_Plus";

		else if ([platform isEqualToString:@"iPod1,1"])
			platformName =  @"iPod_Touch_1G";

		else if ([platform isEqualToString:@"iPod2,1"])
			 platformName =  @"iPod_Touch_2G";

		else if ([platform isEqualToString:@"iPod3,1"]) 
			platformName =  @"iPod_Touch_3G";

		else if ([platform isEqualToString:@"iPod4,1"])
			platformName =  @"iPod_Touch_4G";

		else if ([platform isEqualToString:@"iPod5,1"])
			platformName =  @"iPod_Touch_5G";

		else if ([platform isEqualToString:@"iPad1,1"])
			platformName =  @"iPad_1G";

		else if ([platform isEqualToString:@"iPad2,1"])
			platformName =  @"iPad_2";

		else if ([platform isEqualToString:@"iPad2,2"])
			platformName =  @"iPad_2";

		else if ([platform isEqualToString:@"iPad2,3"])
			platformName =  @"iPad_2";

		else if ([platform isEqualToString:@"iPad2,4"])
			platformName =  @"iPad_2";

		else if ([platform isEqualToString:@"iPad2,5"])
			platformName =  @"iPad_Mini_1G";

		else if ([platform isEqualToString:@"iPad2,6"])
			platformName =  @"iPad_Mini_1G";

		else if ([platform isEqualToString:@"iPad2,7"])
			platformName =  @"iPad_Mini_1G";

		else if ([platform isEqualToString:@"iPad3,1"])
			platformName =  @"iPad_3";

		else if ([platform isEqualToString:@"iPad3,2"])
			platformName =  @"iPad_3";

		else if ([platform isEqualToString:@"iPad3,3"])
			platformName =  @"iPad_3";

		else if ([platform isEqualToString:@"iPad3,4"])
			platformName =  @"iPad_4";

		else if ([platform isEqualToString:@"iPad3,5"])
			platformName =  @"iPad_4";

		else if ([platform isEqualToString:@"iPad3,6"])
			platformName =  @"iPad_4";

		else if ([platform isEqualToString:@"iPad4,1"])
			platformName =  @"iPad_Air";

		else if ([platform isEqualToString:@"iPad4,2"])
			platformName =  @"iPad_Air";

		else if ([platform isEqualToString:@"iPad4,3"])
			platformName =  @"iPad_Air";

		else if ([platform isEqualToString:@"iPad4,4"])
			platformName =  @"iPad_Mini_2G";

		else if ([platform isEqualToString:@"iPad4,5"])
			platformName =  @"iPad_Mini_2G";

		else if ([platform isEqualToString:@"iPad4,6"])
			platformName =  @"iPad_Mini_2G";

		else if ([platform isEqualToString:@"i386"])
			platformName = @"iPhone_Simulator";
		
		else if ([platform isEqualToString:@"x86_64"])
			platformName = @"iPhone_Simulator";

		return MakeStringCopy([platformName UTF8String]);

	}
}