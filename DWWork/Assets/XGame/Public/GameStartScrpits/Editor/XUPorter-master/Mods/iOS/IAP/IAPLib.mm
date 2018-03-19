//
//  IAPLib
//
//
//  Created by JanusLau
//
//

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import "InAppPurchaseManager.h"
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
    
    //IAP
    bool CanPurchase()
    {
        return ([SKPaymentQueue canMakePayments]);
    }
    InAppPurchaseManager *sharedMgr = nil;
    
    void PurchaseSucceed(const char * pid, const char* tid)
    {
		NSString* pid_ns = CreateNSString(pid);
		NSString* tid_ns = CreateNSString(tid);
        [sharedMgr PurchaseSucceedWithPid:pid_ns withTid:tid_ns];
    }
   
    void BuyIAP(const char * id)
    {
        NSLog(@"here");
        if (sharedMgr == nil) {
            sharedMgr = [[InAppPurchaseManager alloc]init];
        }
        NSString* id_ns = CreateNSString(id);
        [sharedMgr BuyIapWithID:id_ns withSandBox:FALSE];
        NSLog(@"beginBuy,id is %s",[id_ns UTF8String]);
    }
    
    void InitUnlegalCountry(const char* list)
    {
        if(sharedMgr == nil)
        {
            sharedMgr = [[InAppPurchaseManager alloc] init];
        }
        
        NSString* list_ns = CreateNSString(list);
        NSArray* arr = [list_ns componentsSeparatedByString:@"&"];
        
        [sharedMgr InitUnlegalCountry:arr]; 
    }
	
	void Restore()
	{
		[sharedMgr restorePurchas:TRUE];
	}
    
    char* _GetIDFA()
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
    
    float GetBatteryLevel()
    {
        [[UIDevice currentDevice] setBatteryMonitoringEnabled:YES];
        return [[UIDevice currentDevice] batteryLevel];
    }

	
	char* GetIDFV()
    {
        static std::string str = "";
        if ([[UIDevice currentDevice].systemVersion floatValue] >= 6.0) {
            NSString* adid =  [[[UIDevice currentDevice] identifierForVendor] UUIDString];  
            str = [adid cStringUsingEncoding: NSASCIIStringEncoding];
        }
        
        char* res = (char*)malloc(strlen(str.c_str()) + 1);
        strcpy(res, str.c_str());
        return res;
    }
	
	void _Pay(const char * id, bool isSandbox)
	{
		NSLog(@"here");
        if (sharedMgr == nil) {
            sharedMgr = [[InAppPurchaseManager alloc]init];
        }
        NSString* id_ns = CreateNSString(id);
        [sharedMgr BuyIapWithID:id_ns withSandBox:isSandbox];
        NSLog(@"beginBuy,id is %s",[id_ns UTF8String]);
	}
    
    
	void _RestoreProductID(const char* pid)
	{
		[sharedMgr restoreProductID:CreateNSString(pid)];
	}
	
	
	void _Restore()
	{
		[sharedMgr restorePurchas:TRUE];
	}
	
	void _PayFinishSuc(const char * pid, const char* tid)
	{
		NSString* pid_ns = CreateNSString(pid);
		NSString* tid_ns = CreateNSString(tid);
        [sharedMgr PurchaseSucceedWithPid:pid_ns withTid:tid_ns];
	}
	
	void _Login(const char* login)
	{

	}
	
	void _GetGuestUUID(const char* strGuid)
	{

	}
    
    void _Logout()
	{

	}
    
    void _CreatRole(const char* strJson)
    {

    }
    
    void _EnterGame(const char* strJson)
    {

    }
    
    void _GameUpdateLevel(const char* strJson)
    {

    }
    
    void _GameUpdateRoleName(const char* strJson)
    {

    }


}
