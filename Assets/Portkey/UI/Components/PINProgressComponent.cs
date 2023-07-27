using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Portkey.UI
{
    public class PINProgressComponent : MonoBehaviour
    {
        [SerializeField] private List<Image> pinProgressIndicatorList;
        [SerializeField] private Color activatedColor;
        [SerializeField] private Color deactivatedColor;
        
        public void SetPINProgress(int progress)
        {
            progress = Math.Min(progress, pinProgressIndicatorList.Count);
            
            for (var i = 0; i < pinProgressIndicatorList.Count; i++)
            {
                var color = i < progress ? activatedColor : deactivatedColor;
                pinProgressIndicatorList[i].color = color;
            }
        }
        
        public int GetMaxPINLength()
        {
            return pinProgressIndicatorList.Count;
        }
    }
}