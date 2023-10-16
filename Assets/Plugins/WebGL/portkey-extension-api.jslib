mergeInto(LibraryManager.library, {

  IsPortkeyExtensionExist: function () {
    var portkey = window.portkey;
    if (portkey) {
      return true;
    } else {
      return false;
    }
  },
  
  Connect: async function () {
    try {
        var provider = window.portkey;
        const accounts = await provider.request({ method: 'requestAccounts' });
        window.unityInstance.SendMessage('PortkeyExtensionConnectCallback', 'OnConnect', JSON.stringify(accounts));
        console.log(accounts);
    } catch (e) {
        console.log(e);
        // An error will be thrown if the user denies the permission request.
        console.log('user denied the permission request');
        window.unityInstance.SendMessage('PortkeyExtensionConnectCallback', 'OnError', 'User denied the permission request!');
    }
  },
  
  SignMessage: async function (messageInHex) {
    try {
      var provider = window.portkey;
      const signature = await provider.request({
        method: 'wallet_getSignature',
        payload: {
          data: UTF8ToString(messageInHex),
        }
      });
      if (!signature) throw new Error('Sign failed!');
      window.unityInstance.SendMessage('PortkeyExtensionSignCallback', 'OnSign', JSON.stringify(signature));
    } catch (e) {
        console.log(e);
        window.unityInstance.SendMessage('PortkeyExtensionSignCallback', 'OnError', 'Sign failed!');
    }
  },

  GetCurrentManagerAddress: async function () {
    try {
        var provider = window.portkey;
        const managerAddress = await provider.request({ method: 'wallet_getCurrentManagerAddress' });
        window.unityInstance.SendMessage('PortkeyExtensionConnectCallback', 'OnGetManagementAccountAddress', managerAddress);
    } catch (e) {
        console.log(e);
        console.log('fail to get management account address');
        window.unityInstance.SendMessage('PortkeyExtensionConnectCallback', 'OnError', 'Unable to get management account address!');
    }
  }
});