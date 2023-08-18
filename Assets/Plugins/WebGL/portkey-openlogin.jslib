
mergeInto(LibraryManager.library, {

  OpenURL: function (url) {
  
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
    const windowOpener = window.open(UTF8ToString(url));
    
    const timer = setInterval(() => {
      if (windowOpener != null && windowOpener.closed) {
          clearInterval(timer);
          
          window.unityInstance.SendMessage('WebGLPortkeyGoogleLoginCallback', 'OnFailure', 'Login window closed.');
  
          window.removeEventListener('message', onMessage);
          timer = null;
      }
    }, 1600);
  },

});