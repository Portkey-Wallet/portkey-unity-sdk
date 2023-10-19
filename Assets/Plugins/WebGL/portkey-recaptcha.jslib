
mergeInto(LibraryManager.library, {

  ExecuteRecaptcha: function (url) {
    // adding google recaptcha api script onto html header
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.text = "var verifyCallback = function(response) { document.getElementById('recaptchaDiv').remove(); alert(response); }; var onloadCallback = function() { grecaptcha.render(document.getElementById('recaptchaDiv'), {'sitekey' : '6LfR_bElAAAAAJSOBuxle4dCFaciuu9zfxRQfQC0', 'callback' : verifyCallback}); };";
    document.head.appendChild(script);

    var apiScript = document.createElement('script');
    apiScript.type = 'text/javascript';
    apiScript.src = 'https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit';
    apiScript.async = true;
    apiScript.defer = true;
    document.body.appendChild(apiScript);
    
    var div = document.createElement("div");
    div.id = "recaptchaDiv";
    div.style.width = "auto";
    div.style.height = "auto";
    div.style.top = "50%";
    div.style.left = "50%";
    div.style.position = "absolute";
    div.style.transform = "translate(-50%, -50%)";
    
    document.getElementById("unity-container").appendChild(div);
  },

});