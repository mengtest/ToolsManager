//
//  IosSwizzle.m
//  MemoryWarningReporterIOS
//
//  Created by xclouder on 2017/8/1.
//  Copyright © 2017年 xclouder. All rights reserved.
//

#import "IosSwizzle.h"
#import "OverrideAppDelegate.h"
#import "EPPZSwizzler.h"

__strong IosSwizzle *_instance;

@implementation IosSwizzle

+ (void)load
{
    NSLog(@"[IosSwizzle load]");
    [self swizzle];
}

+ (IosSwizzle *)instance
{
    return _instance;
}

+ (void)swizzle
{
    NSLog(@"[IosSwizzle do swizzle]");
    
    Class unityAppDelegate = NSClassFromString(@"UnityAppController");
    Class overrideAppDelegate = OverrideAppDelegate.class;
    
    //[EPPZSwizzler setLogging:YES];
    
    if ([EPPZSwizzler hasInstanceMethod:@selector(applicationDidReceiveMemoryWarning:) ofClass:unityAppDelegate])
    {
        //method exist, replace
        
        // Add empty placholder to Unity app delegate.
        [EPPZSwizzler addInstanceMethod:@selector(origin_applicationDidReceiveMemoryWarning:)
                                toClass:unityAppDelegate
                              fromClass:overrideAppDelegate];
        
        // Save the original Unity app delegate implementation into.
        [EPPZSwizzler swapInstanceMethod:@selector(origin_applicationDidReceiveMemoryWarning:)
                      withInstanceMethod:@selector(applicationDidReceiveMemoryWarning:)
                                 ofClass:unityAppDelegate];
        
        // Replace Unity app delegate with ours.
        [EPPZSwizzler replaceInstanceMethod:@selector(applicationDidReceiveMemoryWarning:)
                                    ofClass:unityAppDelegate
                                  fromClass:overrideAppDelegate];
    }
    else
    {
        // Add empty placholder to Unity app delegate.
        [EPPZSwizzler addInstanceMethod:@selector(origin_applicationDidReceiveMemoryWarning:)
                                toClass:unityAppDelegate
                              fromClass:overrideAppDelegate];
        
        //method not exist, add one
        [EPPZSwizzler addInstanceMethod:@selector(applicationDidReceiveMemoryWarning:)
                                toClass:unityAppDelegate
                              fromClass:overrideAppDelegate];

    }
    
}

@end
