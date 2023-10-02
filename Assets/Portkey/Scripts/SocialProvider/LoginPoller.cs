using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Portkey.Core;
using Portkey.Utilities;
using UnityEngine;

namespace Portkey.SocialProvider
{
    public class LoginPoller : ILoginPoller
    {
        private class PollConfig
        {
            public float pollInterval;
            public float timeOut;
            public LoginPollerHandler handler;
        }

        private readonly IPortkeySocialService _portkeySocialService;
        private readonly Dictionary<int, Coroutine> _coroutines = new Dictionary<int, Coroutine>();
        private int _currentIndex = 0;

        public LoginPoller(IPortkeySocialService portkeySocialService)
        {
            _portkeySocialService = portkeySocialService;
            _currentIndex = 0;
        }

        public LoginPollerHandler Start(string chainId, ISigningKey signingKey, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback, float timeOut, float pollInterval)
        {
            var handler = new LoginPollerHandler
            {
                id = _currentIndex
            };
            
            var pollConfig = new PollConfig
            {
                timeOut = timeOut,
                pollInterval = pollInterval,
                handler = handler
            };
            
            var coroutine = StaticCoroutine.StartCoroutine(WaitForResponse(0.0f, pollConfig, chainId, signingKey, successCallback, errorCallback));
            
            _coroutines.Add(_currentIndex++, coroutine);
            return handler;
        }

        public void Stop(LoginPollerHandler handler)
        {
            if (!_coroutines.TryGetValue(handler.id, out var coroutine))
            {
                throw new Exception("LoginHandler does not exists!");
            }
            StaticCoroutine.StopCoroutine(coroutine);
            _coroutines.Remove(handler.id);
        }
        
        private IEnumerator WaitForResponse(float timer, PollConfig pollConfig, string chainId, ISigningKey signingKey, SuccessCallback<PortkeyAppLoginResult> successCallback, ErrorCallback errorCallback)
        {
            yield return new WaitForSeconds(pollConfig.pollInterval);
            timer += pollConfig.pollInterval;

            var param = new GetCAHolderByManagerParams
            {
                manager = signingKey.Address,
                chainId = chainId
            };
            yield return _portkeySocialService.GetHolderInfoByManager(param, result =>
            {
                foreach (var caHolder in result.caHolders)
                {
                    if (caHolder.holderManagerInfo.managerInfos.All(manager => manager.address != signingKey.Address))
                    {
                        continue;
                    }
                    var loginResult = new PortkeyAppLoginResult
                    {
                        caHolder = caHolder, 
                        managementAccount = signingKey
                    };
                    successCallback(loginResult);
                    return;
                }

                RestartWaitForResponse();
            }, error =>
            {
                Debugger.LogError(error);
                RestartWaitForResponse();
            });

            void RestartWaitForResponse()
            {
                if (pollConfig.timeOut > 0.0f && timer > pollConfig.timeOut)
                {
                    errorCallback("Login timeout");
                    return;
                }

                if (!_coroutines.ContainsKey(pollConfig.handler.id))
                {
                    errorCallback("Poller not found!");
                    return;
                }

                //if we did not find the manager, we poll again
                _coroutines[pollConfig.handler.id] = StaticCoroutine.StartCoroutine(WaitForResponse(timer, pollConfig, chainId, signingKey, successCallback, errorCallback));
            }
        }
    }
}