//
//  UnityBiometricPlugin.mm
//  UnityFramework
//
//  Created by Zhi Feng on 21/8/23.
//

#import <Foundation/Foundation.h>
#import <UnityFramework/UnityFramework-Swift.h>

extern "C" {
    void BiometricAuthenticate() {
        [UnityBiometricPlugin Authenticate];
    }
    
    void BiometricCanAuthenticate() {
        [UnityBiometricPlugin CanAuthenticate];
    }
}
