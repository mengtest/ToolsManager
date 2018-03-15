
#include "InAppPurchaseManager.h"

@implementation InAppPurchaseManager

- (id)init
{
    self = [super init];
	m_transactionLs =[[NSMutableArray alloc] init];
    m_unlegalCountryArray = [[NSMutableArray alloc] init];
    
    if(self)
    {
        //NSLog(@"init selef");
    }
    
    return self;
}

#pragma -
#pragma Public method
- (void)InitUnlegalCountry:(NSArray *)list
{
    for(NSString *item in list)
    {
        if([m_unlegalCountryArray containsObject:item])
        {
            continue;
        }
        
        [m_unlegalCountryArray addObject:item];
    }
}

- (void) restorePurchas:(BOOL) is_sandbox
{
	m_isSandbox = is_sandbox;
	UnitySendMessage("EzFunSDKRoot", "OCLog", "restorePurchas");
    [[SKPaymentQueue defaultQueue] restoreCompletedTransactions];
}

- (void) restoreProductID:(NSString *) p_id
{
}

- (void) paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue
{
    UnitySendMessage("EzFunSDKRoot", "OCLog", "restorePurchas Success");
    [self paymentQueue:[SKPaymentQueue defaultQueue] updatedTransactions:queue.transactions];
}

- (void) paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error
{
	UnitySendMessage("EzFunSDKRoot", "OCLog", "restorePurchas Fail");
}

#pragma mark -
#pragma mark SKProductsRequestDelegate methods

//# get good list success

- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response
{
    NSLog(@"request response");
    [[SKPaymentQueue defaultQueue] addTransactionObserver:self];
	if (response.products == nil || response.products.count == 0) {
        UnitySendMessage("EzFunSDKRoot", "PayCallBack", "0");
    }
	else
	{
        //for(NSString * item in m_unlegalCountryArray)
        //{
        //    NSLog(@"unlegal item:%@", item);
        //}
        
        for(SKProduct *item in response.products)
		{
            NSArray * array = [item.priceLocale.localeIdentifier componentsSeparatedByString:@"="];
            //for(NSString* str in array)
            //{
                //NSLog(@"array str:%@", str);
            //}
            if(array != nil && array.count >= 2)
            {
                NSString *country = [array lastObject];
                //NSLog(@"local id : %@", country);
                if([m_unlegalCountryArray containsObject: country])
                {
                    UnitySendMessage("EzFunSDKRoot", "PayCallBack","0");
                    break;
                }
            }
            
			SKPayment *payment = [SKPayment paymentWithProduct: item];
			[[SKPaymentQueue defaultQueue] addPayment:payment];
			
			break;
		}	
    }
}

//# get good list fail

 - (void)request:(SKRequest *)request didFailWithError:(NSError *)error
 {
     NSString *errorStr; 
    if (error != nil) {
        errorStr = error.description;
    } else {
        errorStr = @"purchaseFailed";
    }
    UnitySendMessage("EzFunSDKRoot", "PayCallBack", "0");
 }

 
//
// call this before making a purchase
//
- (BOOL)canMakePurchases
{
    return [SKPaymentQueue canMakePayments];
}
//
// kick off the upgrade transaction
//
- (void)BuyIapWithID:(NSString *) m_id withSandBox:(BOOL) is_sandbox;
{
	[[SKPaymentQueue defaultQueue] addTransactionObserver:self];	
	NSSet *productIdentifiers = [NSSet setWithObject:m_id];
    productsRequest = [[SKProductsRequest alloc] initWithProductIdentifiers:productIdentifiers];
    productsRequest.delegate = self;
	m_isSandbox = is_sandbox;
    [productsRequest start];
    //NSLog(@"buy id : %@", m_id);
}

-(void) PurchasedTransaction: (SKPaymentTransaction *)transaction
{
	NSArray *transactions =[[NSArray alloc] initWithObjects:transaction, nil];
	[self paymentQueue:[SKPaymentQueue defaultQueue] updatedTransactions:transactions];
//	[transactions release];
}

#pragma mark -
#pragma mark SKPaymentTransactionObserver methods
//
// called when the transaction status is updated
//
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions
{
	UnitySendMessage("EzFunSDKRoot", "OCLog", "paymentQueue");
    for (SKPaymentTransaction *transaction in transactions)
    {
        const char * id_str = [transaction.transactionIdentifier UTF8String];
        switch (transaction.transactionState)
        {
            case SKPaymentTransactionStatePurchased:
				UnitySendMessage("EzFunSDKRoot", "OCLog", "paymentQueue SKPaymentTransactionStatePurchased");
                [self completeTransaction:transaction];
                break;
            case SKPaymentTransactionStateFailed:
				UnitySendMessage("EzFunSDKRoot", "OCLog", "paymentQueue SKPaymentTransactionStateFailed");
                [self failedTransaction:transaction];
                break;
            case SKPaymentTransactionStateRestored:
				UnitySendMessage("EzFunSDKRoot", "RestoreCallBack", [transaction.payment.productIdentifier UTF8String]);
				//[self completeTransaction:transaction];
                break;
            default:
                break;
        }	
    }
    NSLog(@"paymentQueue");
}

- (void)PurchaseSucceedWithPid:(NSString *) pid withTid:(NSString *) tid
{
	UnitySendMessage("EzFunSDKRoot", "OCLog", "Ready PurchaseSucceedWithPid");
	for (SKPaymentTransaction *transaction in m_transactionLs)
    {  
		if([pid isEqualToString:transaction.payment.productIdentifier] && [tid isEqualToString:transaction.transactionIdentifier])
		{
			[[SKPaymentQueue defaultQueue] finishTransaction: transaction];		
			UnitySendMessage("EzFunSDKRoot", "OCLog", "PurchaseSucceedWithPid Success");		 
            [m_transactionLs removeObject:transaction];
            break;
		}	 
    }	 	
}

 - (void)completeTransaction:(SKPaymentTransaction *)transaction
 {
     NSString *jsonobjectstring =[self encode:(uint8_t *)transaction.transactionReceipt.bytes length:transaction.transactionReceipt.length];
     
     [m_transactionLs addObject:transaction];
     
     NSString* isSandBoxStr = m_isSandbox ? @"true" : @"false";
     
	 NSDictionary *jsonDic = @{
	 	 @"is_sandbox": isSandBoxStr,
		 @"data": jsonobjectstring
         };

	 NSData* data = [NSJSONSerialization dataWithJSONObject:jsonDic options:kNilOptions error:nil];

	 NSString* jsonStr = [[NSString alloc] initWithData:data encoding:NSUTF8StringEncoding];

	 NSString* res = @"1;1;";
	 res = [res stringByAppendingString:jsonStr]; 
	 res = [res stringByAppendingString:@";"];
	 res = [res stringByAppendingString:transaction.payment.productIdentifier];
	 res = [res stringByAppendingString:@";"];
	 res = [res stringByAppendingString:transaction.transactionIdentifier];
     UnitySendMessage("EzFunSDKRoot", "PayCallBack", [res UTF8String]);
 }

 - (NSString *)encode:(const uint8_t *)input length:(NSInteger)length {
     
     static char table[] = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";
     NSMutableData *data = [NSMutableData dataWithLength:((length + 2) / 3) * 4];
     uint8_t *output = (uint8_t *)data.mutableBytes;
     for (NSInteger i = 0; i < length; i += 3) {
         NSInteger value = 0;
         for (NSInteger j = i; j < (i + 3); j++) {
             value <<= 8;
             if (j < length) {
                 value |= (0xFF & input[j]);
             }
         }
         NSInteger index = (i / 3) * 4;
         output[index + 0] = table[(value >> 18) & 0x3F];
         output[index + 1] = table[(value >> 12) & 0x3F];
         output[index + 2] = (i + 1) < length ? table[(value >> 6)  & 0x3F] : '=';
         output[index + 3] = (i + 2) < length ? table[(value >> 0)  & 0x3F] : '=';
     }
     return [[NSString alloc] initWithData:data encoding:NSASCIIStringEncoding];
 }
 - (void)failedTransaction:(SKPaymentTransaction *)transaction
{
	// error info is transaction.error.code
	// SKErrorUnknown,
	// SKErrorClientInvalid,               // client is not allowed to issue the request, etc.
	// SKErrorPaymentCancelled,            // user cancelled the request, etc.
	// SKErrorPaymentInvalid,              // purchase identifier was invalid, etc.
	// SKErrorPaymentNotAllowed,           // this device is not allowed to make the payment
	// SKErrorStoreProductNotAvailable,    // Product is not available in the current storefront
	UnitySendMessage("EzFunSDKRoot", "PayCallBack", "0");
	[[SKPaymentQueue defaultQueue] finishTransaction: transaction];
}
- (void) dealloc
{
	[[SKPaymentQueue defaultQueue] removeTransactionObserver:self];
}


@end
