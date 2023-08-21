//
//  UnityBiometricPlugin.swift
//  UnityFramework
//
//  Created by Zhi Feng on 21/8/23.
//

import Foundation
import BiometricAuthentication

@objc public class UnityBiometricPlugin: NSObject {
    
    @objc public static func Authenticate() {
        
        if BioMetricAuthenticator.canAuthenticate() {
            BioMetricAuthenticator.authenticateWithBioMetrics(reason: "Biometric Authentication") { (result) in
                
                switch result {
                    case .success( _):

                        print("Authentication successful.")
                        UnityFramework.getInstance().sendMessageToGO(withName: "IOSPortkeyBiometricCallback", functionName: "OnSuccess", message: "Authentication successful.")

                    case .failure(let error):

                        switch error {

                        // do nothing on canceled by system or user
                        case .canceledBySystem, .canceledByUser:
                            UnityFramework.getInstance().sendMessageToGO(withName: "IOSPortkeyBiometricCallback", functionName: "OnFailure", message: "Cancelled")
                            break

                        // show error for any other reason
                        default:
                            print(error.message())
                            UnityFramework.getInstance().sendMessageToGO(withName: "IOSPortkeyBiometricCallback", functionName: "OnFailure", message: error.message())
                        }
                    }
            }
        }
    }
}
