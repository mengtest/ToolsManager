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
    NSString *IAP_CreateNSString(const char* string) {
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
		NSString* pid_ns = IAP_CreateNSString(pid);
		NSString* tid_ns = IAP_CreateNSString(tid);
        [sharedMgr PurchaseSucceedWithPid:pid_ns withTid:tid_ns];
    }
   
    void BuyIAP(const char * id)
    {
        NSLog(@"here");
        if (sharedMgr == nil) {
            sharedMgr = [[InAppPurchaseManager alloc]init];
        }
        NSString* id_ns = IAP_CreateNSString(id);
        [sharedMgr BuyIapWithID:id_ns];
        NSLog(@"beginBuy,id is %s",[id_ns UTF8String]);
    }
    
    void InitUnlegalCountry(const char* list)
    {
        if(sharedMgr == nil)
        {
            sharedMgr = [[InAppPurchaseManager alloc] init];
        }
        
        NSString* list_ns = IAP_CreateNSString(list);
        NSArray* arr = [list_ns componentsSeparatedByString:@"&"];
        
        [sharedMgr InitUnlegalCountry:arr]; 
    }
	
	void Restore()
	{
		[sharedMgr restorePurchas];
	}
}
