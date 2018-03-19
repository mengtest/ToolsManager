//
//  MemoryWarningReporter.m
//  MemoryWarningReporterIOS
//
//  Created by xclouder on 2017/8/1.
//  Copyright © 2017年 xclouder. All rights reserved.
//

#import "MemoryWarningReporter.h"

__strong static MemoryWarningReporter *m_ins;


@implementation MemoryWarningReporter

+ (instancetype)instance
{
    if (self == MemoryWarningReporter.class)
    {
        if (m_ins == nil)
        {
            m_ins = [self new];
        }
    }
    
    return m_ins;
}

- (void)report
{
    NSLog(@"dispatch memory warning");
    
    UnitySendMessage("MemoryWarnReporter", "_OnMemoryWarningIOS", "");
}


@end
