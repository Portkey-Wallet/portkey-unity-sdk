
mergeInto(LibraryManager.library, {

  Listen: function () {
  
    const onMessage = (event) => {
      const type = event.data.type;
      switch (type) {
        case 'PortkeySocialLoginOnSuccess':
          window.unityInstance.SendMessage('PortkeyUICanvas', 'WebGLPortkeySocialLoginOnSuccess', JSON.stringify(event.data.data));
          //resolve(event.data.data);
          console.log(event.data.data);
          break;
        case 'PortkeySocialLoginOnFailure':
          window.unityInstance.SendMessage('PortkeyUICanvas', 'WebGLPortkeySocialLoginOnFailure', JSON.stringify(event.data.error));
          //reject(event.data.error);
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