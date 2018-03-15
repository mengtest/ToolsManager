//
//  OverrideAppDelegate.h
//  MemoryWarningReporterIOS
//
//  Created by xclouder on 2017/8/1.
//  Copyright © 2017年 xclouder. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface OverrideAppDelegate : NSObject

- (void)origin_applicationDidReceiveMemoryWarning:(UIApplication *)application;
- (void)applicationDidReceiveMemoryWarning:(UIApplication *)application;

@end
