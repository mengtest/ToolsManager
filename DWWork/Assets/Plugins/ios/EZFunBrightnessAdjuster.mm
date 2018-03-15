//
//  EZFunBrightnessAdjuster.m
//  BrightnessAdjusterIOS
//
//  Created by xclouder on 2017/8/9.
//  Copyright © 2017年 xclouder. All rights reserved.
//

#import "EZFunBrightnessAdjuster.h"
#import <UIKit/UIKit.h>

@implementation EZFunBrightnessAdjuster

extern "C" {
    void __IOS_SetBrightness(float brightness) {
        [[UIScreen mainScreen] setBrightness:brightness];
    }
    
    float __IOS_GetBrightness() {
        return [[UIScreen mainScreen] brightness];
    }
}

@end
