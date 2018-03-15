//
//  AdTrackingPlugin
//
//
//  Created by JanusLau
//
//AdTrackingPlugin.h

#import <Foundation/Foundation.h>
#import <StoreKit/StoreKit.h>
#import "TalkingDataAppCpa.h"

extern "C" {
    
    // Helper method used to convert NSStrings into C-style strings.
    NSString *ADCreateNSString(const char* string) {
        if (string) {
            return [NSString stringWithUTF8String:string];
        } else {
            return [NSString stringWithUTF8String:""];
        }
    }
	
	//AdTrackingPlugin
	void _Init(const char* appId, const char* channelID)
	{
		NSString* ns_appID = ADCreateNSString(appId);
		NSString* ns_channelID = ADCreateNSString(channelID);
		
		[TalkingDataAppCpa init:ns_appID withChannelId:ns_channelID];
	}
	
	void _onRegister(const char* account)
	{
		NSString* ns_account = ADCreateNSString(account);
		
		[TalkingDataAppCpa onRegister:ns_account];
	}
	
	void _onLogin(const char* account)
	{
		NSString* ns_account = ADCreateNSString(account);
		
		[TalkingDataAppCpa onLogin:ns_account];
	}
	
	void _onCreateRole(const char* rID)
	{
		NSString* ns_rID = ADCreateNSString(rID);
		
		[TalkingDataAppCpa onCreateRole:ns_rID];
	}
	
	void _onPay(const char* UID, const char* orderID, int price, const char* currency, const char* payType)
	{
		NSString* NSUID = ADCreateNSString(UID);
		NSString* NSorderID = ADCreateNSString(orderID);
		NSString* NScurrency = ADCreateNSString(currency);
		NSString* NSpayType = ADCreateNSString(payType);
		
		[TalkingDataAppCpa onPay:NSUID withOrderId:NSorderID withAmount:price withCurrencyType:NScurrency withPayType:NSpayType];
	}
	
	void _onOrderPaySucc(const char* UID, const char* orderID, int price, const char* currency, const char* payType)
	{
		NSString* NSUID = ADCreateNSString(UID);
		NSString* NSorderID = ADCreateNSString(orderID);
		NSString* NScurrency = ADCreateNSString(currency);
		NSString* NSpayType = ADCreateNSString(payType);
		
        [TalkingDataAppCpa onOrderPaySucc:NSUID withOrderId: NSorderID withAmount: price withCurrencyType: NScurrency withPayType: NSpayType];
	}
	
	void _onCustEvent(int type)
	{
		switch(type)
		{
		case 1:
			[TalkingDataAppCpa onCustEvent1];
			break;
		case 2:
			[TalkingDataAppCpa onCustEvent2];
			break;
		case 3:
			[TalkingDataAppCpa onCustEvent3];
			break;
		case 4:
			[TalkingDataAppCpa onCustEvent4];
			break;
		case 5:
			[TalkingDataAppCpa onCustEvent5];
			break;
		case 6:
			[TalkingDataAppCpa onCustEvent6];
			break;
		case 7:
			[TalkingDataAppCpa onCustEvent7];
			break;
		case 8:
			[TalkingDataAppCpa onCustEvent8];
			break;
		case 9:
			[TalkingDataAppCpa onCustEvent9];
			break;
		case 10:
			[TalkingDataAppCpa onCustEvent10];
			break;
		}		
	} 
}
