
mergeInto(LibraryManager.library, {

  Listen: function () {
  
    const onMessage = (event) => {
      const type = event.data.type;
      switch (type) {
        case 'PortkeySocialLoginOnSuccess':
          window.unityInstance.SendMessage('WebGLPortkeyGoogleLoginCallback', 'OnSuccess', JSON.stringify(event.data.data));

          console.log(event.data.data);
          break;
        case 'PortkeySocialLoginOnFailure':
          window.unityInstance.SendMessage('WebGLPortkeyGoogleLoginCallback', 'OnFailure', JSON.stringify(event.data.error));

          console.log(event.data.error);
          break;
        default:
          return;
      }
      window.removeEventListener('message', onMessage);
    };
  
    window.addEventListener('message', onMessage);
  },

});