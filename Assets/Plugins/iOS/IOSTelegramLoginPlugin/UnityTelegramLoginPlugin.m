//
//  UnityGoogleLoginPlugin.m
//  UnityFramework
//
//  Created by Zhi Feng on 14/8/23.
//

#import <Foundation/Foundation.h>
#import "UnityTelegramLoginPlugin.h"
#import <GIDSignIn.h>
#import <GIDSignInResult.h>
#import <GIDGoogleUser.h>
#import <GIDToken.h>

@implementation UnityTelegramLoginPlugin

+ (void) SignIn {
  [GIDSignIn.sharedInstance
      signInWithPresentingViewController:UnityGetGLViewController()
                              completion:^(GIDSignInResult * _Nullable signInResult,
                                           NSError * _Nullable error) {
        if (error) {
            UnitySendMessage("IOSPortkeyTelegramLoginCallback", "OnFailure", [error.domain.description UTF8String]);
            return;
        }


    UnitySendMessage("IOSPortkeyTelegramLoginCallback", "OnSuccess", [signInResult.user.accessToken.tokenString UTF8String]);
  }];
}

@end
