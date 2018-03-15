//
//  PlatInterface.m
//  PlatInterface
//
//  Created by leishengchao on 2017/6/26.
//  Copyright © 2017年 leishengchao. All rights reserved.
//

#import "PlatInterface.h"

#import "KeyChainTool.h"
#import <UIKit/UIKit.h>
#import "UnityInterface.h"

@implementation PlatInterface
static PlatInterface *instance = nil;
+(PlatInterface *)sharedInstance{
    @synchronized(self) {
        if(instance == nil) {
            instance = [[[self class] alloc] init];
        }
    }
    return instance;
}

#pragma mark - 检查授权状态
- (void)checkLocationServicesAuthorizationStatus {

    [self reportLocationServicesAuthorizationStatus:[CLLocationManager authorizationStatus]];
}


- (void)reportLocationServicesAuthorizationStatus:(CLAuthorizationStatus)status
{
    if(status == kCLAuthorizationStatusNotDetermined)
    {
        //未决定，继续请求授权
        [self requestLocationServicesAuthorization];
    }
    else if(status == kCLAuthorizationStatusRestricted)
    {
        //受限制，尝试提示然后进入设置页面进行处理（根据API说明一般不会返回该值）
        [self alertViewWithMessage];

    }
    else if(status == kCLAuthorizationStatusDenied)
    {
        //拒绝使用，提示是否进入设置页面进行修改
        [self alertViewWithMessage];

    }
    else if(status == kCLAuthorizationStatusAuthorizedWhenInUse)
    {
        //授权使用
        [self requestLocationServicesAuthorization];
    }
    else if(status == kCLAuthorizationStatusAuthorizedAlways)
    {
       //始终使用
        [self requestLocationServicesAuthorization];
    }

}

#pragma mark - Helper methods


- (void)alertViewWithMessage {
    UIAlertView *alter = [[UIAlertView alloc] initWithTitle:@"定位服务未开启" message:@"请到“设置->隐私->定位服务”中开启定位" delegate:self cancelButtonTitle:@"暂不" otherButtonTitles:@"去设置", nil];
    [alter show];

}

- (void)alertView:(UIAlertView *)alertView clickedButtonAtIndex:(NSInteger)buttonIndex
{
    if (buttonIndex == 0)
    {

    }
    else
    {
        //进入系统设置页面，APP本身的权限管理页面
        [[UIApplication sharedApplication] openURL:[NSURL URLWithString:UIApplicationOpenSettingsURLString]];
    }
}

#pragma mark - 获取授权

- (void)requestLocationServicesAuthorization
{
    //CLLocationManager的实例对象一定要保持生命周期的存活
    if (!_locaManager) {
        _locaManager  = [[CLLocationManager alloc] init];
        _locaManager.delegate = self;
    }
    //The desired location accuracy.//精确度
    _locaManager.desiredAccuracy = kCLLocationAccuracyBest;
    //Specifies the minimum update distance in meters.
    //距离
    _locaManager.distanceFilter = 10;
    if ([[[UIDevice currentDevice] systemVersion] floatValue] >= 8.0)
    {
        [_locaManager requestWhenInUseAuthorization];
        [_locaManager requestAlwaysAuthorization];
        
    }
    else
    {
        [_locaManager requestWhenInUseAuthorization];
    }
    [_locaManager startUpdatingLocation];
}

- (void)locationManager:(CLLocationManager *)manager didUpdateToLocation:(CLLocation *)newLocation fromLocation:(CLLocation *)oldLocation{
    NSLog(@"%f,%f",newLocation.coordinate.latitude,newLocation.coordinate.longitude);
    
    NSString* location = [NSString stringWithFormat:@"%f%s%f", newLocation.coordinate.latitude, "#", newLocation.coordinate.longitude];
    
    //UnitySendMessage("Camera", "onLocationBack_IOS", [location UTF8String]);
}

//定位回调代理
- (void)locationManager:(CLLocationManager *)manager didUpdateLocations:(NSArray *)locations
{
    //for(CLLocation *location in locations){
    //    NSLog(@"---------%@-------",location);
    //}
//    CLLocation *currLocation=[locations firstObject];
    
    //NSString* userlocation = [NSString stringWithFormat:@"%f%s%f", currLocation.coordinate.latitude, "#", currLocation.coordinate.longitude];
    
    CLGeocoder *geoCoder = [[CLGeocoder alloc]init];//反向解析，根据及纬度反向解析出地址
    CLLocation *location = [locations objectAtIndex:0];
    [geoCoder reverseGeocodeLocation:location completionHandler:^(NSArray *placemarks, NSError *error) {
        
        for(CLPlacemark *place in placemarks)
        {
            //取出当前位置的坐标
            //NSLog(@"latitude : %f,longitude: %f",currLocation.coordinate.latitude,currLocation.coordinate.longitude);
            NSString *latStr = [NSString stringWithFormat:@"%f",location.coordinate.latitude];
            NSString *lngStr = [NSString stringWithFormat:@"%f",location.coordinate.longitude];
            NSDictionary *dict = [place addressDictionary];
            NSString* FormattedAddressLine = dict[@"FormattedAddressLines"][0];
            
            [dict setValue:FormattedAddressLine forKey:@"FormattedAddressLine"];
    
//            
            NSMutableDictionary *resultDic = [[NSMutableDictionary alloc] init];
            if (dict[@"SubLocality"]) {
                [resultDic setObject:dict[@"SubLocality"] forKey:@"SubLocality"];
            }
            if(dict[@"City"]){
                [resultDic setObject:dict[@"City"] forKey:@"City"];
            }
            if (latStr)
            {
                [resultDic setObject:latStr forKey:@"Latitude"];
            }
            if (lngStr)
            {
                [resultDic setObject:lngStr forKey:@"Longitude"];
            }

            if (dict[@"State"]) {
                [resultDic setObject:dict[@"State"] forKey:@"State"];
            }
            
            if (dict[@"Name"]) {
                [resultDic setObject:dict[@"Name"] forKey:@"Name"];
            }
            if (dict[@"FormattedAddressLine"])
            {
                [resultDic setObject:dict[@"FormattedAddressLine"] forKey:@"FormattedAddressLine"];
            }

            if (dict[@"CountryCode"])
            {
                [resultDic setObject:dict[@"CountryCode"] forKey:@"CountryCode"];
            }

            
            if (dict[@"Street"]) {
                [resultDic setObject:dict[@"Street"] forKey:@"Street"];
            }
            if(dict[@"Thoroughfare"])
            {
                [resultDic setObject:dict[@"Thoroughfare"] forKey:@"Thoroughfare"];
            }
            if (dict[@"SubThoroughfare"]) {
                [resultDic setObject:dict[@"SubThoroughfare"] forKey:@"SubThoroughfare"];
            }
            if (dict[@"Country"]) {
                [resultDic setObject:dict[@"Country"] forKey:@"Country"];
            }
            
//            NSLog(@"------addressDictionary-%@------",dict);
//            NSLog(@"=---------resultdict ------%@---", resultDic);
            //[[NSUserDefaults standardUserDefaults] setObject:dict[@"SubLocality"] forKey:@"XianUser"];
            //[[NSUserDefaults standardUserDefaults] setObject:resultDic forKey:@"LocationInfo"];
            //[[NSUserDefaults standardUserDefaults] synchronize];
            
            
            if([NSJSONSerialization isValidJSONObject:resultDic])
            {
                NSError* error;
                NSData *str = [NSJSONSerialization dataWithJSONObject:resultDic options:kNilOptions error:&error];
                NSLog(@"Result: %@",[[NSString alloc]initWithData:str
                                                         encoding:NSUTF8StringEncoding]);
                NSString* userlocation = [[NSString alloc]initWithData:str
                                                              encoding:NSUTF8StringEncoding];
                UnitySendMessage("PlatInterface", "onLocationBack_IOS", [userlocation UTF8String]);
                break;

            }
        }
    }];
    
    [_locaManager stopUpdatingLocation];
}

- (void)locationManager:(CLLocationManager *)manager didChangeAuthorizationStatus:(CLAuthorizationStatus)status
{
    [self reportLocationServicesAuthorizationStatus:status];
}


#pragma mark - 检测应用是否开启定位服务
- (void)locationManager:(CLLocationManager *)manager didFailWithError:(NSError *)error
{
    [manager stopUpdatingLocation];
    switch([error code]) {
        case kCLErrorDenied:
            [self openGPSTips];
            break;
        case kCLErrorLocationUnknown:
            break;
        default:
            break;
    }
}

-(void)openGPSTips{
    UIAlertView *alet = [[UIAlertView alloc] initWithTitle:@"当前定位服务不可用" message:@"请到“设置->隐私->定位服务”中开启定位" delegate:self cancelButtonTitle:nil otherButtonTitles:@"确定", nil];
    [alet show];
}
//获取定位信息
-(void)getUSerLocation
{
    bool isEnable = [CLLocationManager locationServicesEnabled];
    if (isEnable) {
        [self checkLocationServicesAuthorizationStatus];
    }
    else
    {
        [self alertViewWithMessage];
    }
}

- (NSString *)getIDFV
{
    NSString *IDFV = (NSString *)[KeyChainTool load:@"IDFV"];
    
    if ([IDFV isEqualToString:@""] || !IDFV) {
        
        IDFV = [UIDevice currentDevice].identifierForVendor.UUIDString;
        [KeyChainTool save:@"IDFV" data:IDFV];
    }
    
    return IDFV;
}

-(int) getRssi
{
    int rssi = -49;
    UIApplication *app = [UIApplication sharedApplication];
    NSArray *subviews;
    if ([[app valueForKeyPath:@"_statusBar"] isKindOfClass:NSClassFromString(@"UIStatusBar_Modern")]) {
        subviews = [[[[app valueForKeyPath:@"_statusBar"] valueForKeyPath:@"_statusBar"] valueForKeyPath:@"foregroundView"] subviews];
    } else {
        subviews = [[[app valueForKeyPath:@"_statusBar"] valueForKeyPath:@"foregroundView"] subviews];
    }
    
    NSString *dataNetworkItemView = nil;
    
    for (id subview in subviews) {
        if([subview isKindOfClass:[NSClassFromString(@"UIStatusBarDataNetworkItemView") class]]) {
            dataNetworkItemView = subview;
            rssi = [[dataNetworkItemView valueForKey:@"_wifiStrengthRaw"] intValue];
            break;
        }
    }
    
    return rssi;
}
    
-(int) getNetWorkType{
    int netType = 4;
    UIApplication *app = [UIApplication sharedApplication];
    
    NSArray *subviews;
    if ([[app valueForKeyPath:@"_statusBar"] isKindOfClass:NSClassFromString(@"UIStatusBar_Modern")]) {
        subviews = [[[[app valueForKeyPath:@"_statusBar"] valueForKeyPath:@"_statusBar"] valueForKeyPath:@"foregroundView"] subviews];
    } else {
        subviews = [[[app valueForKeyPath:@"_statusBar"] valueForKeyPath:@"foregroundView"] subviews];
    }

    NSString *dataNetworkItemView = nil;
    
    for (id subview in subviews) {
        if ([subview isKindOfClass:NSClassFromString(@"UIStatusBarDataNetworkItemView")]) {
            dataNetworkItemView = subview;
            netType = [[dataNetworkItemView valueForKeyPath:@"dataNetworkType"] intValue];
            break;
        }
    }
    
    return netType;
}


-(const char*) getDeviceID{
    NSString *deviceid;
    deviceid = [[PlatInterface sharedInstance] getIDFV];
    return [deviceid UTF8String];
}

-(float) getBattery{
    // UIApplication *app = [UIApplication sharedApplication];
    // NSArray *subviews = [[[app valueForKey:@"statusBar"] valueForKey:@"foregroundView"] subviews];
    // NSString *dataNetworkItemView = nil;
    
    // for (id subview in subviews) {
        // if([subview isKindOfClass:[NSClassFromString(@"UIStatusBarBatteryPercentItemView") class]]) {
            // dataNetworkItemView = subview;
            // break;
        // }
    // }
    //NSString* bat = [[dataNetworkItemView valueForKey:@"_percentString"] stringValue];
    [UIDevice currentDevice].batteryMonitoringEnabled = YES;
    CGFloat battery = [[UIDevice currentDevice] batteryLevel];
    return battery;
}

@end


extern "C"{

    void GetLocation_IOS(){
        NSLog(@"ios call =============GetLocation_IOS");
        [[PlatInterface sharedInstance] getUSerLocation];
    }
}

extern "C"{
    void GetRssi_IOS(){
        NSLog(@"ios call =============GetRssi_IOS");
        int rssi = [[PlatInterface sharedInstance] getRssi];
        NSString* rssi_str = [NSString stringWithFormat:@"%d", rssi];
        NSLog(@"rssi ios%@ ",rssi_str);
        UnitySendMessage("PlatInterface", "onRssiBack", [rssi_str UTF8String]);
    }
}

extern "C"{
    void GetDeviceID_IOS(){
        NSLog(@"ios call =============GetDevice_IOS");
        UnitySendMessage("PlatInterface", "onDeviceIDBack", [[PlatInterface sharedInstance] getDeviceID]);
        
    }
}

extern "C"{
    void GetBattery_IOS(){
        NSLog(@"ios call =============GetBattery_IOS");
        float battery =[[PlatInterface sharedInstance] getBattery];
        NSString *str = [NSString stringWithFormat:@"%f", battery];
        UnitySendMessage("PlatInterface", "onBatteryBack", [str UTF8String]);
        
    }
}

extern "C"{
    void GetNetWorkType_IOS(){
        NSLog(@"ios call =============GetNetWorkType_IOS");
        int nettype =[[PlatInterface sharedInstance] getNetWorkType];
        NSString *str = [NSString stringWithFormat:@"%d", nettype];
        NSLog(@"netType%d",nettype);
        UnitySendMessage("PlatInterface", "onNetWorkTypeBack", [str UTF8String]);
        
    }
}

extern "C"{
	void CopyStr_IOS(const char * str){
        if (str) {
            NSString* nsStr = [NSString stringWithUTF8String:str];
            UIPasteboard *board = [UIPasteboard generalPasteboard];
            board.string = nsStr;
        }
	}
}
