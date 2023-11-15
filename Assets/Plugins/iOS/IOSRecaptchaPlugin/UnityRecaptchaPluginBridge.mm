//
//  UnityRecaptchaPluginBridge.mm
//  UnityFramework
//
//  Created by Zhi Feng on 8/11/23.
//

#import <Foundation/Foundation.h>
#import <UnityFramework/UnityFramework-Swift.h>

NSString* CreateNSString (const char* string)
{
  if (string)
    return [NSString stringWithUTF8String: string];
  else
    return [NSString stringWithUTF8String: ""];
}

extern "C" {
    void RecaptchaVerify(const char* siteKey) {
        NSString* key = [NSString stringWithUTF8String:siteKey];
        [UnityRecaptchaPlugin recaptchaVerifyWithRecaptchaSiteKey:key];
    }
}
