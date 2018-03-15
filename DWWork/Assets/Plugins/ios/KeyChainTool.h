//
//  KeyChainTool.h
//  PlatInterface
//
//  Created by leishengchao on 2017/6/26.
//  Copyright © 2017年 leishengchao. All rights reserved.
//

#import <Foundation/Foundation.h>

@interface KeyChainTool : NSObject
+ (void)save:(NSString *)service data:(id)data;
+ (id)load:(NSString *)service;
+ (void)deleteKeyData:(NSString *)service;
@end
