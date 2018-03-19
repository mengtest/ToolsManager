//
//  PlatInterface.h
//  PlatInterface
//
//  Created by leishengchao on 2017/6/26.
//  Copyright © 2017年 leishengchao. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <MapKit/MapKit.h>
#import <CoreLocation/CoreLocation.h>

@interface PlatInterface : NSObject<MKMapViewDelegate,CLLocationManagerDelegate>
@property(nonatomic,strong) CLLocationManager *locaManager;

- (NSString *)getIDFV;
- (void)checkLocationServicesAuthorizationStatus;
- (void)reportLocationServicesAuthorizationStatus:(CLAuthorizationStatus)status;
- (void)requestLocationServicesAuthorization;
-(void)getUSerLocation;
-(int) getRssi;
-(int) getNetWorkType;
-(const char*) getDeviceID;
-(float) getBattery;
-(void) CopyStr_IOS:(const char *) str;

+ (PlatInterface*) sharedInstance;
@end
