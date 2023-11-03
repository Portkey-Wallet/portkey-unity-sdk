# Unity SDK README

## Table of Contents
- [Introduction](#introduction)
- [Features](#features)
- [Installation](#installation)
- [Usage](#usage)
- [Examples](#examples)
- [Contributing](#contributing)
- [License](#license)

## Introduction
Welcome to the Portkey Unity SDK! This software development kit (SDK) is designed to provide developers with a set of tools, libraries, and resources to build Aelf Blockchain Unity applications and games more efficiently. The SDK is built on top of the Unity game engine and provides additional functionality and convenience for Unity developers.

## Features
- **Feature 1**: Portkey DID Wallet Account Management.
- **Feature 2**: Contract Account (AA) SignUp/SignIn capabilities.
  - Google
  - Apple
  - Email
  - Phone
- **Feature 3**: Google ReCaptcha when signing up.

## Installation

### Requirements
- Unity 2021.3.26f1 or later
- Xcode 12.5 or later (for iOS builds)
- Android Studio 4.2.2 or later (for Android builds)

### Setup

To start using the Portkey Unity SDK, follow these steps:

1. Set up your Unity project.
2. Open <root>/Packages/manifest.json
3. Add the following line to the dependencies section:
    ```
    "nuget.moq": "1.0.0",
    "com.unity.nuget.newtonsoft-json": "3.2.1",
    ```
4. Download the SDK package from the [releases](https://github.com/Portkey-Wallet/portkey-unity-sdk/releases) section.
5. Open your Unity project.
6. In the Unity editor, navigate to **Assets > Import Package > Custom Package**.
7. Select the SDK package file (e.g., `portkey-unity-sdk-v1.0.0-alpha.unitypackage`) and click **Open**.
8. Unity will import the SDK assets and libraries into your project.
9. Restart your Unity editor.

#### iOS Setup

We are using External Dependency Manager for Unity (EDM4U) to manage iOS dependencies. Therefore we will need to install some dependencies in order to build the project on iOS.

1. Install rvm
    ```
    curl -sSL https://get.rvm.io | bash -s stable
    ```
2. Install ruby 3.2.2
    ```
    rvm install 3.2.2
    ```
3. Restart Unity
4. Open Unity with super user permission through the command below:
    ```
    sudo /Applications/Unity/Hub/Editor/<Your Unity Version>/Unity.app/Contents/MacOS/unity
    ```

### Tools
- GraphQL C# Code Generator (For generating data structs of GraphQL's query responses)
    - Go to your Terminal
    - cd to Tools/ directory.
    - Execute the following commands in your Terminal
        ```
        npm install
        npm run codegen
        ```
    - The generated C# code will appear in Assets/Portkey/Scripts/__Generated__/GraphQLCodeGen.cs
- Contract C# Code generator to generate C# classes from .proto files
    - https://github.com/AElfProject/contract-plugin/blob/master/src/contract_csharp_generator.cc
- Google Protobuf 3.19.4
    - If you require to generate C# classes from .proto, please use the installation below:
        ```
        brew install protobuf@21
        export PATH="$(brew --prefix protobuf@21)/bin:$PATH"
        ```

## Usage
To use the Unity SDK in your project, follow these steps:

1. Open your Unity project.
2. Ensure that the SDK is properly installed (see [Installation](#installation)).
3. In the Unity editor, navigate to the desired scene or create a new scene.
4. Drag and drop the provided SDK prefabs, scripts, or assets into your scene hierarchy or project assets.
5. Configure the SDK settings, such as API keys or initialization parameters, as required.
6. Write your custom code to interact with the SDK's functionality.
7. Build and run your Unity application or game to test the integration.

## Examples

### **Google Sign In Example**

The following code snippet demonstrates how to use the Portkey Unity SDK to sign in with Google:

```csharp
using Portkey.DID;

public class GoogleLoginExample : MonoBehaviour
{
    [SerializeField] private PortkeySDK _portkeySDK;

    public void SignInWithGoogle()
    {
        _portkeySDK.AuthService.GoogleCredentialProvider.Get(credential => 
        {
            StartCoroutine(_portkeySDK.AuthService.GetGuardians(credential, guardians =>
            {
                if(guardians.Count == 0)
                {
                    // No guardians, proceed to sign up
                    StartCoroutine(_portkeySDK.AuthService.SignUp(credential, didWalletInfo => 
                    {
                        // user has successfully signed up and his info can be found in didWalletInfo
                    }));
                }
                else
                {
                    List<ApprovedGuardian> approvedGuardians = new List<ApprovedGuardian>();
                    // Guardians exist, proceed to sign in
                    for (int i = 0; i < _portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(guardians.Count); ++i)
                    {
                        StartCoroutine(_portkeySDK.AuthService.Verify(guardians[i], approvedGuardian =>
                        {
                            approvedGuardians.Add(approvedGuardian);
                        }));
                    }
                    
                    while(approvedGuardians.Count < _portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(guardians.Count))
                    {
                        // wait for guardians to be approved
                    }

                    // first element of the guardians array is the login guardian
                    StartCoroutine(_portkeySDK.AuthService.Login(guardians[0], approvedGuardians, didWalletInfo =>
                    {
                        // user is logged in and his info can be found in didWalletInfo
                    }));
                }
            }));
        });
    }
}
```

### **Apple Sign In Example**

The following code snippet demonstrates how to use the Portkey Unity SDK to sign in with Apple:

```csharp
using Portkey.DID;

public class AppleLoginExample : MonoBehaviour
{
    [SerializeField] private PortkeySDK _portkeySDK;

    public void SignInWithApple()
    {
        _portkeySDK.AuthService.AppleCredentialProvider.Get(credential => 
        {
            StartCoroutine(_portkeySDK.AuthService.GetGuardians(credential, guardians =>
            {
                if(guardians.Count == 0)
                {
                    // No guardians, proceed to sign up
                    StartCoroutine(_portkeySDK.AuthService.SignUp(credential, didWalletInfo => 
                    {
                        // user has successfully signed up and his info can be found in didWalletInfo
                    }));
                }
                else
                {
                    List<ApprovedGuardian> approvedGuardians = new List<ApprovedGuardian>();
                    // Guardians exist, proceed to sign in
                    for (int i = 0; i < _portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(guardians.Count); ++i)
                    {
                        StartCoroutine(_portkeySDK.AuthService.Verify(guardians[i], approvedGuardian =>
                        {
                            approvedGuardians.Add(approvedGuardian);
                        }));
                    }
                    
                    while(approvedGuardians.Count < _portkeySDK.AuthService.GetRequiredApprovedGuardiansCount(guardians.Count))
                    {
                        // wait for guardians to be approved
                    }

                    // first element of the guardians array is the login guardian
                    StartCoroutine(_portkeySDK.AuthService.Login(guardians[0], approvedGuardians, didWalletInfo =>
                    {
                        // user is logged in and his info can be found in didWalletInfo
                    }));
                }
            }));
        });
    }
}
```

## Contributing
We welcome contributions to the Portkey Unity SDK! To contribute, please follow these guidelines:

1. Fork the repository and create a new branch.
2. Make your changes and ensure they follow the project's coding style.
3. Write appropriate unit tests for your changes if applicable.
4. Commit your changes and push them to your fork.
5. Submit a pull request, providing a detailed description of your changes.

## License
This project is licensed under the [MIT License](LICENSE.txt).