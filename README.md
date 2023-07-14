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
- Unity Package: com.unity.nuget.newtonsoft-json@3.2 (Newtonsoft)

### Dependencies
- Google Protobuf 3.19.4
    - If you require to generate C# classes from .proto, please use the installation below:
        ```
        brew install protobuf@21
        ```

To start using the Portkey Unity SDK, follow these steps:

1. Clone the repository or download the SDK package from the [releases](https://github.com/Portkey-Wallet/portkey-unity-sdk/releases) section.
2. Extract the contents of the package to a desired location in your Unity project.
3. Open your Unity project.
4. In the Unity editor, navigate to **Assets > Import Package > Custom Package**.
5. Select the extracted package file (e.g., `UnitySDK.unitypackage`) and click **Open**.
6. Unity will import the SDK assets and libraries into your project.

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

## Usage
To use the Unity SDK in your project, follow these steps:

1. Open your Unity project.
2. Ensure that the SDK is properly installed (see [Installation](#installation)).
3. In the Unity editor, navigate to the desired scene or create a new scene.
4. Drag and drop the provided SDK prefabs, scripts, or assets into your scene hierarchy or project assets.
5. Configure the SDK settings, such as API keys or initialization parameters, as required.
6. Write your custom code to interact with the SDK's functionality.
7. Build and run your Unity application or game to test the integration.

## Examples (TODO)
Check out the following code snippet to see an example of how to use the Unity SDK:

```csharp
// Example code demonstrating the usage of Unity SDK

using UnityEngine;
using YourNamespace;

public class ExampleScript : MonoBehaviour
{
    private UnitySDK sdk;

    private void Start()
    {
        // Initialize the SDK
        sdk = new UnitySDK();
        sdk.Init();
    }

    private void Update()
    {
        // Perform SDK operations
        sdk.DoSomething();
    }
}
```

For more detailed examples and sample projects, refer to the [Examples](examples/) directory of this repository.

## Contributing
We welcome contributions to the Portkey Unity SDK! To contribute, please follow these guidelines:

1. Fork the repository and create a new branch.
2. Make your changes and ensure they follow the project's coding style.
3. Write appropriate unit tests for your changes if applicable.
4. Commit your changes and push them to your fork.
5. Submit a pull request, providing a detailed description of your changes.

## License
This project is licensed under the [MIT License](LICENSE.txt).