//
//  UnityGoogleLoginPlugin.m
//  UnityFramework
//
//  Created by Zhi Feng on 14/8/23.
//

#import <Foundation/Foundation.h>
#import "UnityGoogleLoginPlugin.h"
#import <GIDSignIn.h>
#import <GIDSignInResult.h>
#import <GIDGoogleUser.h>
#import <GIDToken.h>

@implementation UnityGoogleLoginPlugin

+ (void) SignIn {
  [GIDSignIn.sharedInstance
      signInWithPresentingViewController:UnityGetGLViewController()
                              completion:^(GIDSignInResult * _Nullable signInResult,
                                           NSError * _Nullable error) {
        if (error) {
            UnitySendMessage("PortkeyUICanvas", "IOSPortkeySocialLoginOnFailure", [error.domain.description UTF8String]);
            return;
        }


    UnitySendMessage("PortkeyUICanvas", "IOSPortkeySocialLoginOnSuccess", [signInResult.user.accessToken.tokenString UTF8String]);
  }];
}

@end
