//
//  IOSAppCheckPlugin.m
//
//  Created by Andre Pothier on 2019-02-12.
//  Copyright © 2019 Andre Pothier. All rights reserved.
//

#import <Foundation/Foundation.h>
#import <UIKit/UIKit.h>

@interface AppCheck : NSObject
{
}
@end


@implementation AppCheck

static AppCheck *_sharedInstance;
+(AppCheck*) sharedInstance
{
    static dispatch_once_t onceToken;
    dispatch_once( &onceToken, ^{
        NSLog(@"Creating Shared Instance");
        _sharedInstance = [[AppCheck alloc] init];
    });
    return _sharedInstance;
}

-(id) init
{
    self = [super init];
    if(self)
        [self initHelper];
    
    return self;
}

-(void) initHelper
{
    NSLog(@"Initilized");
}

-(void) Start
{
    //DO START UP CODE HERE IF NEEDED
    NSLog(@"Started AppCheck");
}

-(bool)Search: (NSURL*) URL{
    UIApplication *application = [UIApplication sharedApplication];
    
    return [application canOpenURL:URL];
}

-(bool)CheckApp: (NSString*) URL_LOC
{
    NSString* URL = [URL_LOC stringByAppendingString: @"://"];
    NSURL* customURL = [NSURL URLWithString:URL];
    
    bool ans = [self Search:customURL];
    
    return ans;
}

@end

extern "C" {
    void InitilizeAppCheck(){
        [[AppCheck sharedInstance] Start];
    }
    bool CheckApp(const char* URL){
        if(URL)
            return [[AppCheck sharedInstance] CheckApp:[NSString stringWithUTF8String: URL]];
        else
            return false;
    }
}
