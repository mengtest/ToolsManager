#import <StoreKit/StoreKit.h>
#define kInAppPurchaseManagerProductsFetchedNotification @"kInAppPurchaseManagerProductsFetchedNotification"
#define kInAppPurchaseManagerTransactionFailedNotification @"kInAppPurchaseManagerTransactionFailedNotification"
#define kInAppPurchaseManagerTransactionSucceededNotification @"kInAppPurchaseManagerTransactionSucceededNotification"

@interface InAppPurchaseManager : NSObject <SKProductsRequestDelegate, SKPaymentTransactionObserver>
{
    SKProduct *proUpgradeProduct;
    SKProductsRequest *productsRequest;
    NSMutableArray* m_transactionLs;
	NSMutableArray* m_unlegalCountryArray;
	BOOL m_isSandbox;
}

- (BOOL)canMakePurchases;
- (void)BuyIapWithID:(NSString *) m_id withSandBox:(BOOL) is_sandbox;
- (void)PurchaseSucceedWithPid:(NSString *) pid withTid:(NSString *) tid;
- (void)restorePurchas:(BOOL) is_sandbox;
- (void)restoreProductID:(NSString *) pid;
- (void)paymentQueueRestoreCompletedTransactionsFinished:(SKPaymentQueue *)queue;
- (void) paymentQueue:(SKPaymentQueue *)queue restoreCompletedTransactionsFailedWithError:(NSError *)error;
- (void)productsRequest:(SKProductsRequest *)request didReceiveResponse:(SKProductsResponse *)response;
- (void)request:(SKRequest *)request didFailWithError:(NSError *)error;
-(void) PurchasedTransaction: (SKPaymentTransaction *)transaction;
- (void)paymentQueue:(SKPaymentQueue *)queue updatedTransactions:(NSArray *)transactions;
- (void)completeTransaction:(SKPaymentTransaction *)transaction;
- (NSString *)encode:(const uint8_t *)input length:(NSInteger)length;
- (void)failedTransaction:(SKPaymentTransaction *)transaction;
- (void)InitUnlegalCountry:(NSArray *)list;


@end