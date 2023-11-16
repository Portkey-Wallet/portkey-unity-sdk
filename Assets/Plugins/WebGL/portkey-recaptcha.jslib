
mergeInto(LibraryManager.library, {

  ExecuteRecaptcha: function (sitekey) {
    // adding google recaptcha api script onto html header
    var script = document.createElement('script');
    script.type = 'text/javascript';
    script.text = "var verifyCallback = function(response) { document.getElementById('overlay').remove(); window.unityInstance.SendMessage('PortkeyGoogleRecaptchaCallback', 'OnCaptchaSuccess', response); }; var onloadCallback = function() { grecaptcha.render(document.getElementById('recaptchaDiv'), {'sitekey' : '" + UTF8ToString(sitekey) + "', 'callback' : verifyCallback}); };";
    document.head.appendChild(script);

    var apiScript = document.createElement('script');
    apiScript.type = 'text/javascript';
    apiScript.src = 'https://www.google.com/recaptcha/api.js?onload=onloadCallback&render=explicit';
    apiScript.async = true;
    apiScript.defer = true;
    document.body.appendChild(apiScript);
    
    var div = document.createElement("div");
    div.id = "overlay";
    div.style.width = "100%";
    div.style.height = "100%";
    div.style.top = "0";
    div.style.left = "0";
    div.style.position = "fixed";
    div.style.backgroundColor = "rgba(0, 0, 0, 0.7)";
    div.style.display = "flex";
    div.style.alignItems = "center";
    div.style.justifyContent = "center";
    div.style.color = "white";
    
    var recaptchaDiv = document.createElement("div");
    recaptchaDiv.id = "recaptchaDiv";
    recaptchaDiv.style.width = "auto";
    recaptchaDiv.style.height = "auto";
    recaptchaDiv.style.top = "50%";
    recaptchaDiv.style.left = "50%";
    recaptchaDiv.style.position = "absolute";
    recaptchaDiv.style.transform = "translate(-50%, -50%)";

    div.appendChild(recaptchaDiv);
    
    document.body.appendChild(div);

    var overlayScript = document.createElement('script');
    overlayScript.type = 'text/javascript';
    overlayScript.text = "function overlayClickCallback() {" +
        "  window.unityInstance.SendMessage('PortkeyGoogleRecaptchaCallback', 'OnCaptchaError', 'Closed captcha verification!');" +
        "}" +
        "var overlay = document.getElementById('overlay');" +
        "overlay.addEventListener(\"click\", function() {" +
        "  overlayClickCallback();" +
        "  overlay.remove();" +
        "  document.body.style.overflow = \"auto\";" +
        "});";
    document.body.appendChild(overlayScript);
  },

});