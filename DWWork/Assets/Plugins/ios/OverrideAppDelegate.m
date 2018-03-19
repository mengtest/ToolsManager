//
//  OverrideAppDelegate.m
//  MemoryWarningReporterIOS
//
//  Created by xclouder on 2017/8/1.
//  Copyright © 2017年 xclouder. All rights reserved.
//

#import "OverrideAppDelegate.h"
#import "MemoryWarningReporter.h"

@implementation OverrideAppDelegate

- (void)origin_applicationDidReceiveMemoryWarning:(UIApplication *)application
{
    //Empty
}

- (void)applicationDidReceiveMemoryWarning:(UIApplication *)application
{
    [self origin_applicationDidReceiveMemoryWarning:application];
    
    [[MemoryWarningReporter instance] report];
}


@end
